using Meadow.Hardware;
using System;

namespace Meadow.Devices
{
    public abstract partial class F7MicroBase
    {
        public IBiDirectionalPort CreateBiDirectionalPort(
            IPin pin,
            bool initialState = false,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input
            )
        {
            return CreateBiDirectionalPort(pin, initialState, interruptMode, resistorMode, initialDirection, TimeSpan.Zero, TimeSpan.Zero, OutputType.PushPull);
        }

        public IBiDirectionalPort CreateBiDirectionalPort(
            IPin pin,
            bool initialState,
            InterruptMode interruptMode,
            ResistorMode resistorMode,
            PortDirectionType initialDirection,
            TimeSpan debounceDuration,
            TimeSpan glitchDuration,
            OutputType outputType
            )
        {
            // Convert durations to unsigned int with 100 usec resolution
            return BiDirectionalPort.From(pin, this.IoController, initialState, interruptMode, resistorMode, initialDirection, debounceDuration, glitchDuration, outputType);
        }
    }
}