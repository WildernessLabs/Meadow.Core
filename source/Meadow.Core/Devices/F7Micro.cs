using System.Collections.Generic;
using Meadow.Hardware;
using Meadow.Gateway.WiFi;
using System;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a Meadow F7 micro device. Includes device-specific IO mapping,
    /// capabilities and provides access to the various device-specific features.
    /// </summary>
    public partial class F7Micro : IIODevice
    {
        /// <summary>
        /// The default resolution for analog inputs
        /// </summary>
        public const int DefaultA2DResolution = 12;

        public DeviceCapabilities Capabilities { get; protected set; }

        /// <summary>
        /// Gets the pins.
        /// </summary>
        /// <value>The pins.</value>
        public F7MicroPinDefinitions Pins { get; protected set; }

        public SerialPortNameDefinitions SerialPortNames { get; protected set; }
            = new SerialPortNameDefinitions();


        internal IIOController IoController { get; private set; }

        static F7Micro() { }

        public F7Micro()
        {
            this.Capabilities = new DeviceCapabilities(
                new AnalogCapabilities(true, DefaultA2DResolution),
                new NetworkCapabilities(true, true)
                );

            this.IoController = new F7GPIOManager();
            this.IoController.Initialize();

            this.Pins = new F7MicroPinDefinitions();
        }

        public IDigitalOutputPort CreateDigitalOutputPort(
            IPin pin,
            bool initialState = false)
        {
            return DigitalOutputPort.From(pin, this.IoController, initialState);
        }

        public IDigitalInputPort CreateDigitalInputPort(
            IPin pin,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            uint debounceDuration = 0,
            uint glitchFilterCycleCount = 0
            )
        {
            return DigitalInputPort.From(pin, this.IoController, interruptMode, resistorMode, debounceDuration, glitchFilterCycleCount);
        }

        public IBiDirectionalPort CreateBiDirectionalPort(
            IPin pin,
            bool initialState = false,
            bool glitchFilter = false,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input)
        {
            return BiDirectionalPort.From(pin, this.IoController, initialState, glitchFilter, interruptMode, resistorMode, initialDirection);
        }

        public IAnalogInputPort CreateAnalogInputPort(
            IPin pin,
            float voltageReference = IIODevice.DefaultA2DReferenceVoltage)
        {
            return AnalogInputPort.From(pin, this.IoController, voltageReference);
        }

        public IPwmPort CreatePwmPort(
            IPin pin,
            float frequency = IIODevice.DefaultPwmFrequency,
            float dutyCycle = IIODevice.DefaultPwmDutyCycle,
            bool inverted = false)
        {
            bool isOnboard = IsOnboardLed(pin);
            return PwmPort.From(pin, this.IoController, frequency, dutyCycle, inverted, isOnboard);
        }

        /// <summary>
        /// Tests whether or not the pin passed in belongs to an onboard LED
        /// component. Used for a dirty dirty hack.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns>whether or no the pin belons to the onboard LED</returns>
        protected bool IsOnboardLed(IPin pin)
        {
            return (
                pin == Pins.OnboardLedBlue ||
                pin == Pins.OnboardLedGreen ||
                pin == Pins.OnboardLedRed
                );
        }

        public ISerialPort CreateSerialPort(
            SerialPortName portName,
            int baudRate,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 4096)
        {
            return SerialPort.From(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
        }

        /// <summary>
        /// Creates a SPI bus instance for the requested bus speed with the Meadow- default IPins for CLK, MOSI and MISO
        /// </summary>
        /// <param name="speedkHz">The bus speed (in kHz)</param>
        /// <returns>An instance of an IISpiBus</returns>
        public ISpiBus CreateSpiBus(
            long speedkHz = IIODevice.DefaultSpiBusSpeed
        )
        {
            return CreateSpiBus(Pins.SCK, Pins.MOSI, Pins.MISO, speedkHz);
        }

        /// <summary>
        /// Creates a SPI bus instance for the requested control pins and bus speed
        /// </summary>
        /// <param name="pins">IPint instances used for (in this order) CLK, MOSI, MISO</param>
        /// <param name="speedkHz">The bus speed (in kHz)</param>
        /// <returns>An instance of an IISpiBus</returns>
        public ISpiBus CreateSpiBus(
            IPin[] pins,
            long speedkHz = IIODevice.DefaultSpiBusSpeed
        )
        {
            return CreateSpiBus(pins[0], pins[1], pins[2], speedkHz);
        }

        /// <summary>
        /// Creates a SPI bus instance for the requested control pins and bus speed
        /// </summary>
        /// <param name="clock">The IPin instance to use as the bus clock</param>
        /// <param name="mosi">The IPin instance to use for data transmit (master out/slave in)</param>
        /// <param name="miso">The IPin instance to use for data receive (master in/slave out)</param>
        /// <param name="speedkHz">The bus speed (in kHz)</param>
        /// <returns>An instance of an IISpiBus</returns>
        public ISpiBus CreateSpiBus(
            IPin clock,
            IPin mosi,
            IPin miso,
            long speedkHz = IIODevice.DefaultSpiBusSpeed
        )
        {
            var bus = SpiBus.From(clock, mosi, miso);
            bus.BusNumber = GetSpiBusNumberForPins(clock, mosi, miso);
            bus.Configuration.SpeedKHz = speedkHz;
            return bus;
        }

        /// <summary>
        /// Creates a SPI bus instance for the requested control pins and bus speed
        /// </summary>
        /// <param name="clock">The IPin instance to use as the bus clock</param>
        /// <param name="mosi">The IPin instance to use for data transmit (master out/slave in)</param>
        /// <param name="miso">The IPin instance to use for data receive (master in/slave out)</param>
        /// <param name="config">The bus clock configuration parameters</param>
        /// <returns>An instance of an IISpiBus</returns>
        public ISpiBus CreateSpiBus(
            IPin clock,
            IPin mosi,
            IPin miso,
            SpiClockConfiguration config
        )
        {
            var bus = SpiBus.From(clock, mosi, miso);
            bus.BusNumber = GetSpiBusNumberForPins(clock, mosi, miso);
            bus.Configuration = config;
            return bus;
        }

        private int GetSpiBusNumberForPins(IPin clock, IPin mosi, IPin miso)
        {
            // we're only looking at clock pin.  
            // For the F7 meadow it's enough to know and any attempt to use other pins will get caught by other sanity checks
            if (clock == Pins.ESP_CLK)
            {
                return 2;
            }
            else if (clock == Pins.SCK)
            {
                return 3;
            }

            // this is an unsupported bus, but will get caught elsewhere
            return -1;
        }

        /// <summary>
        /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed
        /// </summary>
        /// <param name="frequencyHz">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            I2cBusSpeed busSpeed
        )
        {
            return CreateI2cBus(Pins.I2C_SCL, Pins.I2C_SDA, (int)busSpeed);
        }

        /// <summary>
        /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed
        /// </summary>
        /// <param name="frequencyHz">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            int frequencyHz = IIODevice.DefaultI2cBusSpeed
        )
        {
            return CreateI2cBus(Pins.I2C_SCL, Pins.I2C_SDA, frequencyHz);
        }

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequencyHz">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            IPin[] pins,
            int frequencyHz = IIODevice.DefaultI2cBusSpeed
        )
        {
            return CreateI2cBus(pins[0], pins[1], frequencyHz);
        }

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequencyHz">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            IPin clock,
            IPin data,
            int frequencyHz = IIODevice.DefaultI2cBusSpeed
        )
        {
            return I2cBus.From(this.IoController, clock, data, frequencyHz);
        }

        public void SetClock(DateTime dateTime)
        {
            var ts = new Core.Interop.Nuttx.timespec
            {
                tv_sec = new DateTimeOffset(dateTime).ToUnixTimeSeconds()
            };

            Core.Interop.Nuttx.clock_settime(Core.Interop.Nuttx.clockid_t.CLOCK_REALTIME, ref ts);
        }
    }
}
