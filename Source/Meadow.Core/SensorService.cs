using Meadow.Peripherals.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Meadow;

internal class SensorService : ISensorService
{
    private ThreadedPollingSensorMonitor? _pollMonitor;

    private List<ISensor> _sensors = new();

    internal SensorService()
    {
    }

    /// <inheritdoc/>
    public IEnumerable<TSensor> GetSensorsOfType<TSensor>()
        where TSensor : ISensor
    {
        foreach (var sensor in _sensors)
        {
            if (sensor is TSensor s)
            {
                yield return s;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ISensor> GetSensorsWithData<TUnit>()
        where TUnit : struct
    {
        foreach (var sensor in _sensors)
        {
            var typeToCheck = sensor.GetType();

            do
            {
                if (typeToCheck
                    .GetGenericArguments()
                    .SelectMany(a => a
                        .GetFields()
                        .Where(f => f.FieldType.Equals(typeof(TUnit)) || Nullable.GetUnderlyingType(f.FieldType).Equals(typeof(TUnit))))
                    .Any())
                {
                    yield return sensor;
                    break;
                }

                typeToCheck = typeToCheck.BaseType;
            } while (!typeToCheck.Equals(typeof(object)));
        }

        //        return list;
    }

    /// <inheritdoc/>
    public void RegisterSensor(ISensor sensor)
    {
        if (sensor is IPollingSensor s)
        {
            if (s.UpdateInterval.TotalSeconds >= 1)
            {
                if (_pollMonitor == null)
                {
                    _pollMonitor = new ThreadedPollingSensorMonitor();
                }

                // don't migrate fast-polling sensors to the threaded monitor
                s.SensorMonitor?.StopSampling(s);

                s.StopUpdating();

                s.SensorMonitor = _pollMonitor;

                _pollMonitor.StartSampling(s);
            }
        }

        if (sensor is ISleepAwarePeripheral sp)
        {
            Resolver.Device.PlatformOS.RegisterForSleep(sp);
        }

        lock (_sensors)
        {
            if (!_sensors.Contains(sensor))
            {
                _sensors.Add(sensor);
            }
        }
    }
}

