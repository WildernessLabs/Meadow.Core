using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Hardware;
using System.Net;
using System.Net.NetworkInformation;
using static Meadow.Logging.Logger;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Devices;

/// <summary>
/// Represents a F7 Ethernet network adapter
/// </summary>
internal unsafe class F7EthernetNetworkAdapter : NetworkAdapterBase, IWiredNetworkAdapter
{

    private readonly Esp32Coprocessor _esp32;
    private NetworkState _state;

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
        _state = NetworkState.Unknown;
        EthernetStartup();
    }

    private Task EthernetStartup()
    {
        return Task.Run(() =>
        {
            //((byte)Esp32Interfaces.WiredEthernet, (UInt32)EthernetFunction.StartEthernetInterface, false, null);
            //Resolver.Log.Info($"Sending command result = {result}");
            _esp32.SendIncommingEvent((byte)Esp32Interfaces.WiredEthernet, (UInt32)EthernetFunction.StartEthernetInterface);

            var timeout = 0;
            while(_state == NetworkState.Unknown)
            {
                Thread.Sleep(500);
                timeout += 500;
                if (timeout > TimeSpan.FromSeconds(10).TotalMilliseconds)
                {
                    // throw new TimeoutException();
                    Resolver.Log.Info("TimeoutException");
                }
            }
        });
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
               _state = NetworkState.Connected;

                RaiseNetworkConnected(args);

                break;
            case EthernetFunction.NetworkDisconnectedEvent:
                _state = NetworkState.Connected;
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
    public override bool IsConnected { get => _state == NetworkState.Connected; }
}
