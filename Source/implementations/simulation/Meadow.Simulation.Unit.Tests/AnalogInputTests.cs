using Meadow.Hardware;
using Meadow.Units;
using System;
using Xunit;

namespace Meadow.Simulation.Unit.Tests;

public class SimulatedExpanderTests
{
    [Fact]
    public void TestAnalogs()
    {
        var pinCount = 3;

        var sim = new SimulatedIOExpander(pinCount);

        for (var i = 0; i < pinCount; i++)
        {
            var pin = sim.GetPin(i);
            Assert.Equal($"PIN{i}", pin.Name);

            var port = pin.CreateAnalogInputPort(1) as SimulatedAnalogInputPort;
            Assert.NotNull(port);
            var testVoltage = i * 0.5;
            port.Voltage = new Voltage(testVoltage);
            Assert.Equal(testVoltage, port.Voltage.Volts);
        }
    }

    [Fact]
    public void TestDigitalInputs()
    {
        var pinCount = 3;

        var sim = new SimulatedIOExpander(pinCount);

        for (var i = 0; i < pinCount; i++)
        {
            var pin = sim.GetPin(i);
            Assert.Equal($"PIN{i}", pin.Name);

            var port = pin.CreateDigitalInputPort() as SimulatedDigitalInputPort;
            Assert.NotNull(port);
            port.State = false;
            Assert.False(port.State);
            port.State = true;
            Assert.True(port.State);
            port.State = false;
            Assert.False(port.State);
        }
    }

    [Fact]
    public void TestDigitalOutputs()
    {
        var pinCount = 3;

        var sim = new SimulatedIOExpander(pinCount);

        for (var i = 0; i < pinCount; i++)
        {
            var pin = sim.GetPin(i);
            Assert.Equal($"PIN{i}", pin.Name);

            var port = pin.CreateDigitalOutputPort() as SimulatedDigitalOutputPort;
            Assert.NotNull(port);
            port.State = false;
            Assert.False(port.State);
            port.State = true;
            Assert.True(port.State);
            port.State = false;
            Assert.False(port.State);
        }
    }
}

public class AnalogInputTests
{
    [Fact]
    public async void TestInputs()
    {
        var sim = new SimulatedMeadow<SimulatedPinout>();

        foreach (var pin in sim.Pins)
        {
            if (pin.Supports<IAnalogChannelInfo>())
            {
                using (var port = sim.CreateAnalogInputPort(pin, 1, TimeSpan.Zero, new Voltage(3.3f)))
                {
                    var state = true;

                    for (var i = 0; i < 10; i++)
                    {
                        var driven = new Voltage(i * 0.5);
                        sim.DrivePinVoltage(pin, driven);
                        var read = await port.Read();

                        Assert.Equal(driven, read);

                        state = !state;
                    }
                }
            }
        }
    }
}