using Meadow.Gateways;
using Meadow.Hardware;
using Meadow.Units;
using System;
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
        public INtpClient NtpClient { get; }

        public IDeviceInformation Information { get; protected set; }

        public INetworkAdapterCollection NetworkAdapters => networkAdapters;

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

            NtpClient = new NtpClient();
        }

        public void Initialize()
        {
            IoController.Initialize();

            InitCoprocessor();
        }

        protected bool InitCoprocessor()
        {
            lock(coprocInitLock)
            {
                if(this.esp32 == null)
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

                        esp32.NtpTimeChanged += (s, e) =>
                        {
                            // forward to the NtpClient
                        };
                    }
                    catch(Exception e)
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

        /// <summary>
        /// Gets the current Battery information
        /// </summary>
        /// <remarks>Override this method if you have an SMBus Smart Battery</remarks>
        public virtual BatteryInfo GetBatteryInfo()
        {
            if(Coprocessor != null)
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
