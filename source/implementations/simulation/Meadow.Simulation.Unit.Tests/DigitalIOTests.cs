using Meadow.Hardware;
using Xunit;

namespace Meadow.Simulation.Unit.Tests;

public class ConfigurationTests
{
    [Fact]
    public void LoadAppConfigPositive()
    {
        var sim = new SimulatedMeadow<SimulatedPinout>();
    }
}

public class DigitalIOTests
{
    [Fact]
    public void TestBiDirectional()
    {
        var sim = new SimulatedMeadow<SimulatedPinout>();

        foreach (var pin in sim.Pins)
        {
            if (pin.Supports<IDigitalChannelInfo>(p => p.OutputCapable && p.InputCapable))
            {
                using (var port = sim.CreateBiDirectionalPort(pin))
                {
                    var state = true;
                    port.Direction = PortDirectionType.Output;

                    for (var i = 0; i < 10; i++)
                    {
                        ((IDigitalOutputPort)port).State = state;
                        var read = sim.ReadPinState(pin);

                        Assert.Equal(((IDigitalInputPort)port).State, read);

                        state = !state;
                    }

                    port.Direction = PortDirectionType.Input;

                    for (var i = 0; i < 10; i++)
                    {
                        sim.DrivePinState(pin, state);

                        Assert.Equal(state, ((IDigitalInputPort)port).State);

                        state = !state;
                    }
                }
            }
        }
    }

    [Fact]
    public void TestOutputs()
    {
        var sim = new SimulatedMeadow<SimulatedPinout>();

        foreach (var pin in sim.Pins)
        {
            if (pin.Supports<IDigitalChannelInfo>(p => p.OutputCapable))
            {
                using (var port = sim.CreateDigitalOutputPort(pin))
                {
                    var state = true;

                    for (var i = 0; i < 10; i++)
                    {
                        port.State = state;
                        var read = sim.ReadPinState(pin);

                        Assert.Equal(port.State, read);

                        state = !state;
                    }
                }
            }
        }
    }

    [Fact]
    public void TestInputs()
    {
        var sim = new SimulatedMeadow<SimulatedPinout>();

        foreach (var pin in sim.Pins)
        {
            if (pin.Supports<IDigitalChannelInfo>(p => p.InputCapable))
            {
                using (var port = sim.CreateDigitalInputPort(pin))
                {
                    var state = true;

                    for (var i = 0; i < 10; i++)
                    {
                        sim.DrivePinState(pin, state);

                        Assert.Equal(state, port.State);

                        state = !state;
                    }
                }
            }
        }
    }

    [Fact]
    public void TestInterrupts()
    {
        var sim = new SimulatedMeadow<SimulatedPinout>();

        foreach (var pin in sim.Pins)
        {
            if (pin.Supports<IDigitalChannelInfo>(p => p.InputCapable))
            {
                var port = sim.CreateDigitalInterruptPort(pin, Hardware.InterruptMode.EdgeBoth);
                bool interruptReceived = false;
                port.Changed += (b, a) =>
                {
                    interruptReceived = true;
                };

                var state = true;

                for (var i = 0; i < 10; i++)
                {
                    Assert.False(interruptReceived);
                    sim.DrivePinState(pin, state);
                    Assert.True(interruptReceived);
                    interruptReceived = false;

                    state = !state;
                }
            }
        }
    }
}