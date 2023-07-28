using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Hardware;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Meadow.Devices;


/// <summary>
/// This file holds the Cell specific methods, properties etc for the ICellNetwork interface.
/// </summary>
unsafe internal class F7CellNetworkAdapter : NetworkAdapterBase, ICellNetworkAdapter
{
    private Esp32Coprocessor _esp32;

    public string GetPPPDOutputs
    {
        get
        {
            // allocate a 1k buffer.  It will automatically initialize to zeros
            var buffer = Marshal.AllocHGlobal(1024);

            try
            {
                var len = Core.Interop.Nuttx.meadow_get_cell_pppd_output(buffer);

                if (len > 0)
                {
                    var data = Encoding.UTF8.GetString((byte*)buffer.ToPointer(), len);

                    return data;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                // free resources. this will always run
                Marshal.FreeHGlobal(buffer);
            }
        }
    }
  

    public F7CellNetworkAdapter(Esp32Coprocessor esp32)
        : base(System.Net.NetworkInformation.NetworkInterfaceType.Ppp)
    {
        _esp32 = esp32;
        _esp32.CellMessageReceived += (s, e) =>
        {
            InvokeEvent(e.fn, e.status, e.data);
        };
    }

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

    /// <summary>
    /// Returns <b>true</b> if the adapter is connected, otherwise <b>false</b>
    /// </summary>
    public override bool IsConnected
    {
        get
        {
            try
            {
                return Core.Interop.Nuttx.meadow_cell_is_connected();
            }
            catch (Exception e)
            {
                Resolver.Log.Error($"Failed to read meadow_cell_is_connected(): {e.Message}");
                return false;
            }
        }
    }
}
