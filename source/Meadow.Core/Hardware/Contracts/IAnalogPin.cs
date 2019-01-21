using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for a pin on the Meadow board that is connected to an analog channel.
    /// </summary>
    public interface IAnalogPin : IPin, IAnalogChannel
    {
    }
}
