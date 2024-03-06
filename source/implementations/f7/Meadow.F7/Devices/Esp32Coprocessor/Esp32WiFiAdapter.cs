using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using static Meadow.IPlatformOS;

namespace Meadow.Devices;


/// <summary>
/// This file holds the WiFi specific methods, properties etc for the IWiFiAdapter interface.
/// </summary>
internal class Esp32WiFiAdapter : NetworkAdapterBase, IWiFiNetworkAdapter
{
    /// <summary>
    /// Default delay between WiFi network scans <see cref="ScanPeriod"/>.
    /// </summary>
    public static TimeSpan DefaultScanPeriod = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Minimum delay that can be used for the <see cref="ScanPeriod"/>.
    /// </summary>
    public static TimeSpan MinimumScanPeriod = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Maximum delay that can be used for the <see cref="ScanPeriod"/>.
    /// </summary>
    public static TimeSpan MaximumScanPeriod = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Lock object to make sure the events and the methods do not try to access
    /// properties simultaneously.
    /// </summary>
    private readonly object _lock = new object();
    private readonly Esp32Coprocessor _esp32;

    public Esp32WiFiAdapter(Esp32Coprocessor esp32)
        : base(NetworkInterfaceType.Wireless80211)
    {
        _esp32 = esp32;
        _esp32.WiFiMessageReceived += (s, e) =>
        {
            InvokeEvent(e.fn, e.status, e.data);
        };

        _mac = System.Net.NetworkInformation.PhysicalAddress.None;
        Bssid = System.Net.NetworkInformation.PhysicalAddress.None;
        _apMac = System.Net.NetworkInformation.PhysicalAddress.None;

        _isConnected = false;
        ClearNetworkDetails();
        _antenna = AntennaType.NotKnown;
    }

    /// <summary>
    /// Record if the WiFi ESP32 is connected to an access point.
    /// </summary>
    public override bool IsConnected { get => _isConnected; }

    /// <inheritdoc/>
    public override string Name => "ESP32 WiFi";

    /// <summary>
    /// Current onboard antenna in use.
    /// </summary>
    public AntennaType CurrentAntenna => _antenna;

    /// <summary>
    /// Private copy of the currently selected antenna.
    /// </summary>
    protected AntennaType _antenna;

    /// <summary>
    /// MAC address as used by the ESP32 when acting as a client.
    /// </summary>
    public override PhysicalAddress MacAddress
    {
        get
        {
            if ((_mac == null) || (_mac == System.Net.NetworkInformation.PhysicalAddress.None))
            {
                byte[] mac = new byte[6];
                F7PlatformOS.GetByteArray(IPlatformOS.ConfigurationValues.MacAddress, mac);
                _mac = new PhysicalAddress(mac);
            }
            return _mac;
        }
    }
    private PhysicalAddress _mac;

    /// <summary>
    /// MAC address as used by the ESP32 when acting as an access point.
    /// </summary>
    public PhysicalAddress ApMacAddress
    {
        get
        {
            if (_apMac == null)
            {
                byte[] mac = new byte[6];
                F7PlatformOS.GetByteArray(IPlatformOS.ConfigurationValues.SoftApMacAddress, mac);
                _apMac = new PhysicalAddress(mac);
            }
            return _apMac;
        }
    }
    private PhysicalAddress _apMac;

    /// <summary>
    /// Gets or sets whether to automatically start the network interface when the board reboots.
    /// </summary>
    /// <remarks>
    /// This will automatically connect to any preconfigured access points if they are available.
    /// </remarks>
    public bool AutoConnect
    {
        get => F7PlatformOS.GetBoolean(IPlatformOS.ConfigurationValues.AutomaticallyStartNetwork);
    }

    /// <summary>
    /// Automatically try to reconnect to an access point if there is a problem / disconnection?
    /// </summary>
    public bool AutoReconnect
    {
        get => F7PlatformOS.GetBoolean(IPlatformOS.ConfigurationValues.AutomaticallyReconnect);
    }

    /// <summary>
    /// Default access point to try to connect to if the network interface is started and the board
    /// is configured to automatically reconnect.
    /// </summary>
    public string DefaultSsid => F7PlatformOS.GetString(IPlatformOS.ConfigurationValues.DefaultAccessPoint);

    /// <summary>
    /// Access point the ESP32 is currently connected to.
    /// </summary>
    public string? Ssid { get; private set; }

