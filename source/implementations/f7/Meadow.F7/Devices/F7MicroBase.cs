using Meadow.Gateways;
using Meadow.Hardware;
using Meadow.Units;
using System;

namespace Meadow.Devices;

/// <summary>
/// Represents a Meadow F7 micro device. Includes device-specific IO mapping,
/// capabilities and provides access to the various device-specific features.
/// </summary>
public abstract partial class F7MicroBase : IF7MeadowDevice
{
    public event NetworkConnectionHandler NetworkConnected = delegate { };
    public event NetworkDisconnectionHandler NetworkDisconnected = delegate { };

    //==== events
    public event EventHandler WiFiAdapterInitialized = delegate { };

    //==== public properties
    public IBluetoothAdapter? BluetoothAdapter { get; protected set; }
    public ICoprocessor? Coprocessor { get; protected set; }

    public DeviceCapabilities Capabilities { get; }

    public IPlatformOS PlatformOS { get; protected set; }

    public IDeviceInformation Information { get; protected set; }

    public INetworkAdapterCollection NetworkAdapters => networkAdapters;

    /// <summary>
    /// Gets an IPin by name
    /// </summary>
    /// <param name="pinName"></param>
    /// <returns></returns>
    public abstract IPin GetPin(string pinName);

    public abstract IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = IPwmOutputController.DefaultPwmDutyCycle, bool inverted = false);

    /// <summary>
    /// Gets the current Battery information
    /// </summary>
    /// <remarks>Override this method if you have an SMBus Smart Battery</remarks>
    public abstract BatteryInfo GetBatteryInfo();

    //==== internals
    protected NetworkAdapterCollection networkAdapters;
    protected Esp32Coprocessor? esp32;
    protected object coprocInitLock = new object();
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
        IMeadowIOController ioController,
        AnalogCapabilities analogCapabilities,
        NetworkCapabilities networkCapabilities,
        StorageCapabilities storageCapabilities)
    {
        IoController = ioController;

        Capabilities = new DeviceCapabilities(analogCapabilities, networkCapabilities, storageCapabilities);

        PlatformOS = new F7PlatformOS();

        Information = new F7DeviceInformation();

        networkAdapters = new NetworkAdapterCollection();
        networkAdapters.NetworkConnected += (s, e) => NetworkConnected.Invoke(s, e);
        networkAdapters.NetworkDisconnected += (s) => NetworkDisconnected.Invoke(s);
    }

    /// <summary>
    /// Initializes the F7Micro platform hardware
    /// </summary>
    public void Initialize()
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

                    if (PlatformOS.SelectedNetwork == IPlatformOS.NetworkConnectionType.WiFi)
                    {
                        Resolver.Log.Info($"Device is configured to use WiFi for the network interface");
                        networkAdapters.Add(esp32);

                        if (esp32.AutoConnect)
                        {
                            Resolver.Log.Debug($"Device configured to auto-connect to SSID '{esp32.DefaultSsid}'");

                            if (string.IsNullOrEmpty(esp32.DefaultSsid))
                            {
                                Resolver.Log.Warn($"Device configured to auto-connect to WiFi, but no Default SSID was provided.  Deploy a wifi.config.yaml file.");
                            }
                            else
                            {
                                esp32.ConnectToDefaultAccessPoint();
                            }
                        }
                    }

                    esp32.NtpTimeChanged += (s, e) =>
                    {
                        // TODO: forward to the NtpClient
                    };
                }
                catch (Exception e)
                {
                    Resolver.Log.Error($"Unable to create ESP32 coprocessor: {e.Message}");
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
    public Temperature GetProcessorTemperature()
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
}
