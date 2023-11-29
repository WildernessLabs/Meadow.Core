using Meadow.Devices;
using System;
using System.Threading;
using static Meadow.Core.Interop;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents an SPI communication bus for communicating to peripherals that 
    /// implement the SPI protocol.
    /// </summary>
    public partial class SpiBus : ISpiBus
    {
        private bool _showSpiDebug = false;
        private SemaphoreSlim _busSemaphore = new SemaphoreSlim(1, 1);
        private SpiClockConfiguration? _clockConfig;

        internal int BusNumber { get; set; }

        /// <summary>
        /// Default constructor for the SPIBus.
        /// </summary>
        /// <remarks>
        /// This is private to prevent the programmer using it.
        /// </remarks>
        protected SpiBus()
        {
#if !DEBUG
            // ensure this is off in release (in case a dev sets it to true and forgets during check-in
            _showSpiDebug = false;
#endif
        }

        internal static SpiBus From(
            IPin clock,
            IPin mosi,
            IPin miso)
        {
            // check for pin compatibility and availability
            if (!clock.Supports<SpiChannelInfo>(p => (p.LineTypes & SpiLineType.Clock) != SpiLineType.None))
            {
                throw new NotSupportedException($"Pin {clock.Name} does not support SPI Clock capability");
            }
            if (!mosi.Supports<SpiChannelInfo>(p => (p.LineTypes & SpiLineType.COPI) != SpiLineType.None))
            {
                throw new NotSupportedException($"Pin {mosi.Name} does not support SPI MOSI capability");
            }
            if (!miso.Supports<SpiChannelInfo>(p => (p.LineTypes & SpiLineType.CIPO) != SpiLineType.None))
            {
                throw new NotSupportedException($"Pin {miso.Name} does not support SPI MISO capability");
            }

            // we can't set the speed here yet because the caller has to set the bus number first
            return new SpiBus();
        }

        /// <summary>
        /// Configuration to use for this instance of the SPIBus.
        /// </summary>
        public SpiClockConfiguration Configuration
        {
            get
            {
                if (_clockConfig == null)
                {
                    Configuration = new SpiClockConfiguration(
                        new Units.Frequency(375, Units.Frequency.UnitType.Kilohertz),
                        SpiClockConfiguration.Mode.Mode0);

                    return Configuration;
                }
                return _clockConfig;
            }
            internal set
            {
                if (value == null) { throw new ArgumentNullException(); }

                if (_clockConfig != null)
                {
                    _clockConfig.Changed -= OnConfigChanged;
                }

                _clockConfig = value;

                HandleConfigChange();

                _clockConfig.Changed += OnConfigChanged;
            }
        }

        private void OnConfigChanged(object sender, EventArgs e)
        {
            HandleConfigChange();
        }

        private void HandleConfigChange()
        {
            // try setting the clock frequency.  Actual frequency comes back (based on clock divisor)
            var actual = SetFrequency(Configuration.Speed);
            // update the config with what we actually set so it's readable
            Configuration.SetActualSpeed(actual);

            int mode = 0;

            switch (Configuration.Phase)
            {
                case SpiClockConfiguration.ClockPhase.Zero:
                    mode = (Configuration.Polarity == SpiClockConfiguration.ClockPolarity.Normal) ? 0 : 2;
                    break;
                case SpiClockConfiguration.ClockPhase.One:
                    mode = (Configuration.Polarity == SpiClockConfiguration.ClockPolarity.Normal) ? 1 : 3;
                    break;
            }

            SetMode(mode);
            SetBitsPerWord(Configuration.BitsPerWord);
        }

        /// <summary>
        /// Reads data from the SPI bus into the buffer.
        /// </summary>
        /// <param name="chipSelect">Port to use as the chip select to activate the bus.</param>
        /// <param name="readBuffer">Data to write</param>
        /// <param name="csMode">Describes which level on the chip select activates the peripheral.</param>
        public unsafe void Read(
            IDigitalOutputPort? chipSelect,
            Span<byte> readBuffer,
            ChipSelectMode csMode = ChipSelectMode.ActiveLow)
        {
            _busSemaphore.Wait();

            try
            {
                if (chipSelect != null)
                {
                    // activate the chip select
                    chipSelect.State = csMode == ChipSelectMode.ActiveLow ? false : true;
                }

                fixed (byte* pRead = readBuffer)
                {
                    var command = new Nuttx.UpdSPIDataCommand()
                    {
                        TxBuffer = IntPtr.Zero,
                        BufferLength = readBuffer.Length,
                        RxBuffer = (IntPtr)pRead,
                        BusNumber = BusNumber
                    };

                    var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SPIData, ref command);
                    if (result != 0)
                    {
                        DecipherSPIError(UPD.GetLastError());
                    }

                    if (chipSelect != null)
                    {
                        // deactivate the chip select
                        chipSelect.State = csMode == ChipSelectMode.ActiveLow ? true : false;
                    }
                }
            }
            finally
            {
                _busSemaphore.Release();
            }
        }

        /// <summary>
        /// Writes data to the SPI bus
        /// </summary>
        /// <param name="chipSelect">IPin to use as the chip select to activate the bus</param>
        /// <param name="writeBuffer">Data to write</param>
        /// <param name="csMode">Describes which level on the chip select activates the bus</param>
        public unsafe void Write(
            IDigitalOutputPort? chipSelect,
            Span<byte> writeBuffer,
            ChipSelectMode csMode = ChipSelectMode.ActiveLow)
        {
            _busSemaphore.Wait();
            Output.WriteLineIf(_showSpiDebug, $" +SendData");

            try
            {
                if (chipSelect != null)
                {
                    // activate the chip select
                    chipSelect.State = csMode == ChipSelectMode.ActiveLow ? false : true;
                }

                fixed (byte* pWrite = writeBuffer)
                {
                    var command = new Nuttx.UpdSPIDataCommand()
                    {
                        BufferLength = writeBuffer.Length,
                        TxBuffer = (IntPtr)pWrite,
                        RxBuffer = IntPtr.Zero,
                        BusNumber = BusNumber
                    };

                    Output.WriteLineIf(_showSpiDebug, $" sending {writeBuffer.Length} bytes: {BitConverter.ToString(writeBuffer.ToArray())}");
                    var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SPIData, ref command);
                    if (result != 0)
                    {
                        DecipherSPIError(UPD.GetLastError());
                    }
                    Output.WriteLineIf(_showSpiDebug, $" send complete");

                    if (chipSelect != null)
                    {
                        // deactivate the chip select
                        chipSelect.State = csMode == ChipSelectMode.ActiveLow ? true : false;
                    }
                }
            }
            finally
            {
                _busSemaphore.Release();
                Output.WriteLineIf(_showSpiDebug, $" -SendData");
            }
        }

        /// <summary>
        /// Writes data from the write buffer to a peripheral on the bus while
        /// at the same time reading return data into the read buffer.
        /// </summary>
        /// <param name="chipSelect">Port to use as the chip select to activate the peripheral.</param>
        /// <param name="writeBuffer">Buffer to read data from.</param>
        /// <param name="readBuffer">Buffer to read returning data into.</param>
        /// <param name="csMode">Describes which level on the chip select activates the peripheral.</param>
        public unsafe void Exchange(
            IDigitalOutputPort? chipSelect,
            Span<byte> writeBuffer, Span<byte> readBuffer,
            ChipSelectMode csMode = ChipSelectMode.ActiveLow)
        {
            if (writeBuffer == null) throw new ArgumentNullException("A non-null sendBuffer is required");
            if (readBuffer == null) throw new ArgumentNullException("A non-null receiveBuffer is required");
            if (writeBuffer.Length != readBuffer.Length) throw new Exception("Both buffers must be equal size");

            _busSemaphore.Wait();

            try
            {
                if (chipSelect != null)
                {
                    // activate the chip select
                    chipSelect.State = csMode == ChipSelectMode.ActiveLow ? false : true;
                }

                fixed (byte* pWrite = writeBuffer)
                fixed (byte* pRead = readBuffer)
                {
                    var command = new Nuttx.UpdSPIDataCommand()
                    {
                        BufferLength = readBuffer.Length,
                        TxBuffer = (IntPtr)pWrite,
                        RxBuffer = (IntPtr)pRead,
                        BusNumber = BusNumber
                    };

                    Output.WriteLineIf(_showSpiDebug, "+Exchange");
                    Output.WriteLineIf(_showSpiDebug, $" Sending {writeBuffer.Length} bytes");
                    var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SPIData, ref command);
                    if (result != 0)
                    {
                        DecipherSPIError(UPD.GetLastError());
                    }
                    Output.WriteLineIf(_showSpiDebug, $" Received {readBuffer.Length} bytes");

                    if (chipSelect != null)
                    {
                        // deactivate the chip select
                        chipSelect.State = csMode == ChipSelectMode.ActiveLow ? true : false;
                    }
                }
            }
            finally
            {
                _busSemaphore.Release();
            }
        }

        /// <summary>
        /// Gets an array of all of the speeds that the SPI bus supports.
        /// </summary>
        public Units.Frequency[] SupportedSpeeds
        {
            get => new Units.Frequency[]
                {
                    new Units.Frequency(375, Units.Frequency.UnitType.Kilohertz),
                    new Units.Frequency(750, Units.Frequency.UnitType.Kilohertz),
                    new Units.Frequency(1500, Units.Frequency.UnitType.Kilohertz),
                    new Units.Frequency(3000, Units.Frequency.UnitType.Kilohertz),
                    new Units.Frequency(6000, Units.Frequency.UnitType.Kilohertz),
                    new Units.Frequency(12000, Units.Frequency.UnitType.Kilohertz),
                    new Units.Frequency(24000, Units.Frequency.UnitType.Kilohertz),
                    new Units.Frequency(48000, Units.Frequency.UnitType.Kilohertz)
                };
        }

        private void SetBitsPerWord(int bitsPerWord)
        {
            if (bitsPerWord < 4 || bitsPerWord > 16)
            {
                throw new ArgumentOutOfRangeException(nameof(bitsPerWord), bitsPerWord, null);
            }
            var command = new Nuttx.UpdSPIBitsCommand()
            {
                BusNumber = BusNumber,
                BitsPerWord = bitsPerWord
            };

            Output.WriteLineIf(_showSpiDebug, $" setting bus {command.BusNumber} bits per word to {command.BitsPerWord}");
            var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SPIBits, ref command);
            if (result != 0)
            {
                var error = UPD.GetLastError();
                throw new NativeException(error.ToString());
            }
        }

        private void SetMode(int mode)
        {
            var command = new Nuttx.UpdSPIModeCommand()
            {
                BusNumber = BusNumber,
                Mode = mode
            };

            Output.WriteLineIf(_showSpiDebug, "+SetMode");
            Output.WriteLineIf(_showSpiDebug, $" setting bus {command.BusNumber} mode to {command.Mode}");
            var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SPIMode, ref command);
            if (result != 0)
            {
                DecipherSPIError(UPD.GetLastError());
            }
            Output.WriteLineIf(_showSpiDebug, $" mode set to {mode}");
        }

        private Units.Frequency SetFrequency(Units.Frequency desiredSpeed)
        {
            // TODO: move this to the F7
            var speed = GetSupportedSpeed(desiredSpeed);

            var command = new Nuttx.UpdSPISpeedCommand()
            {
                BusNumber = BusNumber,
                Frequency = Convert.ToInt64(speed.Hertz)
            };

            Output.WriteLineIf(_showSpiDebug, "+SetFrequency");
            Output.WriteLineIf(_showSpiDebug, $" setting bus {command.BusNumber} speed to {command.Frequency}");
            var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SPISpeed, ref command);
            if (result != 0)
            {
                DecipherSPIError(UPD.GetLastError());
            }
            Output.WriteLineIf(_showSpiDebug, $" speed set to {desiredSpeed}");

            return speed;
        }

        private Units.Frequency GetSupportedSpeed(Units.Frequency desiredSpeed)
        {
            /*
             * Meadow's STM32 uses a clock divisor from the PCLK2 for speed.  
             * PCLK2 (at the time of writing) is 96MHz and max SPI speed is PCLK2/2
            48
            24
            12
            6
            3
            1.5
            0.75
            0.375
            */

            var ds = Convert.ToInt64(desiredSpeed.Hertz);

            var clockSpeed = 96000000L;
            var divisor = 2;
            while (divisor <= 256)
            {
                var test = clockSpeed / divisor;
                if (ds >= test)
                {
                    return new Units.Frequency(test, Units.Frequency.UnitType.Hertz);
                }
                divisor *= 2;
            }
            // return the slowest rate
            return new Units.Frequency(clockSpeed / 256, Units.Frequency.UnitType.Hertz);
        }

        private uint SpeedToDivisor(long speed)
        {
            var clockSpeed = 96000000L;
            var divisor = clockSpeed / speed;
            for (int i = 0; i <= 7; i++)
            {
                if ((2 << i) == divisor)
                {
                    return (uint)i;
                }
            }

            return 0;
        }

        private void DecipherSPIError(Nuttx.ErrorCode ec)
        {
            // This is highly unlikely to ever get called.  The underlying Nuttx SPI driver does no error checking and therefore is incapable of returning error codes. Yay.
            switch (ec)
            {
                case Nuttx.ErrorCode.NoSuchFileOrDirectory: // 2
                    throw new NativeException($"Invalid or unsupported bus.  (Error code {(int)ec})");
                default:
                    throw new NativeException($"Communication error.  Error code {(int)ec}");
            }
        }
    }
}