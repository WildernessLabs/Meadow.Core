using System;
using Meadow.Devices.Esp32.MessagePayloads;

namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Data relating to a stop WiFi interface request.
    /// </summary>
    public class NtpTimeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Date and time the event was generated.
        /// </summary>
        public DateTime When { get; private set; }

        /// <summary>
        /// Construct a WiFiInterfaceStoppedEventArgs object.
        /// </summary>
        public NtpTimeChangedEventArgs()
        {
            When = DateTime.Now;
        }
    }
}
