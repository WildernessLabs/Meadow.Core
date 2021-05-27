using Meadow.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            // ensure this is off in release (in case a dev sets it to true and fogets during check-in
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
            if (!mosi.Supports<SpiChannelInfo>(p => (p.LineTypes & SpiLineType.MOSI) != SpiLineType.None))
            {
                throw new NotSupportedException($"Pin {mosi.Name} does not support SPI MOSI capability");
            }
            if (!miso.Supports<SpiChannelInfo>(p => (p.LineTypes & SpiLineType.MISO) != SpiLineType.None))
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
                if(_clockConfig == null)
                {
                    Configuration = new SpiClockConfiguration(375, SpiClockConfiguration.Mode.Mode0);
                    return Configuration;
                }
                return _clockConfig;
            }
            internal set
            {
                if (value == null) { throw new ArgumentNullException(); }

                if(_clockConfig != null)
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
            var actual = SetFrequency(Configuration.SpeedKHz * 1000);
            // update the config with what we actually set so it's readable
            Configuration.SetActualSpeedKHz(actual / 1000);

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
        /// Writes data to the SPI bus
        /// </summary>
        /// <param name="chipSelect">IPin to use as the chip select to activate the bus (active-low)</param>
        /// <param name="data">Data to write</param>
        public void SendData(IDigitalOutputPort chipSelect, IEnumerable<byte> data)
        {
            SendData(chipSelect, ChipSelectMode.ActiveLow, data.ToArray());
        }

        /// <summary>
        /// Writes data to the SPI bus
        /// </summary>
        /// <param name="chipSelect">IPin to use as the chip select to activate the bus</param>
        /// <param name="csMode">Describes which level on the chip select activates the bus</param>
        /// <param name="data">Data to write</param>
        public void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, IEnumerable<byte> data)
        {
            SendData(chipSelect, csMode, data.ToArray());
        }

        /// <summary>
        /// Writes data to the SPI bus
        /// </summary>
        /// <param name="chipSelect">IPin to use as the chip select to activate the bus (active-low)</param>
        /// <param name="data">Data to write</param>
        public void SendData(IDigitalOutputPort chipSelect, params byte[] data)
        {
            SendData(chipSelect, ChipSelectMode.ActiveLow, data);
        }

        /// <summary>
        /// Writes data to the SPI bus
        /// </summary>
        /// <param name="chipSelect">IPin to use as the chip select to activate the bus</param>
        /// <param name="csMode">Describes which level on the chip select activates the bus</param>
        /// <param name="data">Data to write</param>
        public void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, params byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);

            _busSemaphore.Wait();
            Output.WriteLineIf(_showSpiDebug, $" +SendData");

            try
            {
                if (chipSelect != null)
                {
                    // activate the chip select
                    chipSelect.State = csMode == ChipSelectMode.ActiveLow ? false : true;
                }

                var command = new Nuttx.UpdSPIDataCommand()
                {
                    BufferLength = data.Length,
                    TxBuffer = gch.AddrOfPinnedObject(),
                    RxBuffer = IntPtr.Zero,
                    BusNumber = BusNumber
                };

                Output.WriteLineIf(_showSpiDebug, $" sending {data.Length} bytes: {BitConverter.ToString(data)}");
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
            finally
            {
                _busSemaphore.Release();

                if (gch.IsAllocated)
                {
                    gch.Free();
                }
                Output.WriteLineIf(_showSpiDebug, $" -SendData");
            }
        }

        /// <summary>
        /// Reads data from the SPI bus
        /// </summary>
        /// <param name="chipSelect">IPin to use as the chip select to activate the bus (active-low)</param>
        /// <param name="numberOfBytes">Number of bytes to read</param>
        public byte[] ReceiveData(IDigitalOutputPort chipSelect, int numberOfBytes)
        {
            return ReceiveData(chipSelect, ChipSelectMode.ActiveLow, numberOfBytes);
        }

        /// <summary>
        /// Reads data from the SPI bus
        /// </summary>
        /// <param name="chipSelect">IPin to use as the chip select to activate the bus</param>
        /// <param name="csMode">Describes which level on the chip select activates the bus</param>
        /// <param name="numberOfBytes">Number of bytes to read</param>
        public byte[] ReceiveData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, int numberOfBytes)
        {
            var rxBuffer = new byte[numberOfBytes];
            var gch = GCHandle.Alloc(rxBuffer, GCHandleType.Pinned);

            _busSemaphore.Wait();

            try
            {
                if (chipSelect != null)
                {
                    // activate the chip select
                    chipSelect.State = csMode == ChipSelectMode.ActiveLow ? false : true;
                }

                var command = new Nuttx.UpdSPIDataCommand()
                {
                    TxBuffer = IntPtr.Zero,
                    BufferLength = rxBuffer.Length,
                    RxBuffer = gch.AddrOfPinnedObject(),
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

                return rxBuffer;
            }
            finally
            {
                _busSemaphore.Release();

                if (gch.IsAllocated)
                {
                    gch.Free();
                }
            }
        }

        /// <summary>
        /// Does a data exchange on the SPI bus.
        /// </summary>
        /// <param name="chipSelect">IPin to use as the chip select to activate the bus</param>
        /// <param name="csMode">Describes which level on the chip select activates the bus</param>
        /// <param name="sendBuffer">Buffer holding data to be written</param>
        /// <param name="receiveBuffer">Buffer where the received data will be written</param>
        /// <returns></returns>
        /// <remarks>
        /// Due to the nature of a data exchange on a SPI bus, equal numbers of bytes must always be transmitted and received.  Both the sendBuffer and receiveBuffer must be of equal length and must be non-null.
        /// The <b>sendBuffer</b> data will start transmitting on the first clock cycle of the exchange.  If you want the output data to start transmitting on a later clock cycle, you must left-pad <b>dataToWrite</b> with a zero for each clock cycle to skip (it is not actually skipped, but a zero will be transmitted on those cycles).
        /// If you want to read more data that you are writing, you must right-pad the input parameter with enough empty bytes (zeros) to account for the desired return data.
        /// <paramref name="receiveBuffer"/>Note: <i>ExchangeData</i> pins both buffers during execution.  Cross-thread modifications to either of the buffers during execution will result in undefined behavior.</b>
        /// </remarks>
        public void ExchangeData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, byte[] sendBuffer, byte[] receiveBuffer)
        {
            ExchangeData(chipSelect, csMode, sendBuffer, receiveBuffer, sendBuffer.Length);
        }

        /// <summary>
        /// Does a data exchange on the SPI bus.
        /// </summary>
        /// <param name="chipSelect">IPin to use as the chip select to activate the bus</param>
        /// <param name="csMode">Describes which level on the chip select activates the bus</param>
        /// <param name="sendBuffer">Buffer holding data to be written</param>
        /// <param name="receiveBuffer">Buffer where the received data will be written</param>
        /// <param name="bytesToExchange">The number of bytes to exchange, between 1 and the length of the buffer parameters</param>
        /// <returns></returns>
        /// <remarks>
        /// Due to the nature of a data exchange on a SPI bus, equal numbers of bytes must always be transmitted and received.  Both the sendBuffer and receiveBuffer must be of equal length and must be non-null.
        /// The <b>sendBuffer</b> data will start transmitting on the first clock cycle of the exchange.  If you want the output data to start transmitting on a later clock cycle, you must left-pad <b>dataToWrite</b> with a zero for each clock cycle to skip (it is not actually skipped, but a zero will be transmitted on those cycles).
        /// If you want to read more data that you are writing, you must right-pad the input parameter with enough empty bytes (zeros) to account for the desired return data.
        /// <paramref name="receiveBuffer"/>Note: <i>ExchangeData</i> pins both buffers during execution.  Cross-thread modifications to either of the buffers during execution will result in undefined behavior.</b>
        /// </remarks>
        public void ExchangeData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, byte[] sendBuffer, byte[] receiveBuffer, int bytesToExchange)
        {
            if (sendBuffer == null) throw new ArgumentNullException("A non-null sendBuffer is required");
            if (receiveBuffer == null) throw new ArgumentNullException("A non-null receiveBuffer is required");
            if (sendBuffer.Length != receiveBuffer.Length) throw new Exception("Both buffers must be equal size");

            GCHandle rxGch = default(GCHandle);
            GCHandle txGch = default(GCHandle);

            _busSemaphore.Wait();

            try
            {
                txGch = GCHandle.Alloc(sendBuffer, GCHandleType.Pinned);
                rxGch = GCHandle.Alloc(receiveBuffer, GCHandleType.Pinned);

                if (chipSelect != null)
                {
                    // activate the chip select
                    chipSelect.State = csMode == ChipSelectMode.ActiveLow ? false : true;
                }

                var command = new Nuttx.UpdSPIDataCommand()
                {
                    BufferLength = bytesToExchange,
                    TxBuffer = txGch.AddrOfPinnedObject(),
                    RxBuffer = rxGch.AddrOfPinnedObject(),
                    BusNumber = BusNumber
                };

                Output.WriteLineIf(_showSpiDebug, "+Exchange");
                Output.WriteLineIf(_showSpiDebug, $" Sending {sendBuffer.Length} bytes");
                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SPIData, ref command);
                if (result != 0)
                {
                    DecipherSPIError(UPD.GetLastError());
                }
                Output.WriteLineIf(_showSpiDebug, $" Received {receiveBuffer.Length} bytes");

                if (chipSelect != null)
                {
                    // deactivate the chip select
                    chipSelect.State = csMode == ChipSelectMode.ActiveLow ? true : false;
                }
            }
            finally
            {
                _busSemaphore.Release();

                if (rxGch.IsAllocated)
                {
                    rxGch.Free();
                }
                if (txGch.IsAllocated)
                {
                    txGch.Free();
                }
            }
        }

        /// <summary>
        /// Gets an array of all of the speeds (in kHz) that the SPI bus supports.
        /// </summary>
        public long[] SupportedSpeeds
        {
            get => new long[]
                {
                    375,
                    750,
                    1500,
                    3000,
                    6000,
                    12000,
                    24000,
                    48000
                };
        }

        private void SetBitsPerWord(int bitsPerWord)
        {
            if(bitsPerWord < 4 || bitsPerWord > 16)
            {
                throw new ArgumentOutOfRangeException();
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
                throw new NativeException(error);
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

        private long SetFrequency(long desiredSpeed)
        {
            // TODO: move this to the F7
            var speed = GetSupportedSpeed(desiredSpeed);

            var command = new Nuttx.UpdSPISpeedCommand()
            {
                BusNumber = BusNumber,
                Frequency = speed
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

        private long GetSupportedSpeed(long desiredSpeed)
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

            var clockSpeed = 96000000L;
            var divisor = 2;
            while (divisor <= 256)
            {
                var test = clockSpeed / divisor;
                if (desiredSpeed >= test)
                {
                    return test;
                }
                divisor *= 2;
            }
            // return the slowest rate
            return clockSpeed / 256;
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
                default:
                    throw new NativeException($"Communication error.  Error code {(int)ec}");
            }
        }
    }
}