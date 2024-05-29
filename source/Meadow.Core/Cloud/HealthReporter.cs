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
    private Dictionary<string, Func<object>> _customMetrics = new Dictionary<string, Func<object>>();
    private Dictionary<string, Func<Task<object>>> _customMetricsAsync = new Dictionary<string, Func<Task<object>>>();

    /// <inheritdoc/>
    public async Task Start(int interval)
    {
        Resolver.Log.Info($"Health Metrics enabled with interval: {interval} minute(s).");

        if (interval < 0)
        {
            throw new ArgumentException("HealthReporter interval must be a non-negative integer.");
        }

        if (interval == 0)
        {
            // do not set the timer. metrics can be sent manually.
            return;
        }

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
    public bool AddMetric(string name, Func<object> func)
    {
        return _customMetrics.TryAdd(name, func);
    }

    /// <inheritdoc/>
    public bool AddMetric(string name, Func<Task<object>> func)
    {
        return _customMetricsAsync.TryAdd(name, func);
    }

    private (string name, Func<object> function)[] DefaultMetrics =
    {
        new ("health.cpu_temp_celsius", () => Resolver.Device.PlatformOS.GetCpuTemperature().Celsius),
        new ("health.memory_used", () => GC.GetTotalMemory(false)),
        new ("health.disk_space_used", () => Resolver.Device.PlatformOS.GetPrimaryDiskSpaceInUse().Bytes),
        new ("info.os_version", () => Resolver.Device.Information.OSVersion)
    };

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
            Measurements = new Dictionary<string, object>(),
            Timestamp = DateTimeOffset.UtcNow
        };

        foreach (var metric in DefaultMetrics)
        {
            try
            {
                var m = metric.function.Invoke();
                if (m != null)
                {
                    ce.Measurements.Add(metric.name, m);
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Warn($"Cannot collect metric {metric.name}: {ex.Message}");
            }
        }

        var batteryInfo = device.GetBatteryInfo();
        if (batteryInfo != null)
        {
            ce.Measurements.Add("health.battery_percentage", batteryInfo.StateOfCharge);
        }

        if (!string.IsNullOrEmpty(device.Information.CoprocessorOSVersion))
        {
            ce.Measurements.Add("info.coprocessor_os_version", device.Information.CoprocessorOSVersion);
        }

        foreach (var metric in _customMetrics)
        {
            try
            {
                ce.Measurements.TryAdd(metric.Key, metric.Value.Invoke());
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Error reading value for health metric '{metric.Key}': {ex.Message}");
            }
        }

        foreach (var metric in _customMetricsAsync)
        {
            try
            {
                ce.Measurements.TryAdd(metric.Key, await metric.Value.Invoke());
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Error reading value for health metric '{metric.Key}': {ex.Message}");
            }
        }

        try
        {
            await service!.SendEvent(ce);
        }
        catch (MeadowCloudException ex)
        {
            Resolver.Log.Error($"sending health metrics failed: {ex.Message}");
        }
    }

    private Task TimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        return Send();
    }
}