using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Hardware;
using System.Net;
using System.Net.NetworkInformation;
using static Meadow.Logging.Logger;

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
    /// Process the NtpTimeChanged event.
    /// </summary>
    protected void RaiseNtpTimeChangedEvent()
    {
        // the NtpClient should have been added to the Resolver, so pull it and raise an event
        var client = Resolver.Services.Get<INtpClient>();
        if (client is NtpClient ntp)
        {
            ntp.RaiseTimeChanged();
        }
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
                RaiseNetworkDisconnected(new NetworkDisconnectionEventArgs(NetworkDisconnectReason.Unspecified));
                break;
            case EthernetFunction.NtpUpdateEvent:
                RaiseNtpTimeChangedEvent();
                break;
            default:
                Resolver.Log.Trace($"Ethernet event type {eventId} not found", MessageGroup.Core);
                break;
        }
    }

    /// <summary>
    /// Returns <c>true</c> if the adapter is connected, otherwise <c>false</c>
    /// </summary>
    public bool IsConnected => _isConnected;
}
