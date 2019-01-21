using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides data for events that come from an IPort.
    /// </summary>
    public class PortEventArgs : EventArgs
    {
        public bool Value { get; set; }
    }
}