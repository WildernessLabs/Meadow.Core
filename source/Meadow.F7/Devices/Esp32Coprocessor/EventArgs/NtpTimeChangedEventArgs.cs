using System;

namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Data relating to NTP time changed event.
    /// </summary>
    public class NtpTimeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Date and time the event was generated.
        /// </summary>
        public DateTime When { get; private set; }

        /// <summary>
        /// Construct a NtpTimeChangedEventArgs object.
        /// </summary>
        public NtpTimeChangedEventArgs()
        {
            When = DateTime.Now;
        }
    }
}