    /// <summary>
    /// BSSID of the access point the ESP32 is currently connected to.
    /// </summary>
    public PhysicalAddress Bssid { get; private set; }

    /// <summary>
    /// WiFi channel the ESP32 and the access point are using for communication.
    /// </summary>
    public int Channel { get; private set; }

    /// <summary>
    /// IP address (for the selected network interface) specified in the configuration file.
    /// </summary>
    public uint ConfiguredIpAddress => F7PlatformOS.GetUInt(ConfigurationValues.StaticIpAddress);

    /// <summary>
    /// Subnet mask (for the selected network interface) specified in the configuration file.
    /// </summary>
    public uint ConfiguredSubnetMask => F7PlatformOS.GetUInt(ConfigurationValues.SubnetMask);

    /// <summary>
    /// Default gateway (for the selected network interface) specified in the configuration file.
    /// </summary>
    public uint ConfiguredGateway => F7PlatformOS.GetUInt(ConfigurationValues.DefaultGateway);

    /// <summary>
    /// The maximum number of times the ESP32 will retry an operation before returning an error.
    /// </summary>
    /// <remarks>
    /// This property enforces a minimum value of 3.
    /// </remarks>

    public uint MaximumRetryCount
    {
        get => F7PlatformOS.GetUInt(IPlatformOS.ConfigurationValues.MaximumNetworkRetryCount);

        set
        {
            uint retryCount = value;
            if (retryCount < 3)
            {
                retryCount = 3;
            }
            F7PlatformOS.SetUInt(IPlatformOS.ConfigurationValues.MaximumNetworkRetryCount, retryCount);
        }
    }

    /// <summary>
    /// Does the access point the WiFi adapter is currently connected to have internet access?
    /// </summary>
    public bool HasInternetAccess => CurrentState == NetworkState.Connected; // not sure this is true - this just means we're connected

    #region Methods

    /// <summary>
    /// Delay (in milliseconds) between network scans.
    /// </summary>
    /// <remarks>
    /// This will default to the <see cref="DefaultScanPeriod"/> value.
    ///
    /// The ScanFrequency should be between <see cref="MinimumScanPeriod"/> and
    /// <see cref="MaximumScanPeriod"/> (inclusive).
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Exception is thrown if value is less than <see cref="MinimumScanPeriod"/> or greater than <see cref="MaximumScanPeriod"/>.</exception>
    public TimeSpan ScanPeriod
    {
        get { return _scanPeriod; }
        set
        {
            if ((value < MinimumScanPeriod) || (value > MaximumScanPeriod))
            {
                throw new ArgumentOutOfRangeException(nameof(ScanPeriod), ScanPeriod, $"must be between {MinimumScanPeriod} and {MaximumScanPeriod} (inclusive).");
            }
            _scanPeriod = value;
        }
    }
    private TimeSpan _scanPeriod = DefaultScanPeriod;
    private bool _isConnected;

    /// <summary>
    /// Use the event data to work out which event to invoke and create any event args that will be consumed.
    /// </summary>
    /// <param name="eventId">Event ID.</param>
    /// <param name="statusCode">Status of the event.</param>
    /// <param name="payload">Optional payload containing data specific to the result of the event.</param>
    protected void InvokeEvent(WiFiFunction eventId, StatusCodes statusCode, byte[] payload)
    {
        Resolver.Log.Trace($"Wifi InvokeEvent {eventId} returned {statusCode}");

        // look for errors first
        if (eventId == WiFiFunction.ErrorEvent)
        {
            //  Need to work out if we have to do anything here.
        }
        else
        {
            if (statusCode != StatusCodes.CompletedOk)
            {
                _lastStatus = statusCode;
                CurrentState = NetworkState.Error;
                Resolver.Log.Debug($"Wifi function {eventId} returned {statusCode}");
                return;
            }
        }

        switch (eventId)
        {
            case WiFiFunction.NetworkConnectedEvent:
                byte channel = 0;

                ConnectEventData connectEventData = Encoders.ExtractConnectEventData(payload, 0);
                lock (_lock)
                {
                    Ssid = connectEventData.Ssid;
                    Bssid = new PhysicalAddress(connectEventData.Bssid);
                    Channel = channel;
                    _isConnected = true;
                    _authenticationType = (NetworkAuthenticationType)connectEventData.AuthenticationMode;
                }

                // testing has revealed that just because we're connected, it doesn't mean we've propagated the IP to the network infrastructure
                var t = 3000;

                Task.Run(async () =>
                {
                    while (IpAddress.Equals(System.Net.IPAddress.Any) || IpAddress.Equals(System.Net.IPAddress.None))
                    {
                        await Task.Delay(500);
                        this.Refresh();
                        t -= 500;
                        if (t < 0) break;
                    }

                    CurrentState = NetworkState.Connected;
                });

                break;
            case WiFiFunction.NetworkDisconnectedEvent:
                RaiseWiFiDisconnected(statusCode, payload);
                CurrentState = NetworkState.Disconnected;
                break;
            case WiFiFunction.NtpUpdateEvent:
                RaiseNtpTimeChangedEvent();
                break;
            case WiFiFunction.ErrorEvent:
                Resolver.Log.Debug($"Wifi function {eventId} returned {statusCode}");
                RaiseErrorEvent(statusCode);
                break;
            default:
                throw new NotImplementedException($"WiFi event not implemented ({eventId}).");
        }
    }

