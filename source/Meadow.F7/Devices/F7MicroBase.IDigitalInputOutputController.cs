using System;
using Meadow.Hardware;

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
            IPin pin,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            double debounceDuration = 0.0,    // 0 - 1000 msec in .1 increments
            double glitchDuration = 0.0       // 0 - 1000 msec in .1 increments
            )
        {
            return DigitalInputPort.From(pin, this.IoController, interruptMode, resistorMode, debounceDuration, glitchDuration);
        }
    }
}
