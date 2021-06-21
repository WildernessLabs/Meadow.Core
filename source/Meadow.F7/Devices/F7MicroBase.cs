using System;
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
    public abstract class F7MicroBase : IF7MeadowDevice
    {
        private SynchronizationContext? _context;
        protected Esp32Coprocessor? esp32;

        public IBluetoothAdapter? BluetoothAdapter { get; protected set; }
        public IWiFiAdapter? WiFiAdapter { get; protected set; }
        public ICoprocessor? Coprocessor { get; protected set; }

        public event EventHandler WiFiAdapterInitialized = delegate { };

        /// <summary>
        /// The default resolution for analog inputs
        /// </summary>
        // TODO: should this be public?
        public const int DefaultA2DResolution = 12;

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

        public Task<bool> InitWiFiAdapter()
        {
            return InitCoprocessor();
        }

        public Task<bool> InitBluetoothAdapter()
        {
            return InitCoprocessor();
        }

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

        /// <summary>
        /// Creates an `IAnalogInputPort` on the given pin. 
        /// </summary>
        /// <param name="pin">The analog input capable `IPin` on which to create the input port.</param>
        /// <param name="voltageReference">Reference voltage, in Volts, of the maximum input value. Default is `3.3V`.</param>
        /// <param name="updateInterval">A `TimeSpan` that specifies how long to
        /// wait between readings. This value influences how often `*Updated`
        /// events are raised and `IObservable` consumers are notified. Default is `5` seconds.</param>
        /// <param name="sampleCount">Number of samples to take per reading. If > `1` then the port will
        /// take multiple readings and These are automatically averaged to
        /// reduce noise, a process known as _oversampling_. Default is `5` samples.</param>
        /// <param name="sampleInterval">Duration in between samples when oversampling. Default is `40ms`.</param>
        /// <returns></returns>
        public IAnalogInputPort CreateAnalogInputPort(
            IPin pin,
            int sampleCount = 5,
            int sampleIntervalMs = 40,
            float voltageReference = IAnalogInputController.DefaultA2DReferenceVoltage)
        {
            return AnalogInputPort.From(
                pin, this.IoController,
                sampleCount, sampleIntervalMs,
                voltageReference);
        }

        public IPwmPort CreatePwmPort(
            IPin pin,
            float frequency = IPwmOutputController.DefaultPwmFrequency,
            float dutyCycle = IPwmOutputController.DefaultPwmDutyCycle,
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
            // HACK NOTE: can't compare directly here, so we're comparing the name.
            // might be able to cast and compare?
            return (
                pin.Name == Pins.OnboardLedBlue.Name ||
                pin.Name == Pins.OnboardLedGreen.Name ||
                pin.Name == Pins.OnboardLedRed.Name
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
            int readBufferSize = 512)
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
        /// Creates a SPI bus instance for the requested bus speed with the Meadow- default IPins for CLK, COPI and CIPO
        /// </summary>
        /// <param name="speedkHz">The bus speed (in kHz)</param>
        /// <returns>An instance of an IISpiBus</returns>
        public ISpiBus CreateSpiBus(
            long speedkHz = IMeadowDevice.DefaultSpiBusSpeed
        )
        {
            return CreateSpiBus(Pins.SCK, Pins.COPI, Pins.CIPO, speedkHz);
        }

        /// <summary>
        /// Creates a SPI bus instance for the requested control pins and bus speed
        /// </summary>
        /// <param name="pins">IPin instances used for (in this order) CLK, COPI, CIPO</param>
        /// <param name="speedkHz">The bus speed (in kHz)</param>
        /// <returns>An instance of an IISpiBus</returns>
        public ISpiBus CreateSpiBus(
            IPin[] pins,
            long speedkHz = IMeadowDevice.DefaultSpiBusSpeed
        )
        {
            return CreateSpiBus(pins[0], pins[1], pins[2], speedkHz);
        }

        /// <summary>
        /// Creates a SPI bus instance for the requested control pins and bus speed
        /// </summary>
        /// <param name="clock">The IPin instance to use as the bus clock</param>
        /// <param name="copi">The IPin instance to use for data transmit (controller out/peripheral in)</param>
        /// <param name="cipo">The IPin instance to use for data receive (controller in/peripheral out)</param>
        /// <param name="speedkHz">The bus speed (in kHz)</param>
        /// <returns>An instance of an IISpiBus</returns>
        public ISpiBus CreateSpiBus(
            IPin clock,
            IPin copi,
            IPin cipo,
            long speedkHz = IMeadowDevice.DefaultSpiBusSpeed
        )
        {
            var bus = SpiBus.From(clock, copi, cipo);
            bus.BusNumber = GetSpiBusNumberForPins(clock, copi, cipo);
            bus.Configuration.SpeedKHz = speedkHz;
            return bus;
        }

        /// <summary>
        /// Creates a SPI bus instance for the requested control pins and bus speed
        /// </summary>
        /// <param name="clock">The IPin instance to use as the bus clock</param>
        /// <param name="copi">The IPin instance to use for data transmit (controller out/peripheral in)</param>
        /// <param name="cipo">The IPin instance to use for data receive (controller in/peripheral out)</param>
        /// <param name="config">The bus clock configuration parameters</param>
        /// <returns>An instance of an IISpiBus</returns>
        public ISpiBus CreateSpiBus(
            IPin clock,
            IPin copi,
            IPin cipo,
            SpiClockConfiguration config
        )
        {
            var bus = SpiBus.From(clock, copi, cipo);
            bus.BusNumber = GetSpiBusNumberForPins(clock, copi, cipo);
            bus.Configuration = config;
            return bus;
        }

        protected int GetSpiBusNumberForPins(IPin clock, IPin copi, IPin cipo)
        {
            // we're only looking at clock pin.  
            // For the F7 meadow it's enough to know and any attempt to use other pins will get caught by other sanity checks
            // HACK NOTE: can't compare directly here, so we're comparing the name.
            // might be able to cast and compare?
            if (clock.Name == (Pins as IF7MicroPinout)?.ESP_CLK.Name) {
                return 2;
            } else if (clock.Name == (Pins as IF7MicroPinout)?.SCK.Name) {
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
        public II2cBus CreateI2cBus(int busNumber = 0)
        {
            return CreateI2cBus(busNumber, IMeadowDevice.DefaultI2cBusSpeed);
        }

        /// <summary>
        /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed
        /// </summary>
        /// <param name="frequencyHz">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            I2cBusSpeed busSpeed,
            int busNumber = 0
        )
        {
            return CreateI2cBus(Pins.I2C_SCL, Pins.I2C_SDA, new Frequency((int)busSpeed, Frequency.UnitType.Hertz));
        }

        /// <summary>
        /// Creates an I2C bus instance for the default Meadow F7 pins (SCL/D08 and SDA/D07) and the requested bus speed
        /// </summary>
        /// <param name="frequencyHz">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            int busNumber,
            Frequency frequency
        )
        {
            return CreateI2cBus(Pins.I2C_SCL, Pins.I2C_SDA, frequency);
        }

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequencyHz">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            IPin[] pins,
            Frequency frequency
        )
        {
            return CreateI2cBus(pins[0], pins[1], frequency);
        }

        /// <summary>
        /// Creates an I2C bus instance for the requested pins and bus speed
        /// </summary>
        /// <param name="frequencyHz">The bus speed in (in Hz) defaulting to 100k</param>
        /// <returns>An instance of an I2cBus</returns>
        public II2cBus CreateI2cBus(
            IPin clock,
            IPin data,
            Frequency frequency
        )
        {
            return I2cBus.From(this.IoController, clock, data, frequency);
        }

        public void SetClock(DateTime dateTime)
        {
            var ts = new Core.Interop.Nuttx.timespec {
                tv_sec = new DateTimeOffset(dateTime).ToUnixTimeSeconds()
            };

            Core.Interop.Nuttx.clock_settime(Core.Interop.Nuttx.clockid_t.CLOCK_REALTIME, ref ts);
        }

        // TODO: this should move to the MeadowOS class.
        public void SetSynchronizationContext(SynchronizationContext context)
        {
            _context = context;
        }

        // TODO: this should move to the MeadowOS class.
        public void BeginInvokeOnMainThread(Action action)
        {
            if (_context == null) {
                action();
            } else {
                _context.Send(delegate { action(); }, null);
            }
        }

        /// <summary>
        /// Check if the coprocessor is available / ready and throw an exception if it
        /// has not been setup.
        /// </summary>
        protected bool IsCoprocessorInitialized()
        {
            if (esp32 == null) {
                return false;
            } else {
                return true;
            }
        }

        //==== antenna stuff

        /// <summary>
        /// Get the currently setlected WiFi antenna.
        /// </summary>
        public AntennaType CurrentAntenna {
            get {
                if (WiFiAdapter != null) {
                    return WiFiAdapter.Antenna;
                } else {
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
            if (WiFiAdapter != null) {
                WiFiAdapter.SetAntenna(antenna, persist);
            } else {
                throw new Exception("Coprocessor not initialized.");
            }
        }


        //TODO: need the Read()/StartUpdating()/StopUpdating() pattern here.
        /// <summary>
        /// Gets the current battery charge level in voltage.
        /// </summary>
        public Voltage GetBatteryLevel()
        {
            if (Coprocessor != null) {
                return (new Voltage(Coprocessor.GetBatteryLevel(), Voltage.UnitType.Volts));
            } else {
                throw new Exception("Coprocessor not initialized.");
            }
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

        /// <summary>
        /// Enables a watchdog timer with the specified timeout in milliseconds.
        /// If Watchdog.Reset is not called before the timeout period, the Meadow
        /// will reset.
        /// </summary>
        /// <param name="timeoutMs">Watchdog timeout period, in milliseconds.
        /// Maximum allowed timeout of 32,768ms</param>
        public void WatchdogEnable(TimeSpan timeout)
        {
            // we'll use the IWDG registers.  
            // WWDG is too fast, with a max timeout in the 40ms range, which for a managed app is a recipe for lots of restarts
            // the IWDG uses the LSI clock, which has a freq of 32000
            // if we set the prescaler divider at /32, we end up with a simple 1ms per tick

            // from the data sheet:
            //1. Enable the IWDG by writing 0x0000 CCCC in the Key register (IWDG_KR).
            //2. Enable register access by writing 0x0000 5555 in the Key register (IWDG_KR).
            //3. Write the prescaler by programming the Prescaler register (IWDG_PR) from 0 to 7.
            //4. Write the Reload register (IWDG_RLR).
            //5. Wait for the registers to be updated (IWDG_SR = 0x0000 0000).
            //6. Refresh the counter value with IWDG_RLR (IWDG_KR = 0x0000 AAAA)

            // The RLR can have a maximum of 4096, so we have to play with the prescaler to get close to the desired value
            uint prescale, rlr;

            var timeoutMs = (int)timeout.TotalMilliseconds;

            if (timeoutMs < 4096)
            {
                prescale = STM32.IWDG_PR_DIV_32;
                rlr = (uint)timeoutMs;
            }
            else if (timeoutMs < 4096 * 2)
            {
                prescale = STM32.IWDG_PR_DIV_64;
                rlr = (uint)timeoutMs / 2;
            }
            else if (timeoutMs < 4096 * 4)
            {
                prescale = STM32.IWDG_PR_DIV_128;
                rlr = (uint)timeoutMs / 4;
            }
            else if (timeoutMs < 4096 * 8)
            {
                prescale = STM32.IWDG_PR_DIV_256;
                rlr = (uint)timeoutMs / 8;
            }
            else
            {
                throw new ArgumentOutOfRangeException($"Timeout must be less than {4096 * 8}ms");
            }

            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_KR_OFFSET, 0x0000cccc);
            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_KR_OFFSET, 0x00005555);
            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_PR_OFFSET, prescale);
            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_RLR_OFFSET, rlr);
        }

        public void WatchdogReset()
        {
            UPD.SetRegister(STM32.IWDG_BASE + STM32.IWDG_KR_OFFSET, 0x0000aaaa);
        }
    }
}
