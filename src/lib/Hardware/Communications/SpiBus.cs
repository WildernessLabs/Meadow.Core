using Meadow.Hardware;
using System;
using System.Collections.Generic;

namespace Meadow
{
    public class SpiBus : ISpiBus, IDisposable
    {
        private class SpiTransferCommand
        {
            public ulong TransmitBuffer { get; set; } // 64-bit pointer to byte array
            public ulong ReceiveBuffer { get; set; } // 64-bit pointer to byte array
            public int Length { get; set; }
            public int SpeedHz { get; set; } = 500000;
            public short DelayuSec { get; set; } = 0;
            public byte BitsPerWord { get; set; } = 8;
            public byte ChipSelectChange { get; set; }

        }

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

        public long[] SupportedSpeeds => throw new NotImplementedException();

        public SpiClockConfiguration Configuration => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chipSelect">Must be 0 or 1</param>
        internal unsafe SpiBus(int chipSelect, SpiMode mode = SpiMode.Mode0, int speedHz = 50000)
        {
            var bitsPerWord = 8;

            var driver = $"/dev/spidev0.{chipSelect}";
            DriverHandle = Interop.open(driver, Interop.DriverFlags.O_RDWR);
            if (DriverHandle < 0)
            {
                // TODO: maybe try modprobe here?
                throw new Exception($"Unable to open driver {driver}");
            }

            var status = Interop.ioctl(DriverHandle, Interop._IOW('k', ModeIoctl, 8), (byte*)&mode);
            if (status < 0)
            {
                throw new Exception($"Could not set SPIMode (WR): {status}");
            }

            status = Interop.ioctl(DriverHandle, Interop._IOR('k', ModeIoctl, 8), (byte*)&mode);
            if (status < 0)
            {
                throw new Exception($"Could not set SPIMode (RD): {status}");
            }

            status = Interop.ioctl(DriverHandle, Interop._IOW('k', LengthIoctl, 8), (byte*)&bitsPerWord);
            if (status < 0)
            {
                throw new Exception($"Could not set SPI bits per word (WR): {status}");
            }

            status = Interop.ioctl(DriverHandle, Interop._IOR('k', LengthIoctl, 8), (byte*)&bitsPerWord);
            if (status < 0)
            {
                throw new Exception($"Could not set SPI bits per word (RD): {status}");
            }


            status = Interop.ioctl(DriverHandle, Interop._IOW('k', LengthIoctl, 32), (byte*)&speedHz);
            if (status < 0)
            {
                throw new Exception($"Could not set SPI speed (WR): {status}");
            }

            status = Interop.ioctl(DriverHandle, Interop._IOR('k', LengthIoctl, 32), (byte*)&speedHz);
            if (status < 0)
            {
                throw new Exception($"Could not set SPI speed (RD): {status}");
            }
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
            throw new NotImplementedException();
        }

        public void Write(IDigitalOutputPort? chipSelect, Span<byte> writeBuffer, ChipSelectMode csMode = ChipSelectMode.ActiveLow)
        {
            throw new NotImplementedException();
        }

        public void Exchange(IDigitalOutputPort? chipSelect, Span<byte> writeBuffer, Span<byte> readBuffer, ChipSelectMode csMode = ChipSelectMode.ActiveLow)
        {
            throw new NotImplementedException();
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
