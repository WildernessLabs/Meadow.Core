using Meadow.Peripherals.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

internal class ThreadedPollingSensorMonitor : ISensorMonitor, IDisposable
{
    public event EventHandler<object> SampleAvailable = default!;

    private readonly List<SensorMeta> _metaList = new();
    private readonly Task _sampleTask;
    private readonly Random _random = new();
    private readonly TimeSpan PollPeriod = TimeSpan.FromSeconds(1);

    private class SensorMeta
    {
        public SensorMeta(ISamplingSensor sensor, MethodInfo readmethod)
        {
            Sensor = sensor;
            ReadMethod = readmethod;
        }

        public ISamplingSensor Sensor { get; set; }
        public MethodInfo ReadMethod { get; set; }
        public PropertyInfo? ResultProperty { get; set; }
        public TimeSpan NextReading { get; set; }
        public bool EnableReading { get; set; }
    }

    public ThreadedPollingSensorMonitor()
    {
        _sampleTask = Task.Run(() => SamplingTaskProc(Resolver.App.CancellationToken), Resolver.App.CancellationToken);
    }

    public void StartSampling(ISamplingSensor sensor)
    {
        var existing = _metaList.FirstOrDefault(s => s.Sensor.Equals(sensor));

        if (existing == null)
        {
            // find the Read method, if it exists
            // this is just a limitation of C# generics - looking for a better way
            var readMethod = sensor.GetType().GetMethod("Read", BindingFlags.Instance | BindingFlags.Public);

            if (readMethod != null)
            {
                // wait a random period to attempt to spread the updates over time
                var firstInterval = _random.Next(0, (int)(sensor.UpdateInterval.TotalSeconds + 1));
                var meta = new SensorMeta(sensor, readMethod)
                {
                    EnableReading = true,
                    NextReading = TimeSpan.FromSeconds(firstInterval)
                };

                _metaList.Add(meta);
            }
        }
        else
        {
            existing.NextReading = sensor.UpdateInterval;
            existing.EnableReading = true;
        }
    }

    private async Task SamplingTaskProc(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var meta in _metaList)
            {
                // skip "disabled" (i.e. StopUpdating has been called) sensors
                if (!meta.EnableReading) continue;

                // check for the reading due tiime (1s granularity for now)
                var remaining = meta.NextReading.Subtract(TimeSpan.FromSeconds(1));

                if (remaining.TotalSeconds <= 0)
                {
                    // reset the next reading
                    meta.NextReading = meta.Sensor.UpdateInterval;

                    // read the sensor
                    try
                    {
                        var task = (Task)meta.ReadMethod.Invoke(meta.Sensor, null);
                        await task.ConfigureAwait(false);

                        if (meta.ResultProperty == null)
                        {
                            meta.ResultProperty = task.GetType().GetProperty("Result");
                        }

                        var value = meta.ResultProperty.GetValue(task);

                        // raise an event - not ideal as all sensors get events for all other sensors
                        // fixing this requires eitehr exposing a "set" method, which I'd prefer be kept internal
                        // or using reflection to find a set method, which is fragile
                        SampleAvailable?.Invoke(meta.Sensor, value);
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Error($"Unhandled exception reading sensor type {meta.Sensor.GetType().Name}: {ex.Message}");
                    }
                }
                else
                {
                    meta.NextReading = remaining;
                }
            }

            try
            {
                await Task.Delay(PollPeriod, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
        }
    }

    public void StopSampling(ISamplingSensor sensor)
    {
        var existing = _metaList.FirstOrDefault(s => s.Sensor.Equals(sensor));

        if (existing != null)
        {
            existing.EnableReading = false;
        }
    }

    /// <summary>
    /// Dispose of sensor monitoring resources.
    /// </summary>
    public void Dispose()
    {
        try
        {
            _sampleTask?.Wait(TimeSpan.FromSeconds(1));
        }
        catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
        {
            // Expected when canceling the task
        }
        _sampleTask?.Dispose();
    }
}

