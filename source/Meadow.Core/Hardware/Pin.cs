using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a physical pin on the Meadow device.
    /// </summary>
    public class Pin : PinBase
    {
        public Pin(string name, uint address) : base (name, address)
        {
        }
    }
}
