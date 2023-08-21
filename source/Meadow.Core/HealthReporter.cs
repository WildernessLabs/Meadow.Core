using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Meadow.Cloud;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;

namespace Meadow;

/// <summary>
/// Logic responsible for reporting device health metrics to Meadow.Cloud.
/// </summary>
public class HealthReporter : IHealthReporter
{
    static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
    
    /// <inheritdoc/>
    public void Start(int interval)
    {
        System.Timers.Timer timer = new(interval: interval * 60 * 1000);
        timer.Elapsed += async (sender, e) => await TimerOnElapsed(sender, e);
        timer.AutoReset = true;

        Resolver.Device.NetworkAdapters.NetworkConnected += async (sender, args) =>
        {
            Resolver.Log.Trace($"starting health metrics timer");
            await Send();
            timer.Start();
        };
    }

    /// <inheritdoc/>
    public async Task Send()
    {
        var connected = Resolver.Device.NetworkAdapters.Any(a => a.IsConnected);

        if (!connected) return;

        try
        {
            await semaphoreSlim.WaitAsync();

            var service = Resolver.Services.Get<IMeadowCloudService>();
            var device = Resolver.Device;
            DirectoryInfo di = new DirectoryInfo("/meadow0");
            var usedDiskSpace = DirSize(di);

            var ce = new CloudEvent()
            {
                Description = "device.health",
                EventId = 10,
                Measurements = new Dictionary<string, object>()
                {
                    { "cpu_temp_celsius", device.PlatformOS.GetCpuTemperature().Celsius },
                    { "memory_used", GC.GetTotalMemory(false) },
                    { "disk_space_used", usedDiskSpace }
                },
                Timestamp = DateTime.UtcNow
            };
            
            var batteryInfo = device.GetBatteryInfo();
            if (batteryInfo != null)
            {
                ce.Measurements.Add("battery_percentage", batteryInfo.StateOfCharge);
            }

            await service!.SendEvent(ce);
            Resolver.Log.Trace($"health metrics sent");
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }
    
    private async Task TimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        await Send();
    }

    private long DirSize(DirectoryInfo d)
    {
        long size = 0;
        // Add file sizes.
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            size += fi.Length;
        }

        // Add subdirectory sizes.
        DirectoryInfo[] dis = d.GetDirectories();
        foreach (DirectoryInfo di in dis)
        {
            size += DirSize(di);
        }

        return size;
    }
}