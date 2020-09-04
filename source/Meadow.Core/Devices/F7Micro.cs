using System.Collections.Generic;
using Meadow.Hardware;
using Meadow.Gateway.WiFi;
using System;
using System.Threading.Tasks;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a Meadow F7 micro device. Includes device-specific IO mapping,
    /// capabilities and provides access to the various device-specific features.
    /// </summary>
    public partial class F7Micro : IIODevice
    {
        //private Esp32Coprocessor esp32;

        //public WiFiAdapter WiFiAdapter { get; protected set; }

        public event EventHandler WiFiAdapterInitilaized = delegate {}; 

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

            // TODO: do we want to block until this peripheral is up?
            // right now there's a 5 second delay. i wonder if we can get a
            // response when it's up instead. if it's typically within a
            // reasonable timeframe, maybe we block?
            //this.InitEsp32CoProc();
        }

        //protected Task<bool> InitEsp32CoProc()
        //{
        //    Console.WriteLine("Initializing Esp32 coproc.");
        //    return Task.Run<bool>(async () => {
        //        try {
        //            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        //            //stopwatch.Start()
        //            //Console.WriteLine("creating Esp32 Coproc.");
        //            this.esp32 = new Esp32Coprocessor();
        //            this.esp32.Reset();
        //            await Task.Delay(5000);
        //            this.WiFiAdapter = new WiFiAdapter(this.esp32);
        //            Console.WriteLine("Esp32 coproc initialization complete.");
        //            // if we make the creation non-blocking:
        //            this.WiFiAdapterInitilaized(this, new EventArgs());
        //            return true;
        //        } catch (Exception e) {
        //            Console.WriteLine($"Unable to create Esp32 coproc: {e.Message}");
        //            return false;
        //        }
        //    });
        //}

        public IDigitalOutputPort CreateDigitalOutputPort(
            IPin pin,
            bool initialState = false,
            OutputType initialOutputType = OutputType.PushPull)
        {
            return DigitalOutputPort.From(pin, this.IoController, initialState, initialOutputType);
        }

        public IDigitalInputPort CreateDigitalInputPort(
            IPin pin,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            double debounceDuration = 0.0,    // 0 - 1000 msec in .1 increments
            double glitchDuration = 0.0       // 0 - 1000 msec in .1 increments
            )
        {
            return DigitalInputPort.From(pin, this.IoController, interruptMode, resistorMode, debounceDuration, glitchDuration);
        }

        public IBiDirectionalPort CreateBiDirectionalPort(
            IPin pin,
            bool initialState = false,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input,
            double debounceDuration = 0.0,    // 0 - 1000 msec in .1 increments
            double glitchDuration = 0.0,      // 0 - 1000 msec in .1 increments
            OutputType outputType = OutputType.PushPull
            )
        {
            // Convert durations to unsigned int with 100 usec resolution
            return BiDirectionalPort.From(pin, this.IoController, initialState, interruptMode, resistorMode, initialDirection, debounceDuration, glitchDuration, outputType);
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

        /// <summary>
        /// Initializes a new instance of a legacy `ISerialPort`. `ISerialPort`
        /// is provided for legacy compatibility, we recommend using the more
        /// modern, thread-safe `ISerialMessagePort`.
        /// </summary>
        /// <param name="portName">The 'SerialPortName` of port to use.</param>
        /// <param name="baudRate">Speed, in bits per second, of the serial port.</param>
        /// <param name="parity">`Parity` enum describing what type of
        /// cyclic-redundancy-check (CRC) bit, if any, should be expected in the
        /// serial message frame. Default is `Parity.None`.</param>
        /// <param name="dataBits">Number of data bits expected in the serial
        /// message frame. Default is `8`.</param>
        /// <param name="stopBits">`StopBits` describing how many bits should be
        /// expected at the end of every character in the serial message frame.
        /// Default is `StopBits.One`.</param>
        /// <param name="readBufferSize">Size, in bytes, of the read buffer. Default
        /// is 1024.</param>
        /// <returns></returns>
        public ISerialPort CreateSerialPort(
            SerialPortName portName,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 1024)
        {
            return SerialPort.From(portName, baudRate, dataBits, parity, stopBits, readBufferSize);
        }

        /// <summary>
        /// Initializes a new instance of the `ISerialMessagePort` class that
        /// listens for serial messages defined byte[] message termination suffix.
        /// </summary>
        /// <param name="portName">The 'SerialPortName` of port to use.</param>
        /// <param name="suffixDelimiter">A `byte[]` of the delimiter(s) that
        /// denote the end of the message.</param>
        /// <param name="preserveDelimiter">Whether or not to preseve the
        /// delimiter tokens when passing the message to subscribers.</param>
        /// <param name="baudRate">Speed, in bits per second, of the serial port.</param>
        /// <param name="parity">`Parity` enum describing what type of
        /// cyclic-redundancy-check (CRC) bit, if any, should be expected in the
        /// serial message frame. Default is `Parity.None`.</param>
        /// <param name="dataBits">Number of data bits expected in the serial
        /// message frame. Default is `8`.</param>
        /// <param name="stopBits">`StopBits` describing how many bits should be
        /// expected at the end of every character in the serial message frame.
        /// Default is `StopBits.One`.</param>
        /// <param name="readBufferSize">Size, in bytes, of the read buffer. Default
        /// is 512.</param>
        /// <returns></returns>
        public ISerialMessagePort CreateSerialMessagePort(
            SerialPortName portName,
            byte[] suffixDelimiter,
            bool preserveDelimiter,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 512 )
        {
            return SerialMessagePort.From(portName,
                suffixDelimiter, preserveDelimiter, baudRate, dataBits, parity,
                stopBits, readBufferSize);
        }

        /// <summary>
        /// Initializes a new instance of the `ISerialMessagePort` class that
        /// listens for serial messages defined by a `byte[]` prefix, and a
        /// fixed length.
        /// </summary>
        /// <param name="portName">The 'SerialPortName` of port to use.</param>
        /// <param name="messageLength">Length of the message, not including the
        /// delimiter, to be parsed out of the incoming data.</param>
        /// <param name="prefixDelimiter">A `byte[]` of the delimiter(s) that
        /// denote the beginning of the message.</param>
        /// <param name="preserveDelimiter">Whether or not to preseve the
        /// delimiter tokens when passing the message to subscribers.</param>
        /// <param name="baudRate">Speed, in bits per second, of the serial port.</param>
        /// <param name="parity">`Parity` enum describing what type of
        /// cyclic-redundancy-check (CRC) bit, if any, should be expected in the
        /// serial message frame. Default is `Parity.None`.</param>
        /// <param name="dataBits">Number of data bits expected in the serial
        /// message frame. Default is `8`.</param>
        /// <param name="stopBits">`StopBits` describing how many bits should be
        /// expected at the end of every character in the serial message frame.
        /// Default is `StopBits.One`.</param>
        /// <param name="readBufferSize">Size, in bytes, of the read buffer. Default
        /// is 512.</param>
        public ISerialMessagePort CreateSerialMessagePort(
            SerialPortName portName,
            byte[] prefixDelimiter,
            bool preserveDelimiter,
            int messageLength,
            int baudRate = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int readBufferSize = 512)
        {
            return SerialMessagePort.From(portName,
                prefixDelimiter, preserveDelimiter, messageLength, baudRate,
                dataBits, parity, stopBits, readBufferSize);
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
