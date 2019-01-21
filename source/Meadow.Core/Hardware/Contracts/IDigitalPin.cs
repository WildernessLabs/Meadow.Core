using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for a pin on the Meadow board that is connected to an digital channel.
    /// </summary>
    public interface IDigitalPin : IPin, IDigitalChannelInfo
    {
        // TODO: get this out of here. IGPIOManager is specific to our pins.
        IGPIOManager GPIOManager { get; }
    }
}
