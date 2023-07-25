using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using static Meadow.IPlatformOS;

// TODO: Remove unnecessary namespaces
namespace Meadow.Devices
{

    /// <summary>
	/// This file holds the Cell specific methods, properties etc for the ICellNetwork interface.
	/// </summary>
    public partial class Esp32Coprocessor : NetworkAdapterBase, ICellNetworkAdapter
    {

        #region Methods

        /// <summary>
        /// Use the event data to work out which event to invoke and create any event args that will be consumed.
        /// </summary>
        /// <param name="eventId">Event ID.</param>
        /// <param name="statusCode">Status of the event.</param>
        /// <param name="payload">Optional payload containing data specific to the result of the event.</param>
        protected void InvokeEvent(CellFunction eventId, StatusCodes statusCode, byte[] payload)
        {
            Resolver.Log.Trace($"Cell InvokeEvent {eventId} returned {statusCode}");

            // TODO: look for errors first?

            // TODO: Convert WriteLine logs to the appropriated form
            switch (eventId)
            {
                case CellFunction.NetworkConnectedEvent:
                    // Sample IPAddress values for the object creation
                    var ipAddress = IPAddress.Parse("192.168.1.100");
                    var subnet = IPAddress.Parse("255.255.255.0");
                    var gateway = IPAddress.Parse("192.168.1.1");

                    // Create an object using the CellNetworkConnectionEventArgs constructor
                    var args = new CellNetworkConnectionEventArgs(ipAddress, subnet, gateway);
                        
                    RaiseNetworkConnected(args);
                    break;
                case CellFunction.NetworkDisconnectedEvent:
                    Console.WriteLine("Cell disconnected event triggered!");

                    RaiseNetworkDisconnected();
                    break;
                default:
                    Console.WriteLine("Event type not found");
                    break;
            }
        }
        
        #endregion

    }
}
