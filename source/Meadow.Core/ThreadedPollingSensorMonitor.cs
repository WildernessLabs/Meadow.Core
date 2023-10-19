using Meadow.Peripherals.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

internal class ThreadedPollingSensorMonitor : ISensorMonitor
{
    public event EventHandler<object> SampleAvailable = default!;

    private readonly List<SensorMeta> _monitorList = new();
    private Thread _sampleThread;
    private bool _stopRequested = false;

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
        _sampleThread = new Thread(SamplingThreadProc);
        _sampleThread.Start();
    }

    public void StartSampling(ISamplingSensor sensor)
    {
        var existing = _monitorList.FirstOrDefault(s => s.Sensor.Equals(sensor));

        if (existing == null)
        {
            // find the Read method, if it exists
            // this is just a limitation of C# generics - looking for a better way
            var readMethod = sensor.GetType().GetMethod("Read", BindingFlags.Instance | BindingFlags.Public);

            if (readMethod != null)
            {
                var meta = new SensorMeta(sensor, readMethod)
                {
                    EnableReading = true,
                    NextReading = sensor.UpdateInterval
                };

                _monitorList.Add(meta);
            }
        }
        else
        {
            existing.NextReading = sensor.UpdateInterval;
            existing.EnableReading = true;
        }
    }

    private async void SamplingThreadProc()
    {
        while (!_stopRequested)
        {
            foreach (var monitor in _monitorList)
            {
                // skip "disabled" (i.e. StopUpdating has been called) sensors
                if (!monitor.EnableReading) continue;

                // check for the reading due tiime (1s granularity for now)
                var remaining = monitor.NextReading.Subtract(TimeSpan.FromSeconds(1));

                if (remaining.TotalSeconds <= 0)
                {
                    // reset the next reading
                    monitor.NextReading = monitor.Sensor.UpdateInterval;

                    // read the sensor
                    try
                    {
                        var task = (Task)monitor.ReadMethod.Invoke(monitor.Sensor, null);
                        await task.ConfigureAwait(false);

                        if (monitor.ResultProperty == null)
                        {
                            monitor.ResultProperty = task.GetType().GetProperty("Result");
                        }

                        var value = monitor.ResultProperty.GetValue(task);

                        // raise an event - not ideal as all sensors get events for all other sensors
                        // fixing this requires eitehr exposing a "set" method, which I'd prefer be kept internal
                        // or using reflection to find a set method, which is fragile
                        SampleAvailable?.Invoke(monitor.Sensor, value);
                    }
                    catch (Exception ex)
                    {
                        Resolver.Log.Error($"Unhandled exception reading sensor type {monitor.Sensor.GetType().Name}: {ex.Message}");
                    }
                }
                else
                {
                    monitor.NextReading = remaining;
                }
            }

            Thread.Sleep(1000); // TODO: improve this algorithm
        }

        _stopRequested = false;
    }

    public void StopSampling(ISamplingSensor sensor)
    {
        var existing = _monitorList.FirstOrDefault(s => s.Sensor.Equals(sensor));

        if (existing != null)
        {
            existing.EnableReading = false;
        }
    }
}

