using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a channel on the device such as a pin.
    /// </summary>
    public class Pin : PinBase
    {
        public Pin(string name, uint address) : base (name, address)
        {
        }
    }
}
