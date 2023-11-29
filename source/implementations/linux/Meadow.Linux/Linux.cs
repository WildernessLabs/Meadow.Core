﻿using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Pinouts;
using Meadow.Units;
using System;
using System.Linq;

namespace Meadow
{
    /// <summary>
    /// Represents an instance of Meadow as a generic Linux process
    /// </summary>
    /// <typeparam name="TPinout"></typeparam>
    public class Linux<TPinout> : IMeadowDevice
        where TPinout : IPinDefinitions, new()
    {
        private SysFsGpioDriver _sysfs = null!;
        private Gpiod _gpiod = null!;
        private Lazy<NativeNetworkAdapterCollection> _networkAdapters;

        /// <inheritdoc/>
        public event PowerTransitionHandler BeforeReset;
        /// <inheritdoc/>
        public event PowerTransitionHandler BeforeSleep;
        /// <inheritdoc/>
        public event PowerTransitionHandler AfterWake;
        /// <inheritdoc/>
        public event NetworkConnectionHandler NetworkConnected;
        /// <inheritdoc/>
        public event NetworkDisconnectionHandler NetworkDisconnected;

        public TPinout Pins { get; }
        public DeviceCapabilities Capabilities { get; }
        public IPlatformOS PlatformOS { get; }
        public IDeviceInformation Information { get; }
        public INetworkAdapterCollection NetworkAdapters => _networkAdapters.Value;

        /// <summary>
        /// Creates the Meadow on Linux infrastructure instance
        /// </summary>
        public Linux()
        {
            if (typeof(TPinout) == typeof(JetsonNano) || typeof(TPinout) == typeof(JetsonXavierAGX))
            {
                PlatformOS = new JetsonPlatformOS();
            }
            else
            {
                PlatformOS = new LinuxPlatformOS();
            }

            _networkAdapters = new Lazy<NativeNetworkAdapterCollection>(
                new NativeNetworkAdapterCollection());

            Information = new LinuxDeviceInfo();

            Pins = new TPinout();
            Pins.Controller = this;

            Capabilities = new DeviceCapabilities(
                new AnalogCapabilities(false, null),
                new NetworkCapabilities(false, true),
                new StorageCapabilities(true)
                );
        }

        public void Initialize()
        {
            _sysfs = new SysFsGpioDriver();

            try
            {
                _gpiod = new Gpiod(Resolver.Log);
            }
            catch
            {
                Resolver.Log.Warn("Platform does not support gpiod");
            }
        }

        public IPin GetPin(string pinName)
        {
            return Pins.AllPins.First(p => string.Compare(p.Name, pinName) == 0);
        }

        public II2cBus CreateI2cBus(int busNumber = 1)
        {
            return CreateI2cBus(busNumber, II2cController.DefaultI2cBusSpeed);
        }

        public II2cBus CreateI2cBus(int busNumber, I2cBusSpeed busSpeed)
        {
            return new I2CBus(busNumber, busSpeed);
        }

        public II2cBus CreateI2cBus(IPin[] pins, I2cBusSpeed busSpeed)
        {
            return CreateI2cBus(pins[0], pins[1], busSpeed);
        }

        public II2cBus CreateI2cBus(IPin clock, IPin data, I2cBusSpeed busSpeed)
        {
            // TODO: implement this based on channel caps (this is platform specific right now)

            if (Pins is JetsonNano)
            {
                if (clock == Pins["PIN05"] && data == Pins["PIN03"])
                {
                    return new I2CBus(1, busSpeed);
                }
                else if (clock == Pins["PIN28"] && data == Pins["PIN27"])
                {
                    return new I2CBus(0, busSpeed);
                }
            }
            if (Pins is JetsonXavierAGX)
            {
                if (clock == Pins["I2C_GP2_CLK"] && data == Pins["I2C_GP2_DAT"])
                {
                    return new I2CBus(1, busSpeed);
                }
                else if (clock == Pins["I2C_GP5_CLK"] && data == Pins["I2C_GP5_DAT"])
                {
                    return new I2CBus(8, busSpeed);
                }
            }
            else if (Pins is RaspberryPi)
            {
                if (clock == Pins["PIN05"] && data == Pins["PIN03"])
                {
                    return new I2CBus(1, busSpeed);
                }
            }
            else if (Pins is SnickerdoodleBlack)
            {
                return new KrtklI2CBus(busSpeed);
            }

            throw new ArgumentOutOfRangeException("Requested pins are not I2C bus pins");
        }

        public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] suffixDelimiter, bool preserveDelimiter, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
        {
            var classicPort = CreateSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
            return SerialMessagePort.From(classicPort, suffixDelimiter, preserveDelimiter);
        }

        public ISerialMessagePort CreateSerialMessagePort(SerialPortName portName, byte[] prefixDelimiter, bool preserveDelimiter, int messageLength, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 512)
        {
            var classicPort = CreateSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
            return SerialMessagePort.From(classicPort, prefixDelimiter, preserveDelimiter, messageLength);
        }

