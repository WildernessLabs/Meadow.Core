using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Hardware;
using System.Net;
using System.Net.NetworkInformation;

namespace Meadow.Devices;

/// <summary>
/// Represents a F7 Ethernet network adapter
/// </summary>
internal unsafe class F7EthernetNetworkAdapter : NetworkAdapterBase, IWiredNetworkAdapter
{

    private readonly Esp32Coprocessor _esp32;
    private bool _isConnected = false;


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
        switch (eventId)
        {
            case EthernetFunction.NetworkConnectedEvent:
                var args = new EthernetNetworkConnectionEventArgs(IPAddress.Loopback, IPAddress.Any, IPAddress.None);

                this.Refresh();
                _isConnected = true;

                RaiseNetworkConnected(args);

                break;
            case EthernetFunction.NetworkDisconnectedEvent:
                _isConnected = false;
                RaiseNetworkDisconnected(null);
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
