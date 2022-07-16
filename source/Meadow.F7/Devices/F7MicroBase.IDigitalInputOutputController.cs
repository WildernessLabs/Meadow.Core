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
            return CreateDigitalInputPort(pin, InterruptMode.None, ResistorMode.Disabled, TimeSpan.Zero, TimeSpan.Zero);
        }

        public IDigitalInputPort CreateDigitalInputPort(
            IPin pin,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled)
        {
            return CreateDigitalInputPort(pin, interruptMode, resistorMode, TimeSpan.Zero, TimeSpan.Zero);
        }

        public IDigitalInputPort CreateDigitalInputPort(
            IPin pin,
            InterruptMode interruptMode,
            ResistorMode resistorMode,
            TimeSpan debounceDuration,    // 0 - 1000 msec in .1 increments
            TimeSpan glitchDuration       // 0 - 1000 msec in .1 increments
            )
        {
            return DigitalInputPort.From(pin, this.IoController, interruptMode, resistorMode, debounceDuration, glitchDuration);
        }
    }
}