        public ISerialPort CreateSerialPort(SerialPortName portName, int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One, int readBufferSize = 1024)
        {
            return new LinuxSerialPort(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
        }

        public IDigitalOutputPort CreateDigitalOutputPort(IPin pin, bool initialState = false, OutputType initialOutputType = OutputType.PushPull)
        {
            if (_gpiod != null)
            {
                return new GpiodDigitalOutputPort(_gpiod, pin, initialState);
            }
            else
            {
                return new SysFsDigitalOutputPort(_sysfs, pin, initialState);
            }
        }

        public IDigitalInputPort CreateDigitalInputPort(IPin pin)
        {
            return CreateDigitalInputPort(pin, ResistorMode.Disabled);
        }

        public IDigitalInputPort CreateDigitalInputPort(IPin pin, ResistorMode resistorMode)
        {
            if (_gpiod != null)
            {
                return new GpiodDigitalInputPort(_gpiod, pin, new GpiodDigitalChannelInfo(pin.Name), resistorMode);
            }
            else
            {
                return new SysFsDigitalInputPort(_sysfs, pin, new SysFsDigitalChannelInfo(pin.Name), resistorMode);
            }
        }

        public IDigitalInterruptPort CreateDigitalInterruptPort(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
        {
            if (_gpiod != null)
            {
                return new GpiodDigitalInterruptPort(_gpiod, pin, new GpiodDigitalChannelInfo(pin.Name), interruptMode, resistorMode, debounceDuration, glitchDuration);
            }
            else
            {
                return new SysFsDigitalInterruptPort(_sysfs, pin, new SysFsDigitalChannelInfo(pin.Name), interruptMode, resistorMode, debounceDuration, glitchDuration);
            }
        }

        public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, SpiClockConfiguration config)
        {
            return CreateSpiBus(clock, mosi, miso, config.SpiMode, config.Speed);
        }

        public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, Units.Frequency speed)
        {
            return CreateSpiBus(clock, mosi, miso, SpiClockConfiguration.Mode.Mode0, speed);
        }

        public ISpiBus CreateSpiBus(IPin clock, IPin mosi, IPin miso, SpiClockConfiguration.Mode mode, Units.Frequency speed)
        {
            return new SpiBus(0, (SpiBus.SpiMode)mode, speed);
        }

        // ----- BELOW HERE ARE NOT YET IMPLEMENTED -----

        public IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, float voltageReference = 3.3F)
        {
            throw new NotImplementedException();
        }

        public IAnalogInputPort CreateAnalogInputPort(IPin pin, int sampleCount, TimeSpan sampleInterval, Voltage voltageReference)
        {
            throw new NotImplementedException();
        }

        public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState = false, InterruptMode interruptMode = InterruptMode.None, ResistorMode resistorMode = ResistorMode.Disabled, PortDirectionType initialDirection = PortDirectionType.Input, double debounceDuration = 0, double glitchDuration = 0, OutputType output = OutputType.PushPull)
        {
            throw new NotImplementedException();
        }

        public IPwmPort CreatePwmPort(IPin pin, Frequency frequency, float dutyCycle = 0.5F, bool invert = false)
        {
            throw new NotImplementedException();
        }

        public void OnReset()
        {
            throw new NotImplementedException();
        }

        public void SetClock(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public void WatchdogEnable(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void WatchdogReset()
        {
            throw new NotImplementedException();
        }

        public void OnSleep()
        {
            throw new NotImplementedException();
        }

        public BatteryInfo GetBatteryInfo()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            // TODO: $ sudo reboot
            throw new NotImplementedException();
        }

        public void Sleep(TimeSpan duration)
        {
            // not supported on RasPi
            throw new PlatformNotSupportedException();
        }

        public void OnShutdown(out bool complete, Exception? e = null)
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception e, out bool recovered)
        {
            throw new NotImplementedException();
        }

        public void OnResume()
        {
            throw new NotImplementedException();
        }

        public void OnRecovery(Exception e)
        {
            throw new NotImplementedException();
        }

        public void OnUpdate(Version newVersion, out bool approveUpdate)
        {
            throw new NotImplementedException();
        }

        public void OnUpdateComplete(Version oldVersion, out bool rollbackUpdate)
        {
            throw new NotImplementedException();
        }

        public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState, InterruptMode interruptMode, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
        {
            throw new NotImplementedException();
        }

        public ICounter CreateCounter(IPin pin, InterruptMode edge)
        {
            throw new NotImplementedException();
        }

        public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState)
        {
            throw new NotImplementedException();
        }

        public IBiDirectionalInterruptPort CreateBiDirectionalInterruptPort(IPin pin, bool initialState, InterruptMode interruptMode, ResistorMode resistorMode, PortDirectionType initialDirection, TimeSpan debounceDuration, TimeSpan glitchDuration, OutputType output = OutputType.PushPull)
        {
            throw new NotImplementedException();
        }

        public IAnalogInputArray CreateAnalogInputArray(params IPin[] pins)
        {
            throw new NotImplementedException();
        }
    }
}
