using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for a pin that is connected to an analog channel.
    /// </summary>
    public interface IAnalogPin : IPin, IAnalogChannel
    {
    }
}
