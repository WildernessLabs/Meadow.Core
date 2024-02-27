using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Hardware;
using System;
using System.Net;
using System.Net.NetworkInformation;
using Meadow.Networking;

namespace Meadow.Devices;

/// <summary>
/// Represents a F7 Ethernet network adapter
/// </summary>
internal unsafe class F7EthernetNetworkAdapter : NetworkAdapterBase, IWiredNetworkAdapter
{

    private readonly Esp32Coprocessor _esp32;
    private bool _isConnected = false;

    /// <summary>
    /// Creates an instance of a F7EthernetNetworkAdapter
    /// </summary>
    /// <param name="ni">The associated native interface</param>
    public F7EthernetNetworkAdapter(NetworkInterface ni)
        : base(ni)
    {
        if (ni.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
        {
            throw new ArgumentException();
        }
    }

    /// <inheritdoc/>
    public override string Name => "Ethernet";

    /// <summary>
    /// Creates an instance of a F7EthernetNetworkAdapter
    /// </summary>
    /// <param name="esp32">The ESP32 coprocessor</param>
    public F7EthernetNetworkAdapter(Esp32Coprocessor esp32)
        : base(NetworkInterfaceType.Ethernet)
    {
        _esp32 = esp32;
        _esp32.EthernetMessageReceived += (s, e) =>
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
    protected void InvokeEvent(EthernetFunction eventId, StatusCodes statusCode, byte[] payload)
    {
        Resolver.Log.Trace($"Ethernet InvokeEvent {eventId} returned {statusCode}");

        switch (eventId)
        {
            case EthernetFunction.NetworkConnectedEvent:
                Resolver.Log.Trace("Ethernet connected event triggered!");

                var args = new EthernetNetworkConnectionEventArgs(IPAddress.Loopback, IPAddress.Any, IPAddress.None);

                this.Refresh();
                RaiseNetworkConnected(args);

                _isConnected = true;
                break;
            case EthernetFunction.NetworkDisconnectedEvent:
                Resolver.Log.Trace("Ethernet disconnected event triggered!");

                RaiseNetworkDisconnected(null);
                
                _isConnected = false;
                break;
            default:
                Resolver.Log.Trace("Event type not found");
                break;
        }
    }

    /// <summary>
    /// Returns <c>true</c> if the adapter is connected, otherwise <c>false</c>
    /// </summary>
    public override bool IsConnected => _isConnected;
}
