using System;
using Meadow.Gateway.WiFi;
using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Gateways;
using Meadow.Gateways.Exceptions;

namespace Meadow
{
    public class WiFiInterfaceStoppedEventArgs : EventArgs
    {
        public StatusCodes StatusCode { get; private set; }

        public DateTime When { get; private set; }

        public WiFiInterfaceStoppedEventArgs(StatusCodes statusCode)
        {
            StatusCode = statusCode;
            When = DateTime.Now;
        }
    }
}
