using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Gateways;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Threading;
using static Meadow.Logging.Logger;

namespace Meadow.Devices;

/// <summary>
/// Represents a Meadow F7 micro device. Includes device-specific IO mapping,
/// capabilities and provides access to the various device-specific features.
/// </summary>
public abstract partial class F7MicroBase : IF7MeadowDevice
{
    private F7ReliabilityService _reliabilityService;

    /// <summary>
    /// Event raised when a new network is connected
    /// </summary>
    public event NetworkConnectionHandler NetworkConnected = default!;

    /// <summary>
    /// Event raised when an existing network connection is disconnected
    /// </summary>
    public event NetworkDisconnectionHandler NetworkDisconnected = default!;

    /// <summary>
    /// Event raised when the WiFi adapter is initialized
    /// </summary>
    public event EventHandler WiFiAdapterInitialized = default!;

    /// <summary>
    /// The Bluetooth adapter
    /// </summary>
    public IBluetoothAdapter? BluetoothAdapter { get; protected set; }

    /// <summary>
    /// The coprocessor responsible for network and bluetooth communications
    /// </summary>
    public ICoprocessor? Coprocessor { get; protected set; }

    /// <inheritdoc/>
    public DeviceCapabilities Capabilities { get; }

    /// <inheritdoc/>
    public IPlatformOS PlatformOS { get; protected set; }

    /// <inheritdoc/>
    public IDeviceInformation Information { get; protected set; }

    /// <inheritdoc/>
    public INetworkAdapterCollection NetworkAdapters => networkAdapters;

    /// <summary>
    /// Gets an IPin by name
    /// </summary>
    /// <param name="pinName"></param>
    /// <returns></returns>
    public abstract IPin GetPin(string pinName);

    /// <inheritdoc/>
    public abstract IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = IPwmOutputController.DefaultPwmDutyCycle, bool inverted = false);

    /// <summary>
    /// Gets the current Battery information
    /// </summary>
    /// <remarks>Override this method if you have an SMBus Smart Battery</remarks>
    public abstract BatteryInfo? GetBatteryInfo();

    /// <summary>
    /// The collection of network adapters
    /// </summary>
    protected NetworkAdapterCollection networkAdapters;

    /// <summary>
    /// The ESP32 coprocessor used for WiFi and Bluetooth
    /// </summary>
    protected Esp32Coprocessor? esp32;

    /// <summary>
    /// Lock for initializing the coprocessor
    /// </summary>
    protected object coprocInitLock = new object();

    /// <summary>
    /// The IO controller for the device
    /// </summary>
    protected IMeadowIOController IoController { get; }

    //==== constructors
    /// <summary>
    /// Provides required construction of the F7MicroBase abstract class
    /// </summary>
    /// <param name="ioController"></param>
    /// <param name="analogCapabilities"></param>
    /// <param name="networkCapabilities"></param>
    /// <param name="storageCapabilities"></param>
    public F7MicroBase(
        F7GPIOManager ioController,
        AnalogCapabilities analogCapabilities,
        NetworkCapabilities networkCapabilities,
        StorageCapabilities storageCapabilities)
    {
        IoController = ioController;

        Capabilities = new DeviceCapabilities(analogCapabilities, networkCapabilities, storageCapabilities);

        PlatformOS = new F7PlatformOS(ioController);

        Information = new F7DeviceInformation();

        networkAdapters = new NetworkAdapterCollection();
        networkAdapters.NetworkConnected += (s, e) => NetworkConnected?.Invoke(s, e);
        networkAdapters.NetworkDisconnected += (s, e) => NetworkDisconnected?.Invoke(s, e);
    }

    /// <summary>
    /// Initializes the F7Micro platform hardware
    /// </summary>
    public void Initialize(MeadowPlatform detectedPlatform)
    {
        var reservedPins = PlatformOS.ReservedPins?.ToUpper().Split(';', StringSplitOptions.RemoveEmptyEntries) ?? null;
        IoController.Initialize(reservedPins);

        InitCoprocessor();
    }

    /// <summary>
    /// Initializes the ESP32 Coprocessor
    /// </summary>
    /// <returns></returns>
    protected bool InitCoprocessor()
    {
        lock (coprocInitLock)
        {
            if (this.esp32 == null)
            {
                try
                {
                    // instantiate the co proc and set the various adapters
                    // to be it.
                    this.esp32 = new Esp32Coprocessor();
                    BluetoothAdapter = esp32;
                    Coprocessor = esp32;

                    esp32.SystemMessageReceived += (s, e) =>
                    {
                        ReliabilityService?.OnMeadowSystemError(new Esp32SystemErrorInfo((int)e.fn, e.status));
                    };

                    if (PlatformOS.SelectedNetwork == IPlatformOS.NetworkConnectionType.WiFi)
                    {
                        Resolver.Log.Info($"Device is configured to use WiFi for the network interface", MessageGroup.Core);
                        var wifiAdapter = new Esp32WiFiAdapter(esp32);
                        networkAdapters.Add(wifiAdapter);

                        if (wifiAdapter.AutoConnect)
                        {
                            Resolver.Log.Debug($"Device configured to auto-connect to SSID '{wifiAdapter.DefaultSsid}'", MessageGroup.Core);

                            if (string.IsNullOrEmpty(wifiAdapter.DefaultSsid))
                            {
                                Resolver.Log.Warn($"Device configured to auto-connect to WiFi, but no Default SSID was provided.  Deploy a wifi.config.yaml file.", MessageGroup.Core);
                            }
                            else
                            {
                                wifiAdapter.ConnectToDefaultAccessPoint(TimeSpan.FromSeconds(60), CancellationToken.None).Wait();
                            }
                        }
                    }
                    else if (PlatformOS.SelectedNetwork == IPlatformOS.NetworkConnectionType.Cell)
                    {
                        Resolver.Log.Info($"Device is configured to use Cell for the network interface", MessageGroup.Core);
                        networkAdapters.Add(new F7CellNetworkAdapter(esp32));
                    }
                    else if (PlatformOS.SelectedNetwork == IPlatformOS.NetworkConnectionType.Ethernet)
                    {
                        Resolver.Log.Info($"Device is configured to use Ethernet for the network interface", MessageGroup.Core);
                        networkAdapters.Add(new F7EthernetNetworkAdapter(esp32));
                    }
                }
                catch (Exception e)
                {
                    Resolver.Log.Error($"Unable to create ESP32 coprocessor: {e.Message}", MessageGroup.Core);
                    return false;
                }
                finally
                {

                }
                return true;
            }
            else
            { // already initialized, bail out
                return true;
            }

        }
    }

    /// <summary>
    /// Gets the current processor temperature
    /// </summary>
    /// <returns></returns>
    internal Temperature GetProcessorTemperature()
    {
        return IoController.GetTemperature();
    }

    /// <summary>
    /// Creates a new `Counter` instance to count the given transitions on the given pin
    /// </summary>
    /// <param name="pin"></param>
    /// <param name="edge"></param>
    /// <returns></returns>
    public ICounter CreateCounter(
        IPin pin,
        InterruptMode edge)
    {
        return new Counter(pin, edge);
    }

    /// <inheritdoc/>
    public IReliabilityService? ReliabilityService
    {
        get => _reliabilityService ??= new F7ReliabilityService();
    }
}
