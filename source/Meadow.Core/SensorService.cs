using Meadow.Peripherals.Sensors;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

internal class ThreadedPollingSensorMonitor : ISensorMonitor
{
    public EventHandler<object> SampleAvailable = default!;

    private readonly List<(ISensor sensor, MethodInfo readMethod)> _monitorList = new();
    private Thread? _sampleThread;
    private bool _stopRequested = false;
    private PropertyInfo? _resultProperty;
    private List<ISensor> _sensorList = new();
    private long _tick;


    public ThreadedPollingSensorMonitor()
    {
    }

    public void AddSensor(ISensor sensor, TimeSpan updateInterval)
    {
        // find the Read method, if it exists
        // this is just a limitation of C# generics - looking for a better way
        var readMethod = sensor.GetType().GetMethod("Read", BindingFlags.Instance | BindingFlags.Public);

        if (readMethod != null)
        {
            _monitorList.Add((sensor, readMethod));
        }
    }

    public void StartSampling(ISensor sensor)
    {
        if (_sampleThread == null)
        {
            _sampleThread = new Thread(SamplingThreadProc);
            _sampleThread.Start();
        }

        _sensorList.Add(sensor);
    }

    private async void SamplingThreadProc()
    {
        while (!_stopRequested)
        {
            foreach (var monitor in _monitorList)
            {
                if (_sensorList.Contains(monitor.sensor))
                {
                    // TODO: check to see if the sensor should be read

                    var task = (Task)monitor.readMethod.Invoke(monitor.sensor, null);
                    await task.ConfigureAwait(false);

                    if (_resultProperty == null)
                    {
                        _resultProperty = task.GetType().GetProperty("Result");
                    }
                    SampleAvailable?.Invoke(monitor.sensor, _resultProperty.GetValue(task));

                    _tick++;
                }
            }

            Thread.Sleep(1000);
        }

        _stopRequested = false;
        _sampleThread = null;
    }

    public void StopSampling(ISensor sensor)
    {
        _sensorList.Remove(sensor);
    }
}

internal class SensorService : ISensorService
{
    private ThreadedPollingSensorMonitor? _pollMonitor;

    internal SensorService()
    {
    }

    public void RegisterSensor(ISamplingSensor sensor)
    {
        if (_pollMonitor == null)
        {
            _pollMonitor = new ThreadedPollingSensorMonitor();
        }

        if (sensor is ISensor s)
        {
            _pollMonitor.StartSampling(s);
        }
    }
}
