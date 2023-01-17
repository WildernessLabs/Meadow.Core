using Meadow.Hardware;
using Meadow.Units;
using System;
using Xunit;

namespace Meadow.Simulation.Unit.Tests
{
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
}