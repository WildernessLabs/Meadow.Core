﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Meadow.Gateways;
using Meadow.Hardware;
using Meadow.Units;
using static Meadow.Core.Interop;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a Meadow F7 micro device. Includes device-specific IO mapping,
    /// capabilities and provides access to the various device-specific features.
    /// </summary>
    public abstract partial class F7MicroBase : IF7MeadowDevice, IBatteryChargeController
    {
        protected Esp32Coprocessor? esp32;

        public IBluetoothAdapter? BluetoothAdapter { get; protected set; }
        public IWiFiAdapter? WiFiAdapter { get; protected set; }
        public ICoprocessor? Coprocessor { get; protected set; }

        public event EventHandler WiFiAdapterInitialized = delegate { };

        public DeviceCapabilities Capabilities { get; }

        /// <summary>
        /// Gets the pins.
        /// </summary>
        /// <value>The pins.</value>
        public IF7MicroPinout Pins { get; }

        protected IMeadowIOController IoController { get; }

        public F7MicroBase(IF7MicroPinout pins, IMeadowIOController ioController, AnalogCapabilities analogCapabilities, NetworkCapabilities networkCapabilities)
        {
            Pins = pins;
            IoController = ioController;

            Capabilities = new DeviceCapabilities(analogCapabilities, networkCapabilities);
        }

        public void Initialize()
        {
            IoController.Initialize();

            
        }

        public IPin GetPin(string pinName)
        {
            return Pins.AllPins.FirstOrDefault(p => p.Name == pinName || p.Key.ToString() == p.Name);
        }

        public Task<bool> InitCoprocessor()
        {
            if (!IsCoprocessorInitialized()) {
                return Task.Run<bool>(() => {
                    try {
                        //TODO: looks like we're also instantiating this in the ctor
                        // need to cleanup.
                        //Console.WriteLine($"InitWiFiAdapter()");
                        if (this.esp32 == null) {
                            this.esp32 = new Esp32Coprocessor();
                        }
                        BluetoothAdapter = esp32;
                        WiFiAdapter = esp32;
                        Coprocessor = esp32;
                    } catch (Exception e) {
                        Console.WriteLine($"Unable to create ESP32 coprocessor: {e.Message}");
                        return false;
                    }
                    return true;
                });
            } else {
                return Task.FromResult<bool>(true);
            }
        }

        // TODO: get rid of this in b5.5
        [Obsolete("Use `InitCoprocessor()` instead. Will be removed in the future.")]
        public Task<bool> InitWiFiAdapter()
        {
            return InitCoprocessor();
        }

        // TODO: get rid of this in b5.5
        [Obsolete("Use `InitCoprocessor()` instead. Will be removed in the future.")]
        public Task<bool> InitBluetoothAdapter()
        {
            return InitCoprocessor();
        }

        public void SetClock(DateTime dateTime)
        {
            var ts = new Core.Interop.Nuttx.timespec {
                tv_sec = new DateTimeOffset(dateTime).ToUnixTimeSeconds()
            };

            Core.Interop.Nuttx.clock_settime(Core.Interop.Nuttx.clockid_t.CLOCK_REALTIME, ref ts);
        }

        /// <summary>
        /// Check if the coprocessor is available / ready and throw an exception if it
        /// has not been setup.
        /// </summary>
        protected bool IsCoprocessorInitialized()
        {
            if (esp32 == null)
            {
                return false;
            }
            else
            {
                return true;
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
                if (WiFiAdapter != null)
                {
                    return WiFiAdapter.Antenna;
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
            if (WiFiAdapter != null)
            {
                WiFiAdapter.SetAntenna(antenna, persist);
            }
            else
            {
                throw new Exception("Coprocessor not initialized.");
            }
        }

        //TODO: need the Read()/StartUpdating()/StopUpdating() pattern here.
        /// <summary>
        /// Gets the current battery charge level in voltage.
        /// </summary>
        public Voltage GetBatteryLevel()
        {
            if (Coprocessor != null)
            {
                return (new Voltage(Coprocessor.GetBatteryLevel(), Voltage.UnitType.Volts));
            }
            else
            {
                throw new Exception("Coprocessor not initialized.");
            }
        }

        /// <summary>
        /// Set the name of the board as it will appear on the network (when connected to a network / access point).
        /// </summary>
        /// <param name="deviceName">Name to be used.</param>
        /// <returns>True if the request was successful, false otherwise.</returns>
        public bool SetDeviceNmae(string deviceName)
        {
            bool result = F7Micro.Configuration.SetDeviceName(deviceName);
            //
            //  May need to store this somewhere later to split the return.
            //
            return (result);
        }

        /// <summary>
        /// Gets the current processor temerpature
        /// </summary>
        /// <returns></returns>
        public Temperature GetProcessorTemperature()
        {
            return IoController.GetTemperature();
        }

        public void Reset()
        {
            UPD.Ioctl(Nuttx.UpdIoctlFn.PowerReset);
        }

    }
}