    /// <summary>
    /// Clear the IP address, subnet mask and gateway details.
    /// </summary>
    private void ClearNetworkDetails()
    {
        lock (_lock)
        {
            byte[] addressBytes = new byte[4];
            Array.Clear(addressBytes, 0, addressBytes.Length);
            Ssid = string.Empty;
            Bssid = PhysicalAddress.None;
            Channel = 0;
        }
    }

    // TODO: Mark, this should be async. But i think it requires the `SendCommand()` method to be async.
    /// <summary>
    /// Scan for networks.
    /// </summary>
    /// <remarks>
    /// The network must be started before this method can be called.
    /// </remarks>
    public async Task<IList<WifiNetwork>> Scan(TimeSpan timeout)
    {
        var src = new CancellationTokenSource();
        return await Scan(timeout, src.Token);
    }

    /// <summary>
    /// Scan for WiFiNetworks (access points).
    /// </summary>
    /// <param name="token">Cancellation token for the connection attempt</param>
    /// <returns>List of WiFiNetwork objects.</returns>
    public async Task<IList<WifiNetwork>> Scan(CancellationToken token)
    {
        return await Scan(TimeSpan.Zero, token);
    }

    /// <summary>
    /// Scan for WiFiNetworks (access points).
    /// </summary>
    /// <param name="timeout">Length of time to run the scan for before the scan is declared a failure.</param>
    /// <param name="token">Cancellation token for the connection attempt</param>
    /// <returns>List of WiFiNetwork objects.</returns>
    private Task<IList<WifiNetwork>> Scan(TimeSpan timeout, CancellationToken token)
    {
        var networks = new List<WifiNetwork>();
        var resultBuffer = new byte[Esp32Coprocessor.MAXIMUM_SPI_BUFFER_LENGTH];
        var tasks = new List<Task>();

        var scanTask = Task.Run(() =>
          {
              token.ThrowIfCancellationRequested();

              try
              {
                  // note: this is synchronous, so all we can really do is wait for completion in the background and throw away the result
                  var result = _esp32.SendCommand((byte)Esp32Interfaces.WiFi, (UInt32)WiFiFunction.GetAccessPoints, true, resultBuffer);

                  token.ThrowIfCancellationRequested();

                  if (result == StatusCodes.CompletedOk)
                  {
                      var accessPointList = Encoders.ExtractAccessPointList(resultBuffer, 0);
                      var accessPoints = new AccessPoint[accessPointList.NumberOfAccessPoints];

                      if (accessPointList.NumberOfAccessPoints > 0)
                      {
                          int accessPointOffset = 0;
                          for (int count = 0; count < accessPointList.NumberOfAccessPoints; count++)
                          {
                              var accessPoint = Encoders.ExtractAccessPoint(accessPointList.AccessPoints, accessPointOffset);
                              accessPointOffset += Encoders.EncodedAccessPointBufferSize(accessPoint);
                              var bssid = new PhysicalAddress(accessPoint.Bssid);
                              var network = new WifiNetwork(accessPoint.Ssid, bssid, NetworkType.Infrastructure, PhyType.Unknown,
                                  new NetworkSecuritySettings((NetworkAuthenticationType)accessPoint.AuthenticationMode, NetworkEncryptionType.Unknown),
                                  accessPoint.PrimaryChannel, (NetworkProtocol)accessPoint.Protocols, accessPoint.Rssi);
                              networks.Add(network);
                          }
                      }
                  }
                  else
                  {
                      Resolver.Log.Error($"Error getting access points: {result}");
                  }
                  return networks;
              }
              catch (Exception ex)
              {
                  Resolver.Log.Error($"Error getting access points: {ex.Message}");

                  token.ThrowIfCancellationRequested();
                  throw ex;
              }
          }, token);

        tasks.Add(scanTask);

        if (timeout.TotalMilliseconds > 0)
        {
            tasks.Add(Task.Delay(timeout));
        }

        var index = Task.WaitAny(tasks.ToArray());
        if (index == 1)
        {
            throw new TimeoutException();
        }

        return Task.FromResult(scanTask.Result as IList<WifiNetwork>);
    }

