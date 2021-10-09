using System;
using Meadow.Hardware;

namespace Meadow.Devices
{
    public abstract partial class F7MicroBase
    {
        public IBiDirectionalPort CreateBiDirectionalPort(
            IPin pin,
            bool initialState = false,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input,
            double debounceDuration = 0.0,    // 0 - 1000 msec in .1 increments
            double glitchDuration = 0.0,      // 0 - 1000 msec in .1 increments
            OutputType outputType = OutputType.PushPull
            )
        {
            // Convert durations to unsigned int with 100 usec resolution
            return BiDirectionalPort.From(pin, this.IoController, initialState, interruptMode, resistorMode, initialDirection, debounceDuration, glitchDuration, outputType);
        }
    }
}