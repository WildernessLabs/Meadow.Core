using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace Meadow
{
    public partial class SpiBus : ISpiBus, IDisposable
    {
        private SemaphoreSlim _busSemaphore = new SemaphoreSlim(1, 1);

        private const int TransferIoctl = 0;
        private const int ModeIoctl = 1;
        private const int JustificationIoctl = 2;
        private const int LengthIoctl = 3;
        private const int SpeedIoctl = 4;

        [Flags]
        public enum SpiMode
        {
            CPHA = 0x01,
            CPOL = 0x02,
            Mode0 = 0 | 0,
            Mode1 = 0 | CPHA,
            Mode2 = CPOL | 0,
            Mode3 = CPOL | CPHA
        }

        private int DriverHandle { get; set; }

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

        public SpiClockConfiguration Configuration { get; private set; }

        internal unsafe SpiBus(int chipSelect, SpiClockConfiguration.ClockPhase phase, SpiClockConfiguration.ClockPolarity polarity, Units.Frequency speed)
            : this(chipSelect, (SpiMode)((int)phase | (int)polarity), speed)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chipSelect">Must be 0 or 1</param>
        internal unsafe SpiBus(int chipSelect, SpiMode mode, Units.Frequency speed)
        {
            var bitsPerWord = 8;
            var busNumber = 0;

            var driver = $"/dev/spidev{busNumber}.{chipSelect}";
            DriverHandle = Interop.open(driver, Interop.DriverFlags.O_RDWR);
            if (DriverHandle < 0)
            {
                // TODO: maybe try modprobe here?
                throw new NativeException($"Unable to open driver {driver}. Check your user permissions", Marshal.GetLastWin32Error());
            }

            var status = Interop.ioctl(DriverHandle, Interop._IOW('k', ModeIoctl, 8), (byte*)&mode);
            if (status < 0)
            {
                throw new NativeException($"Could not set SPIMode (WR): {status}", Marshal.GetLastWin32Error());
            }

            status = Interop.ioctl(DriverHandle, Interop._IOR('k', ModeIoctl, 8), (byte*)&mode);
            if (status < 0)
            {
                throw new NativeException($"Could not set SPIMode (RD): {status}", Marshal.GetLastWin32Error());
            }

            status = Interop.ioctl(DriverHandle, Interop._IOW('k', LengthIoctl, 8), (byte*)&bitsPerWord);
            if (status < 0)
            {
                throw new NativeException($"Could not set SPI bits per word (WR): {status}", Marshal.GetLastWin32Error());
            }

            status = Interop.ioctl(DriverHandle, Interop._IOR('k', LengthIoctl, 8), (byte*)&bitsPerWord);
            if (status < 0)
            {
                throw new NativeException($"Could not set SPI bits per word (RD): {status}", Marshal.GetLastWin32Error());
            }

            int speedHz = Convert.ToInt32(speed.Hertz);
            status = Interop.ioctl(DriverHandle, Interop._IOW('k', LengthIoctl, 32), (byte*)&speedHz);
            if (status < 0)
            {
                throw new NativeException($"Could not set SPI speed (WR): {status}", Marshal.GetLastWin32Error());
            }

            status = Interop.ioctl(DriverHandle, Interop._IOR('k', LengthIoctl, 32), (byte*)&speedHz);
            if (status < 0)
            {
                throw new NativeException($"Could not set SPI speed (RD): {status}", Marshal.GetLastWin32Error());
            }

            Configuration = new SpiClockConfiguration(speed)
            {
                Phase = SpiClockConfiguration.ClockPhase.Zero,
                Polarity = SpiClockConfiguration.ClockPolarity.Normal
            };
        }

        public void Dispose()
        {
            if (DriverHandle > 0)
            {
                Interop.close(DriverHandle);
                DriverHandle = 0;
            }
        }

        public void Read(IDigitalOutputPort? chipSelect, Span<byte> readBuffer, ChipSelectMode csMode = ChipSelectMode.ActiveLow)
        {
            byte[] writeBuffer = new byte[readBuffer.Length];
            Exchange(chipSelect, writeBuffer, readBuffer, csMode);
        }

        public void Write(IDigitalOutputPort? chipSelect, Span<byte> writeBuffer, ChipSelectMode csMode = ChipSelectMode.ActiveLow)
        {
            byte[] readBuffer = new byte[writeBuffer.Length];
            Exchange(chipSelect, writeBuffer, readBuffer, csMode);
        }

        private void DecipherSPIError(int errorCode)
        {
            switch (errorCode)
            {
                default:
                    throw new NativeException($"Communication error.  Error code {(int)errorCode}");
            }
        }

        public unsafe void Exchange(IDigitalOutputPort? chipSelect, Span<byte> writeBuffer, Span<byte> readBuffer, ChipSelectMode csMode = ChipSelectMode.ActiveLow)
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
                    var command = new SpiTransferCommand()
                    {
                        Length = readBuffer.Length,
                        TransmitBuffer = (ulong)pWrite, // NOTE: these are always 64-bit for OS compatibility
                        ReceiveBuffer = (ulong)pRead,
                        DelayuSec = 0,
                        BitsPerWord = 0,
                        SpeedHz = 0,
                        ChipSelectChange = 0
                    };

                    //                    Output.WriteLineIf(_showSpiDebug, "+Exchange");
                    //                    Output.WriteLineIf(_showSpiDebug, $" Sending {writeBuffer.Length} bytes");
                    var length = readBuffer.Length;
                    var status = Interop.ioctl(DriverHandle, Interop._IOW('k', TransferIoctl, 1), (byte*)&length);
                    if (status != 0)
                    {
                        DecipherSPIError(Marshal.GetLastWin32Error());
                    }
                    //                    Output.WriteLineIf(_showSpiDebug, $" Received {readBuffer.Length} bytes");

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

        public void SendData(IDigitalOutputPort chipSelect, params byte[] data)
        {
            throw new NotImplementedException();
        }

        public void SendData(IDigitalOutputPort chipSelect, IEnumerable<byte> data)
        {
            throw new NotImplementedException();
        }

        public byte[] ReceiveData(IDigitalOutputPort chipSelect, int numberOfBytes)
        {
            throw new NotImplementedException();
        }

        public void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, params byte[] data)
        {
            throw new NotImplementedException();
        }

        public void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, IEnumerable<byte> data)
        {
            throw new NotImplementedException();
        }

        public byte[] ReceiveData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, int numberOfBytes)
        {
            throw new NotImplementedException();
        }

        public void ExchangeData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, byte[] sendBuffer, byte[] receiveBuffer)
        {
            throw new NotImplementedException();
        }

        public void ExchangeData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, byte[] sendBuffer, byte[] receiveBuffer, int bytesToExchange)
        {
            throw new NotImplementedException();
        }
    }
}
