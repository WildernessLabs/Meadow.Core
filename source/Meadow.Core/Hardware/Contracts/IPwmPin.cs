using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for a pin on the Meadow board that is connected to an PWM channel.
    /// </summary>
	public interface IPwmPin : IPin, IPwmChannel
    {
    }
}
