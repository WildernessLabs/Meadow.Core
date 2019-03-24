using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides data for events that come from an IDigitalInputPort.
    /// </summary>
    public class DigitalInputPortEventArgs : EventArgs
    {
        public bool Value { get; set; }
        public DateTime Time { get; set; }
    }
}