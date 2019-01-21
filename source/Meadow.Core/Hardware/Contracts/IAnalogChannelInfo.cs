using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for analog communication channels.
    /// </summary>
    public interface IAnalogChannelInfo
    {
        byte Precision { get; }
    }
}
