using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides data for serial data received events.
    /// </summary>
    public class SerialDataReceivedEventArgs
    {
        public SerialDataReceivedEventArgs(SerialDataType eventType)
        {
            EventType = eventType;
        }

        public SerialDataType EventType { get; }
    }
}
