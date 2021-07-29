using System;
using Meadow.Gateway.WiFi;
using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Gateways;
using Meadow.Gateways.Exceptions;

namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Data relating to a stop WiFi interface request.
    /// </summary>
    public class WiFiInterfaceStoppedEventArgs : EventArgs
    {
        /// <summary>
        /// Status code of the stop WiFi interface request.
        /// </summary>
        public StatusCodes StatusCode { get; private set; }

        /// <summary>
        /// Date and time the event was generated.
        /// </summary>
        public DateTime When { get; private set; }

        /// <summary>
        /// Construct a WiFiInterfaceStoppedEventArgs object.
        /// </summary>
        /// <param name="statusCode">Status code of the </param>
        public WiFiInterfaceStoppedEventArgs(StatusCodes statusCode)
        {
            StatusCode = statusCode;
            When = DateTime.Now;
        }
    }
}
