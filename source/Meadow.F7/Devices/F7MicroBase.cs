using Meadow.Gateways;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Linq;
using System.Threading;
using static Meadow.Core.Interop;
using static Meadow.Core.Interop.Nuttx;

namespace Meadow.Devices
{
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
        public event PowerTransitionHandler BeforeReset = delegate { };
        public event PowerTransitionHandler BeforeSleep = delegate { };
        public event PowerTransitionHandler AfterWake = delegate { };

        //==== public properties
        public IBluetoothAdapter? BluetoothAdapter { get; protected set; }
        //        public IWiFiAdapter? WiFiAdapter { get; protected set; }
        public ICoprocessor? Coprocessor { get; protected set; }

        public DeviceCapabilities Capabilities { get; }

        public IPlatformOS PlatformOS { get; protected set; }

        public IDeviceInformation Information { get; protected set; }

        public INetworkAdapterCollection NetworkAdapters => networkAdapters;

        public IWiFiAdapter? GetPrimaryWiFiAdapter() => NetworkAdapters.OfType<IWiFiAdapter>().First();

        public IWiredNetworkAdapter? GetPrimaryWiredNetworkAdapter() => NetworkAdapters.OfType<IWiredNetworkAdapter>().First();

        public abstract IPin GetPin(string pinName);
        public abstract IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = IPwmOutputController.DefaultPwmDutyCycle, bool inverted = false);

        //==== internals
        protected NetworkAdapterCollection networkAdapters;
        protected Esp32Coprocessor? esp32;
        protected object coprocInitLock = new object();
        protected IMeadowIOController IoController { get; }

        //==== constructors
        public F7MicroBase(IMeadowIOController ioController, AnalogCapabilities analogCapabilities, NetworkCapabilities networkCapabilities)
        {
            IoController = ioController;

            Capabilities = new DeviceCapabilities(analogCapabilities, networkCapabilities);

            PlatformOS = new F7PlatformOS();

            Information = new F7DeviceInformation();

            networkAdapters = new NetworkAdapterCollection();
            networkAdapters.NetworkConnected += (s, e) => NetworkConnected.Invoke(s, e);
            networkAdapters.NetworkDisconnected += (s) => NetworkDisconnected.Invoke(s);
        }

        public void Initialize()
        {
            IoController.Initialize();

            InitCoprocessor();
        }

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
                        //                        WiFiAdapter = esp32;
                        Coprocessor = esp32;

                        networkAdapters.Add(esp32);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Unable to create ESP32 coprocessor: {e.Message}");
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

        //==== antenna stuff

        /// <summary>
        /// Get the currently setlected WiFi antenna.
        /// </summary>
        public AntennaType CurrentAntenna
        {
            get
            {
                // TODO: this feels awkward - should all of this antenna be in the adapter?
                var wifi = NetworkAdapters.FirstOrDefault(a => a is IWirelessNetworkAdapter) as IWiFiAdapter;

                if (wifi != null)
                {
                    return wifi.Antenna;
                }
                else
                {
                    throw new Exception("Coprocessor not initialized.");
                }
            }
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
            // TODO: this feels awkward - should all of this antenna be in the adapter?
            var wifi = NetworkAdapters.FirstOrDefault(a => a is IWirelessNetworkAdapter) as IWiFiAdapter;

            if (wifi != null)
            {
                wifi.SetAntenna(antenna, persist);
            }
            else
            {
                throw new Exception("Coprocessor not initialized.");
            }
        }

        /// <summary>
        /// Gets the current Battery information
        /// </summary>
        /// <remarks>Override this method if you have an SMBus Smart Battery</remarks>
        public virtual BatteryInfo GetBatteryInfo()
        {
            if (Coprocessor != null)
            {
                return new BatteryInfo
                {
                    Voltage = new Voltage(Coprocessor.GetBatteryLevel(), Voltage.UnitType.Volts)
                };
            }

            throw new Exception("Coprocessor not initialized.");
        }

        /// <summary>
        /// Gets the current processor temperature
        /// </summary>
        /// <returns></returns>
        public virtual Temperature GetProcessorTemperature()
        {
            return IoController.GetTemperature();
        }

        public void Reset()
        {
            BeforeReset?.Invoke();

            UPD.Ioctl(Nuttx.UpdIoctlFn.PowerReset);
        }

        public void SetClock(DateTime dateTime)
        {
            var ts = new Core.Interop.Nuttx.timespec
            {
                tv_sec = new DateTimeOffset(dateTime).ToUnixTimeSeconds()
            };

            Core.Interop.Nuttx.clock_settime(Core.Interop.Nuttx.clockid_t.CLOCK_REALTIME, ref ts);
        }

        public void Sleep(int seconds = Timeout.Infinite)
        {
            var cmd = new UpdSleepCommand
            {
                SecondsToSleep = seconds
            };

            BeforeSleep?.Invoke();

            UPD.Ioctl(Nuttx.UpdIoctlFn.PowerSleep, cmd);

            // Stop execution while the device actually does it's thing
            Thread.Sleep(100);

            // TODO: see how long this actually takes

            AfterWake?.Invoke();
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
            return new Counter(this, pin, edge);
        }
    }
}
