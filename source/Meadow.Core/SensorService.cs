using Meadow.Peripherals.Sensors;

namespace Meadow;

internal class SensorService : ISensorService
{
    private ThreadedPollingSensorMonitor? _pollMonitor;

    internal SensorService()
    {
    }

    public void RegisterSensor(ISensor sensor)
    {
        if (_pollMonitor == null)
        {
            _pollMonitor = new ThreadedPollingSensorMonitor();
        }

        if (sensor is IPollingSensor s)
        {
            s.SensorMonitor?.StopSampling(s);

            s.StopUpdating();

            Resolver.Log.Info($"Replacing monitor on {s.GetType().Name}");

            s.SensorMonitor = _pollMonitor;

            _pollMonitor.StartSampling(s);
        }
    }
}

