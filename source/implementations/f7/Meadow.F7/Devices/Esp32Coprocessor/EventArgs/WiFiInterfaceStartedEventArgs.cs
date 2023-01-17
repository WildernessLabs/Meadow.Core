using System;
using Meadow.Gateway.WiFi;
using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Gateways;
using Meadow.Gateways.Exceptions;

namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Data related to a start WiFi interface request.
    /// </summary>
    public class WiFiInterfaceStartedEventArgs : EventArgs
    {
        /// <summary>
        /// Status code of the start WiFi interface request.
        /// </summary>
        public StatusCodes StatusCode { get; private set; }

        /// <summary>
        /// Date and time the event was generated.
        /// </summary>
        public DateTime When { get; private set; }

        /// <summary>
        /// Construct a WiFiInterfaceStartedEventArgs object.
        /// </summary>
        /// <param name="statusCode">Status of the WiFi start interface start request.</param>
        public WiFiInterfaceStartedEventArgs(StatusCodes statusCode)
        {
            StatusCode = statusCode;
            When = DateTime.Now;
        }
    }
}
