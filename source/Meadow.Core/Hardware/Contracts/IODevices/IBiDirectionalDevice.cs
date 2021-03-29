using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for devices that expose `IBiDirectionPort(s)`.
    /// </summary>
    public interface IBiDirectionalDevice
    {
        /// <summary>
        /// Creates an `IBiDirectionPort` on the specified pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="initialState"></param>
        /// <param name="interruptMode"></param>
        /// <param name="resistorMode"></param>
        /// <param name="initialDirection"></param>
        /// <param name="debounceDuration"></param>
        /// <param name="glitchDuration"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        IBiDirectionalPort CreateBiDirectionalPort(
            IPin pin,
            bool initialState = false,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input,
            double debounceDuration = 0,
            double glitchDuration = 0,
            OutputType output = OutputType.PushPull
        );

    }
}
