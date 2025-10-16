using Meadow.Peripherals.Sensors;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace Core.Unit.Tests;

public abstract class PressureSensor : ISensor<Pressure>
{
    public Task<Pressure> Read()
    {
        throw new NotImplementedException();
    }
}

public abstract class TemperatureSensor : ISensor<Temperature>
{
    public Task<Temperature> Read()
    {
        throw new NotImplementedException();
    }
}

public class CurrentLoopSensor<T> where T : struct
{
    private Func<Task<Current>> _inputReadDelegate;
    public double Scale { get; }
    public double Offset { get; }

    public CurrentLoopSensor(Func<Task<Current>> inputReadDelegate, double scale, double offset)
    {
        _inputReadDelegate = inputReadDelegate;
        Scale = scale;
        Offset = offset;
    }

    public ValueTask<Current> ReadRaw()
    {
        var raw = _inputReadDelegate();
        return new ValueTask<Current>(raw);
    }

    protected T CreateUnit(double canonical)
    {
        return default(T);
    }

    public async ValueTask<T> Read()
    {
        throw new NotImplementedException();
        //var raw = await ReadRaw();
        //return (T)((raw.Amps * Scale) + Offset);
    }
}

public class ConfigurableAnalogInput
{
    public enum InputType
    {
        Current_4_20,
        Voltage_0_10,
        Temperature_Ntc
    }

    public Channel[] Channels { get; }

    public ConfigurableAnalogInput(InputType[] channelDefinitions)
    {
    }

    public abstract class Channel { }

    public class CurrentChannel : Channel
    {
        public Current Read()
        {
            return Current.Zero;
        }
    }
    public class VoltageChannel : Channel
    {
        public Voltage Read()
        {
            return Voltage.Zero;
        }
    }
    public class TemperatureChannel : Channel
    {
        public Temperature Read()
        {
            return Temperature.AbsoluteZero;
        }
    }
}

public class SensorServiceTests
{
}
