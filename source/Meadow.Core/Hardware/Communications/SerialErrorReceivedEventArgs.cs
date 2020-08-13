using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides data from serial communication errors.
    /// </summary>
    public class SerialErrorReceivedEventArgs
    {
        public SerialErrorType EventType { get; }
    }
}
