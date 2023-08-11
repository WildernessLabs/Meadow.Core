using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Meadow.Cloud;

namespace Meadow;

public class HealthReporter : IHealthReporter
{
    static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    /// <param name="interval">In minutes</param>
    public void Start(int interval)
    {
        System.Timers.Timer timer = new(interval: interval * 60 * 1000);
        timer.Elapsed += async (sender, e) => await TimerOnElapsed(sender, e);
        timer.Start();
    }

    private async Task TimerOnElapsed(object sender, ElapsedEventArgs e)
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

            await service!.SendEvent(new CloudEvent()
            {
                Description = "device.health",
                EventId = 0,
                Measurements = new Dictionary<string, object>()
                {
                    // {"cpu", }, // this is available on the OS but no API for it yet.
                    // {"boot_count", } // a ctacke gem not yet available in OS.
                    { "cpu_temp_celsius", device.PlatformOS.GetCpuTemperature().Celsius },
                    { "memory_used", GC.GetTotalMemory(false) },
                    { "disk_space_used", usedDiskSpace },
                    { "battery_percentage", device.GetBatteryInfo().StateOfCharge }
                },
                Timestamp = DateTime.UtcNow
            });
            Resolver.Log.Trace($"health metrics sent");
        }
        finally
        {
            semaphoreSlim.Release();
        }
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