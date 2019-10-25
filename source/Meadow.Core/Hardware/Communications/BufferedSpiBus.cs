using Meadow.Devices;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using static Meadow.Core.Interop;

namespace Meadow.Hardware
{
    public class BufferedSpiBus : IDisposable
    {
        private bool _showSpiDebug = false;
        private SemaphoreSlim _busSemaphore = new SemaphoreSlim(1, 1);
        private SpiClockConfiguration _clockConfig = new SpiClockConfiguration();

        private readonly byte[] _txBuffer;
        private readonly byte[] _rxBuffer;
        private GCHandle _rxGch;
        private GCHandle _txGch;

        internal int BusNumber { get; set; }
        public byte[] TxBuffer => _txBuffer;
        public byte[] RxBuffer => _rxBuffer;

        public BufferedSpiBus(
            IPin clock,
            IPin mosi,
            IPin miso, 
            SpiClockConfiguration config,
            int bufferSize = 0x10000)
        {
            _txBuffer = new byte[bufferSize];
            _txGch = GCHandle.Alloc(_txBuffer, GCHandleType.Pinned);
            _rxBuffer = new byte[bufferSize];
            _rxGch = GCHandle.Alloc(_rxBuffer, GCHandleType.Pinned);

            Configuration = config;

            BusNumber = GetSpiBusNumberForPinName(clock);
        }

        public void Dispose()
        {
            if (_txGch.IsAllocated)
            {
                _txGch.Free();
            }
            if (_rxGch.IsAllocated)
            {
                _rxGch.Free();
            }
        }


        private int GetSpiBusNumberForPinName(IPin clock)
        {
            // we're only looking at clock pin.  
            // For the F7 meadow it's enough to know and any attempt to use other pins will get caught by other sanity checks
            if (clock.Name == "ESP_CLK")
            {
                return 2;
            }

            return 3;
        }

        /// <summary>
        /// Configuration to use for this instance of the SPIBus.
        /// </summary>
        public SpiClockConfiguration Configuration
        {
            get => _clockConfig;
            internal set
            {
                if (value == null) { throw new ArgumentNullException(); }

                if (value.SpeedKHz != Configuration.SpeedKHz)
                {
                    SetFrequency(value.SpeedKHz * 1000);
                    Configuration.SpeedKHz = value.SpeedKHz;
                }

                var modeChange = false;

                if (value.Polarity != Configuration.Polarity ||
                        value.Phase != Configuration.Phase)
                {
                    modeChange = true;
                }

                if (modeChange)
                {
                    int mode = 0;

                    switch (value.Phase)
                    {
                        case SpiClockConfiguration.ClockPhase.Zero:
                            mode = (value.Polarity == SpiClockConfiguration.ClockPolarity.Normal) ? 0 : 2;
                            break;
                        case SpiClockConfiguration.ClockPhase.One:
                            mode = (value.Polarity == SpiClockConfiguration.ClockPolarity.Normal) ? 1 : 3;
                            break;
                    }

                    SetMode(mode);
                }

                _clockConfig = value;
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

        public void SetMode(int mode)
        {
            Console.WriteLine($"SetMode {mode}");

            var command = new Nuttx.UpdSPIModeCommand()
            {
                BusNumber = BusNumber,
                Mode = mode
            };

            Output.WriteLineIf(_showSpiDebug, "+SetMode");
            Output.WriteLineIf(_showSpiDebug, $" setting bus {command.BusNumber} mode to {command.Mode}");
            var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SPIMode, ref command);
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

        public void ExchangeData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, int bytesToExchange)
        {
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
                    BufferLength = bytesToExchange,
                    TxBuffer = _txGch.AddrOfPinnedObject(),
                    RxBuffer = _rxGch.AddrOfPinnedObject(),
                    BusNumber = BusNumber
                };

                Output.WriteLineIf(_showSpiDebug, "+ExchangeData");
                Output.WriteLineIf(_showSpiDebug, $" Sending {bytesToExchange} bytes");
                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SPIData, ref command);
                Output.WriteLineIf(_showSpiDebug, $" Received {bytesToExchange} bytes");

                if (chipSelect != null)
                {
                    // deactivate the chip select
                    chipSelect.State = csMode == ChipSelectMode.ActiveLow ? true : false;
                }
            }
            finally
            {
                _busSemaphore.Release();
            }
        }
    }
}