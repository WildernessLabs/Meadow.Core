using Meadow.Cloud;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Meadow;

/// <summary>
/// Logic responsible for reporting device health metrics to Meadow.Cloud.
/// </summary>
public class HealthReporter : IHealthReporter
{
    /// <inheritdoc/>
    public async Task Start(int interval)
    {
        Resolver.Log.Info($"Health Metrics enabled with interval: {interval} minute(s).");

        System.Timers.Timer timer = new(interval: interval * 60 * 1000);
        timer.Elapsed += async (sender, e) => await TimerOnElapsed(sender, e);
        timer.AutoReset = true;

        var anyAdapter = Resolver.Device.NetworkAdapters.Primary<INetworkAdapter>();

        // if we're already connected, start the timer
        if (anyAdapter != null && anyAdapter.IsConnected)
        {
            Resolver.Log.Trace($"starting health metrics timer");
            timer.Start();

            await Send();
        }

        Resolver.Device.NetworkAdapters.NetworkConnected += async (sender, args) =>
        {
            // TODO: what happens if we disconnect and reconnect?

            if (!timer.Enabled)
            {
                Resolver.Log.Trace($"starting health metrics timer");
                timer.Start();

                // send the first health metric
                await Send();
            }
        };
    }

    /// <inheritdoc/>
    public async Task Send()
    {
        var connected = Resolver.Device.NetworkAdapters.Any(a => a.IsConnected);

        if (!connected)
        {
            Resolver.Log.Trace("could not send health metric, connection unavailable.");
            return;
        }

        var service = Resolver.Services.Get<IMeadowCloudService>();
        var device = Resolver.Device;

        var ce = new CloudEvent()
        {
            Description = "device.health",
            EventId = 10,
            Measurements = new Dictionary<string, object>()
                {
                    { "health.cpu_temp_celsius", device.PlatformOS.GetCpuTemperature().Celsius },
                    { "health.memory_used", GC.GetTotalMemory(false) },
                    { "health.disk_space_used", device.PlatformOS.GetPrimaryDiskSpaceInUse().Bytes },
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

        await service!.SendEvent(ce);
    }

    private Task TimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        return Send();
    }
}