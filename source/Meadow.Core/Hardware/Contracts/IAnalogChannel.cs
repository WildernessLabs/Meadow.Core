using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for analog communications channels.
    /// </summary>
    public interface IAnalogChannel
    {
        byte Precision { get; }
    }
}
