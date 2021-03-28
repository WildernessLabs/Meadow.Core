using System;
namespace Meadow.Hardware
{
    public interface IDigitalInputDevice
    {
        IDigitalInputPort CreateDigitalInputPort(
            IPin pin,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            double debounceDuration = 0,
            double glitchDuration = 0
            );

    }
}
