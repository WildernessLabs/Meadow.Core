using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for analog communication channels.
    /// </summary>
    public interface IAnalogChannel : IChannel
    {
        byte Precision { get; }
    }
}
