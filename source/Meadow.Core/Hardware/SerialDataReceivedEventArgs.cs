using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides data for serial data received events.
    /// </summary>
    public class SerialDataReceivedEventArgs
    {
        internal SerialDataReceivedEventArgs(SerialDataType eventType)
        {
            EventType = eventType;
        }

        public SerialDataType EventType { get; }
    }
}
