using Meadow.Cloud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Meadow;

/// <summary>
/// Logic responsible for reporting device health metrics to Meadow.Cloud.
/// </summary>
public class HealthReporter : IHealthReporter
{
    private static readonly SemaphoreSlim semaphoreSlim = new(1, 1);

    /// <inheritdoc/>
    public void Start(int interval)
    {
        System.Timers.Timer timer = new(interval: interval * 60 * 1000);
        timer.Elapsed += async (sender, e) => await TimerOnElapsed(sender, e);
        timer.AutoReset = true;

        Resolver.Device.NetworkAdapters.NetworkConnected += async (sender, args) =>
        {
            Resolver.Log.Trace($"starting health metrics timer");
            timer.Start();

            // send the first health metric
            await Send();
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

            DirectoryInfo di = new(Resolver.Device.PlatformOS.FileSystem.FileSystemRoot);

            var usedDiskSpace = DirSize(di);

            var ce = new CloudEvent()
            {
                Description = "device.health",
                EventId = 10,
                Measurements = new Dictionary<string, object>()
                {
                    { "health.cpu_temp_celsius", device.PlatformOS.GetCpuTemperature().Celsius },
                    { "health.memory_used", GC.GetTotalMemory(false) },
                    { "health.disk_space_used", usedDiskSpace },
                    { "info.os_version", device.Information.OSVersion },

                },
                Timestamp = DateTimeOffset.UtcNow
            };

            var batteryInfo = device.GetBatteryInfo();
            if (batteryInfo != null)
            {
                ce.Measurements.Add("health.battery_percentage", batteryInfo.StateOfCharge);
            }

            if (!string.IsNullOrEmpty(device.Information.CoprocessorOSVersion))
            {
                ce.Measurements.Add("info.coprocessor_os_version", device.Information.CoprocessorOSVersion);
            }

            if (await service!.SendEvent(ce))
            {
                Resolver.Log.Trace($"health metrics sent");
            }
        }
        finally
        {
            semaphoreSlim.Release();
        }
    }

    private Task TimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        return Send();
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