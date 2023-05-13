using Meadow.Hardware;
using System;

namespace Meadow.Devices
{
    public abstract partial class F7MicroBase
    {
        public IDigitalOutputPort CreateDigitalOutputPort(
            IPin pin,
            bool initialState = false,
            OutputType initialOutputType = OutputType.PushPull)
        {
            return DigitalOutputPort.From(pin, this.IoController, initialState, initialOutputType);
        }

        public IDigitalInputPort CreateDigitalInputPort(
            IPin pin)
        {
            return CreateDigitalInputPort(pin, ResistorMode.Disabled);
        }

        public IDigitalInputPort CreateDigitalInputPort(
            IPin pin,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled)
        {
            return CreateDigitalInputPort(pin, resistorMode);
        }

        public IDigitalInputPort CreateDigitalInputPort(
            IPin pin,
            ResistorMode resistorMode
            )
        {
            return DigitalInputPort.From(pin, this.IoController, resistorMode);
        }

        public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
        {
            return DigitalInterruptPort.From(pin, this.IoController, interruptMode, resistorMode, debounceDuration, glitchDuration);
        }
    }
}
