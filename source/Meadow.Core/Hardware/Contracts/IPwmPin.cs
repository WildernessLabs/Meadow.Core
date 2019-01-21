using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for a pin on the Meadow device that can be configured as a
    /// Pulse-Width-Modulation (PWM) port.
    /// </summary>
	public interface IPwmPin : IPin, IPwmChannelInfo
    {
    }
}
