using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Gateways;
using Meadow.Gateway.WiFi;
using Meadow.Gateways.Exceptions;

namespace Meadow.Devices
{
    public partial class Esp32Coprocessor
    {
        /// <summary>
        /// Use the event data to work out which event to invoke and create any event args that will be consumed.
        /// </summary>
        /// <param name="eventId">Event ID.</param>
        /// <param name="statusCode">Status of the event.</param>
        /// <param name="payload">Optional payload containing data specific to the result of the event.</param>
        protected void InvokeEvent(BluetoothFunction eventId, StatusCodes statusCode, byte[] payload)
        {
            //  Placeholder
        }
    }
}