    /// <summary>
    /// Connect to a WiFi network.
    /// </summary>
    /// <param name="ssid">SSID of the network to connect to</param>
    /// <param name="password">Password for the WiFi access point.</param>
    /// <param name="timeout">Timeout period for the connection attempt</param>
    /// <param name="token">Cancellation token for the connection attempt</param>
    /// <param name="reconnection">Determine if the adapter should automatically attempt to reconnect (see <see cref="ReconnectionType"/>) to the access point if it becomes disconnected for any reason.</param>
    /// <returns>The connection result</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="TimeoutException"></exception>
    /// <exception cref="NetworkException"></exception>
    /// <exception cref="NetworkNotFoundException"></exception>
    /// <exception cref="NetworkAuthenticationException"></exception>
    public async Task Connect(string ssid, string password, TimeSpan timeout, CancellationToken token, ReconnectionType reconnection = ReconnectionType.Automatic)
    {
        switch (CurrentState)
        {
            case NetworkState.Connecting:
                throw new Exception("Adapter already connecting");
            case NetworkState.Disconnecting:
                throw new Exception("Adapter is in the process of disconnecting");
        }

        switch (Resolver.Device.PlatformOS.SelectedNetwork)
        {
            case IPlatformOS.NetworkConnectionType.WiFi:
                // pass through
                break;
            default:
                throw new NotSupportedException($"Connect can only be called when the platform is configured to use the WiFi network adapter.  It is currently configured for {Resolver.Device.PlatformOS.SelectedNetwork}");
        }

        if (string.IsNullOrEmpty(ssid))
        {
            throw new ArgumentNullException($"Invalid SSID '{ssid}'");
        }
        if (password == null)
        {
            throw new ArgumentNullException($"{nameof(password)} cannot be null.");
        }

        CurrentState = NetworkState.Connecting;

        var connectTask = Task.Run(async () =>
        {
            token.ThrowIfCancellationRequested();

            AccessPointInformation request = new AccessPointInformation()
            {
                NetworkName = ssid,
                Password = password,
                IpAddress = ConfiguredIpAddress,
                Gateway = ConfiguredGateway,
                //
                //  Setting Gateway to 0 will allow an error event to be raised.
                //
                //Gateway = 0,
                SubnetMask = ConfiguredSubnetMask
            };
            byte[] encodedPayload = Encoders.EncodeAccessPointInformation(request);
            byte[] resultBuffer = new byte[Esp32Coprocessor.MAXIMUM_SPI_BUFFER_LENGTH];

            ClearNetworkDetails();

            try
            {
                Resolver.Log.Trace($"Sending command to connect");
                _lastStatus = _esp32.SendCommand((byte)Esp32Interfaces.WiFi, (UInt32)WiFiFunction.ConnectToAccessPoint, true, encodedPayload, resultBuffer);
                Resolver.Log.Trace($"SendingCommand returned: {_lastStatus}");

                NetworkException? ne = null;
                switch (_lastStatus)
                {
                    case StatusCodes.AccessPointNotFound:
                        ne = new NetworkNotFoundException($"Access point not found");
                        break;
                    case StatusCodes.AuthenticationFailed:
                        ne = new NetworkAuthenticationException($"Authentication Failed");
                        break;
                    case StatusCodes.CannotConnectToAccessPoint:
                        ne = new NetworkException("Network error", (int)_lastStatus);
                        break;
                }

                if (ne != null)
                {
                    CurrentState = NetworkState.Error;
                    throw new Exception($"Connection failed: {_lastStatus}");
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Error connecting to access point: {ex.Message}");

                token.ThrowIfCancellationRequested();
                throw new NetworkException(ex.Message);
            }

            token.ThrowIfCancellationRequested();

            var t = 0;

            // wait for a state transition
            while (CurrentState == NetworkState.Connecting)
            {
                await Task.Delay(500);
                t += 500;
                if ((timeout.TotalMilliseconds > 0) && (t > timeout.TotalMilliseconds))
                {
                    throw new TimeoutException();
                }
            }

            if (CurrentState == NetworkState.Connected)
            {
                // testing has revealed that just because we're connected, it doesn't mean we've propagated the IP to the network infrastructure
                while (IpAddress.Equals(System.Net.IPAddress.Any))
                {
                    await Task.Delay(500);
                    this.Refresh();
                    t += 500;
                    if ((timeout.TotalMilliseconds > 0) && (t > timeout.TotalMilliseconds))
                    {
                        throw new TimeoutException();
                    }
                }
            }
        }, token);

        await connectTask;

        if (CurrentState == NetworkState.Error)
        {
            throw new Exception($"Connection error: {_lastStatus}");
        }
    }

    /// <summary>
    /// Disconnect from the current access point.
    /// </summary>
    /// <param name="turnOffWiFiInterface">Stop the WiFi interface.</param>
    /// <returns></returns>
    public async Task Disconnect(bool turnOffWiFiInterface)
    {
        switch (CurrentState)
        {
            case NetworkState.Connected:
                break;
            default:
                // do nothing if we're not connected
                return;
        }

        switch (Resolver.Device.PlatformOS.SelectedNetwork)
        {
            case IPlatformOS.NetworkConnectionType.WiFi:
                // pass through
                break;
            default:
                throw new NotSupportedException($"Disconnect can only be called when the platform is configured to use the WiFi network adapter.  It is currently configured for {Resolver.Device.PlatformOS.SelectedNetwork}");
        }

        await Task.Run(async () =>
        {
            var request = new DisconnectFromAccessPointRequest()
            {
                TurnOffWiFiInterface = (byte)((turnOffWiFiInterface ? 1 : 0) & 0xff)
            };
            byte[] encodedRequest = Encoders.EncodeDisconnectFromAccessPointRequest(request);

            var result = _esp32.SendCommand((byte)Esp32Interfaces.WiFi, (UInt32)WiFiFunction.DisconnectFromAccessPoint, true, encodedRequest, null);

            // NOTE: 'result' here is only the result of the ioctl, *not* the result of the disconnection request

            var t = 0;

            // wait for a state transition
            while (CurrentState == NetworkState.Disconnecting)
            {
                await Task.Delay(500);
                t += 500;
                if (t > TimeSpan.FromSeconds(30).TotalMilliseconds)
                {
                    throw new TimeoutException();
                }
            }
        });
    }

    /// <summary>
    /// Connect to the default access point.
    /// </summary>
    /// <remarks>The access point credentials should be stored in the coprocessor memory.</remarks>
    public void ConnectToDefaultAccessPoint()
    {
        if (Resolver.Device.PlatformOS.SelectedNetwork != IPlatformOS.NetworkConnectionType.WiFi)
        {
            throw new NotSupportedException($"ConnectToDefaultAccessPoint can only be called when the platform is configured to use the WiFi network adapter.  It is currently configured for {Resolver.Device.PlatformOS.SelectedNetwork}");
        }

        if ((CurrentState == NetworkState.Connected) || (CurrentState == NetworkState.Connecting))
        {
            throw new InvalidOperationException($"Already connected or connecting to an access point, current state {CurrentState}");
        }

        CurrentState = NetworkState.Connecting;
        _esp32.SendCommand((byte)Esp32Interfaces.WiFi, (UInt32)WiFiFunction.ConnectToDefaultAccessPoint, false, null);

        Task.Run(async () =>
        {
            var t = 0;
            var timeout = MaximumRetryCount * 3500;

            while (CurrentState == NetworkState.Connecting)
            {
                await Task.Delay(3500);
                t += 3500;
                if ((t > timeout))
                {
                    CurrentState = NetworkState.Disconnected;
                }
            }
        });
    }

    /// <summary>
    /// Removed any stored access point information from the coprocessor memory.
    /// </summary>
    public Task ClearStoredAccessPointInformation()
    {
        return Task.Run(() =>
        {
            _esp32.SendCommand((byte)Esp32Interfaces.WiFi, (UInt32)WiFiFunction.ClearDefaultAccessPoint, true, null);
        });
    }

    /// <summary>
    /// Change the current WiFi antenna.
    /// </summary>
    /// <remarks>
    /// Allows the application to change the current antenna used by the WiFi adapter.  This
    /// can be made to persist between reboots / power cycles by setting the persist option
    /// to true.
    /// </remarks>
    /// <param name="antenna">New antenna to use.</param>
    /// <param name="persist">Make the antenna change persistent.</param>
    public void SetAntenna(AntennaType antenna, bool persist = true)
    {
        if (antenna == AntennaType.NotKnown)
        {
            throw new ArgumentException("Setting the antenna type NotKnown is not allowed.");
        }

        SetAntennaRequest request = new SetAntennaRequest();
        if (persist)
        {
            request.Persist = 1;
        }
        else
        {
            request.Persist = 0;
        }
        if (antenna == AntennaType.OnBoard)
        {
            request.Antenna = (byte)AntennaTypes.OnBoard;
        }
        else
        {
            request.Antenna = (byte)AntennaTypes.External;
        }
        byte[] encodedPayload = Encoders.EncodeSetAntennaRequest(request);
        byte[] encodedResult = new byte[4000];
        StatusCodes result = _esp32.SendCommand((byte)Esp32Interfaces.WiFi, (UInt32)WiFiFunction.SetAntenna, true, encodedPayload, encodedResult);
        if (result == StatusCodes.CompletedOk)
        {
            _antenna = antenna;
        }
        else
        {
            throw new Exception("Failed to change the antenna in use.");
        }
    }

    #endregion Methods

    /// <summary>
    /// Process the Disconnected event extracting any event data from the
    /// payload and create an EventArg object if necessary
    /// </summary>
    /// <param name="statusCode">Status code for the WiFi disconnection request.</param>
    /// <param name="payload">Event data encoded in the payload.</param>
    protected void RaiseWiFiDisconnected(StatusCodes statusCode, byte[] payload)
    {
        ClearNetworkDetails();
        DisconnectEventData disconnectEventData = Encoders.ExtractDisconnectEventData(payload, 0);
        string reason = DisconnectReason(disconnectEventData);
        lock (_lock)
        {
            RaiseNetworkDisconnected(new NetworkDisconnectionEventArgs(reason));
        }
    }

    private string DisconnectReason(DisconnectEventData eventData)
    {
       WiFiDisconnectedReason reason = (WiFiDisconnectedReason) eventData.Reason;

        return reason switch
        {
            WiFiDisconnectedReason.Unspecified => "Internal Failed",
            WiFiDisconnectedReason.AssociatedExpire => "Disconnect due inactivity",
            WiFiDisconnectedReason.AssociatedTooMany => "Too many devices already connect to the Access Point",
            WiFiDisconnectedReason.AssociatedLeave => "Disconnected manually by the user",
            WiFiDisconnectedReason.FourWayHandshakeTimenout => "Wrong password. Please check your WiFi credentials",
            WiFiDisconnectedReason.BandWidtNotEnogh => "Low signal quality from the Access Point",
            WiFiDisconnectedReason.BeaconTimeout => "Close connection from Access Point",
            WiFiDisconnectedReason.AccessPointNotFound=> "Access Point not found. Please check your WiFi credentials or may be out of range of the access point",
            _  => $"Undefined Reason ({reason})",
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
    /// Process the Error event.
    /// </summary>
    protected void RaiseErrorEvent(StatusCodes statusCode)
    {
        RaiseNetworkError(new NetworkErrorEventArgs((uint)statusCode));
    }

    private NetworkState _state;
    private NetworkAuthenticationType _authenticationType;
    private StatusCodes _lastStatus;

    private NetworkState CurrentState
    {
        get => _state;
        set
        {
            if (value == _state) return;

            _state = value;

            switch (CurrentState)
            {
                case NetworkState.Connecting:
                    break;
                case NetworkState.Connected:
                    Refresh();
                    var args = new WirelessNetworkConnectionEventArgs(IpAddress, SubnetMask, Gateway, Ssid, Bssid, (byte)Channel, _authenticationType);
                    RaiseNetworkConnected(args);
                    break;
                case NetworkState.Disconnecting:
                    break;
                case NetworkState.Disconnected:
                    _isConnected = false;
                    break;
                case NetworkState.Error:
                    break;
            }
        }
    }

    private enum NetworkState
    {
        Unknown,
        Disconnected,
        Connecting,
        Connected,
        Disconnecting,
        Error
    }
}