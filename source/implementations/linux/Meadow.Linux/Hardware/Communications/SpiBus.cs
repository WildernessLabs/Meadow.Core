using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Meadow;

/// <summary>
/// Represents an SPI bus
/// </summary>
public partial class SpiBus : ISpiBus, IDisposable
{
    private SemaphoreSlim _busSemaphore = new SemaphoreSlim(1, 1);

    private const int TransferIoctl = 0;
    private const int ModeIoctl = 1;
    private const int JustificationIoctl = 2;
    private const int LengthIoctl = 3;
    private const int SpeedIoctl = 4;
    private const int MAX_TX_BLOCK_SIZE_BYTES = 4096;

    [Flags]
    internal enum SpiMode
    {
        CPHA = 0x01,
        CPOL = 0x02,
        Mode0 = 0 | 0,
        Mode1 = 0 | CPHA,
        Mode2 = CPOL | 0,
        Mode3 = CPOL | CPHA
    }

    private int DriverHandle { get; set; }
    private int BusNumber { get; }

    /// <inheritdoc/>
    public Frequency[] SupportedSpeeds
    {
        get => new Frequency[]
            {
                new Frequency(375, Frequency.UnitType.Kilohertz),
                new Frequency(750, Frequency.UnitType.Kilohertz),
                new Frequency(1500, Frequency.UnitType.Kilohertz),
                new Frequency(3000, Frequency.UnitType.Kilohertz),
                new Frequency(6000, Frequency.UnitType.Kilohertz),
                new Frequency(12000, Frequency.UnitType.Kilohertz),
                new Frequency(24000, Frequency.UnitType.Kilohertz),
                new Frequency(48000, Frequency.UnitType.Kilohertz),
            };
    }

    /// <inheritdoc/>
    public SpiClockConfiguration Configuration { get; private set; }

    internal unsafe SpiBus(int chipSelect, SpiClockConfiguration.ClockPhase phase, SpiClockConfiguration.ClockPolarity polarity, Units.Frequency speed)
        : this(0, chipSelect, (SpiMode)((int)phase | (int)polarity), speed)
    {
    }

    internal unsafe SpiBus(int busNumber, int chipSelect, SpiMode mode, Units.Frequency speed)
    {
        BusNumber = busNumber;
        var driver = $"/dev/spidev{busNumber}.{chipSelect}";
        DriverHandle = Interop.open(driver, Interop.DriverFlags.O_RDWR);
        if (DriverHandle < 0)
        {
            // TODO: maybe try modprobe here?
            throw new NativeException($"Unable to open driver {driver}. Check your user permissions", Marshal.GetLastWin32Error());
        }

        Mode = mode;
        BitsPerWord = 8;
        SpeedHz = Convert.ToInt32(speed.Hertz);

        Configuration = new SpiClockConfiguration(speed)
        {
            Phase = SpiClockConfiguration.ClockPhase.Zero,
            Polarity = SpiClockConfiguration.ClockPolarity.Normal
        };
    }

    /// <inheritdoc/>
    public unsafe int SpeedHz
    {
        set
        {
            var status = Interop.ioctl(DriverHandle, Interop._IOW('k', SpeedIoctl, 4), (byte*)&value);
            if (status < 0)
            {
                throw new NativeException($"Could not set SPI speed (WR): {status}", Marshal.GetLastWin32Error());
            }
        }
        get
        {
            int speed = 0;
            var status = Interop.ioctl(DriverHandle, Interop._IOR('k', SpeedIoctl, 4), (byte*)&speed);
            if (status < 0)
            {
                throw new NativeException($"Could not get SPI speed (RD): {status}", Marshal.GetLastWin32Error());
            }
            return speed;
        }
    }

    internal unsafe SpiMode Mode
    {
        set
        {
            var status = Interop.ioctl(DriverHandle, Interop._IOW('k', ModeIoctl, 1), (byte*)&value);
            if (status < 0)
            {
                throw new NativeException($"Could not set SPIMode (WR): {status}", Marshal.GetLastWin32Error());
            }
        }
        get
        {
            byte value = 0;
            var status = Interop.ioctl(DriverHandle, Interop._IOR('k', ModeIoctl, 1), &value);
            if (status < 0)
            {
                throw new NativeException($"Could not get SPIMode (RD): {status}", Marshal.GetLastWin32Error());
            }
            return (SpiMode)value;
        }
    }

    /// <inheritdoc/>
    public unsafe byte BitsPerWord
    {
        set
        {
            var status = Interop.ioctl(DriverHandle, Interop._IOW('k', LengthIoctl, 1), &value);
            if (status < 0)
            {
                throw new NativeException($"Could not set SPI bits per word (WR): {status}", Marshal.GetLastWin32Error());
            }
        }
        get
        {
            byte value = 0;
            var status = Interop.ioctl(DriverHandle, Interop._IOR('k', LengthIoctl, 1), &value);
            if (status < 0)
            {
                throw new NativeException($"Could not get SPI bits per word (RD): {status}", Marshal.GetLastWin32Error());
            }
            return value;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (DriverHandle > 0)
        {
            Interop.close(DriverHandle);
            DriverHandle = 0;
        }
    }

    /// <inheritdoc/>
    public void Read(IDigitalOutputPort? chipSelect, Span<byte> readBuffer, ChipSelectMode csMode = ChipSelectMode.ActiveLow)
    {
        byte[] writeBuffer = new byte[readBuffer.Length];
        Exchange(chipSelect, writeBuffer, readBuffer, csMode);
    }

    /// <inheritdoc/>
    public void Write(IDigitalOutputPort? chipSelect, Span<byte> writeBuffer, ChipSelectMode csMode = ChipSelectMode.ActiveLow)
    {
        byte[] readBuffer = new byte[writeBuffer.Length];
        Exchange(chipSelect, writeBuffer, readBuffer, csMode);
    }

    private void DecipherSPIError(int status, int errorCode)
    {
        switch (errorCode)
        {
            case 22:
                throw new NativeException($"Communication error.  {errorCode} (Invalid Argument)");
            case 90:
                throw new NativeException($"Communication error.  {errorCode} (Message too long)");
            default:
                throw new NativeException($"Communication error.  Return {status}  Error code {errorCode}");
        }
    }

    /// <inheritdoc/>
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
                byte* p = pWrite;

                // each write can't be bigger than MAX_TX_BLOCK_SIZE_BYTES
                var offset = 0;
                while (offset < writeBuffer.Length)
                {
                    var length = (writeBuffer.Length - offset) > MAX_TX_BLOCK_SIZE_BYTES ? MAX_TX_BLOCK_SIZE_BYTES : (writeBuffer.Length - offset);

                    var command = new SpiTransferCommand()
                    {
                        Length = length,
                        TransmitBuffer = (ulong)p, // NOTE: these are always 64-bit for OS compatibility
                        ReceiveBuffer = (ulong)pRead,
                        DelayuSec = 0,
                        BitsPerWord = 0,
                        SpeedHz = 0,
                        ChipSelectChange = 0
                    };

                    p += length;
                    offset += length;

                    var status = Interop.ioctl(DriverHandle, Interop._IOW('k', TransferIoctl, 32), (byte*)&command);
                    if (status < 0)
                    {
                        DecipherSPIError(status, Marshal.GetLastWin32Error());
                    }
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
}
