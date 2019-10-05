using Meadow.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private long _speed;

        internal int BusNumber { get; set; }

        ///// <summary>
        ///// SPI bus object.
        ///// </summary>
        //private static Spi _spi;

        /// <summary>
        /// Configuration to use for this instance of the SPIBus.
        /// </summary>
        public SpiBus.ConfigurationOptions Configuration { get; protected set; }

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

        // TODO: Call from Device.CreateSpiBus
        // TODO: use Spi.Configuration configuration? don't we already know this, as its chip specific?
        // TODO: we should already know clock phase and polarity, yeah?
        internal static SpiBus From(
            IPin clock,
            IPin mosi,
            IPin miso,
            byte cpha = 0,
            byte cpol = 0)
        {
            // check for pin compatibility and availability
            if (!clock.Supports<SpiChannelInfo>(p => (p.LineTypes & SpiLineType.Clock) != SpiLineType.None))
            {
                throw new NotSupportedException($"Pin {clock.Name} does not support SPI Clock capability");
            }
            if (!mosi.Supports<SpiChannelInfo>(p => (p.LineTypes & SpiLineType.MOSI) != SpiLineType.None))
            {
                throw new NotSupportedException($"Pin {clock.Name} does not support SPI MOSI capability");
            }
            if (!miso.Supports<SpiChannelInfo>(p => (p.LineTypes & SpiLineType.MISO) != SpiLineType.None))
            {
                throw new NotSupportedException($"Pin {clock.Name} does not support SPI MISO capability");
            }

            // we can't set the speed here yet because the caller has to set the bus number first
            return new SpiBus();
        }

        public long BusSpeed
        {
            get => _speed;
            set
            {
                if (value == _speed) return;
                Output.WriteIf(_showSpiDebug, $"Setting bus speed to {value}");
                var actual = this.SetFrequency(value);
                _speed = actual;
            }
        }

        public void SendData(IDigitalOutputPort chipSelect, IEnumerable<byte> data)
        {
            SendData(chipSelect, ChipSelectMode.ActiveLow, data.ToArray());
        }

        public void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, IEnumerable<byte> data)
        {
            SendData(chipSelect, csMode, data.ToArray());
        }

        public void SendData(IDigitalOutputPort chipSelect, params byte[] data)
        {
            SendData(chipSelect, ChipSelectMode.ActiveLow, data);
        }

        public void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, params byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);

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
                    BufferLength = data.Length,
                    TxBuffer = gch.AddrOfPinnedObject(),
                    RxBuffer = IntPtr.Zero,
                    BusNumber = BusNumber
                };

                Output.WriteLineIf(_showSpiDebug, $" +SendData {BitConverter.ToString(data)}");
                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SPIData, ref command);

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
            }
        }

        public byte[] ReceiveData(IDigitalOutputPort chipSelect, int numberOfBytes)
        {
            return ReceiveData(chipSelect, ChipSelectMode.ActiveLow, numberOfBytes);
        }

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

                //Console.Write(" +ReceiveData");
                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SPIData, ref command);
                //Console.WriteLine($" returned {BitConverter.ToString(rxBuffer)}");

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

        public byte[] ExchangeData(IDigitalOutputPort chipSelect, params byte[] dataToWrite)
        {
            return ExchangeData(chipSelect, ChipSelectMode.ActiveLow, dataToWrite);
        }

        public byte[] ExchangeData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, params byte[] dataToWrite)
        {
            var rxBuffer = new byte[dataToWrite.Length];
            var rxGch = GCHandle.Alloc(rxBuffer, GCHandleType.Pinned);
            var txGch = GCHandle.Alloc(dataToWrite, GCHandleType.Pinned);

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
                    BufferLength = dataToWrite.Length,
                    TxBuffer = txGch.AddrOfPinnedObject(),
                    RxBuffer = rxGch.AddrOfPinnedObject(),
                    BusNumber = BusNumber
                };

                Output.WriteLineIf(_showSpiDebug, "+ExchangeData");
                Output.WriteLineIf(_showSpiDebug, $" Sending {BitConverter.ToString(dataToWrite)}");
                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SPIData, ref command);
                Output.WriteLineIf(_showSpiDebug, $" Received {BitConverter.ToString(rxBuffer)}");

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

        public long[] SupportedSpeeds
        {
            get => new long[]
                {
                    375000,
                    750000,
                    1500000,
                    3000000,
                    6000000,
                    12000000,
                    24000000,
                    48000000
                };
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
            Output.WriteLineIf(_showSpiDebug, $" setting speed to {desiredSpeed}");
            var result = UPD.Ioctl(Nuttx.UpdIoctlFn.SPISpeed, ref command);

            return speed;
            /*
            switch (BusNumber)
            {
                case 2:
                    spi_base = STM32.MEADOW_SPI2_BASE; // ESP
                    break;
                default:
                    spi_base = STM32.MEADOW_SPI3_BASE; // external
                    break;
            }

            // determine the actual supported speed
            var speed = GetSupportedSpeed(desiredSpeed);
            var divisor = SpeedToDivisor(speed);

            // stop the SPI bus
            // shutdown per the STM32 Reference manual:
            // wait for FTLVL == 0
            var sr = UPD.GetRegister(spi_base + STM32.SPI_SR_OFFSET);
            while((sr & (3 << 11)) != 0)
            {
                Thread.Sleep(1);
            }
            // wait for BSY == 0
            
            while ((sr & (1 << 7)) != 0)
            {
                Thread.Sleep(1);
                sr = UPD.GetRegister(spi_base + STM32.SPI_SR_OFFSET);
            }
            // set SPE == 0
            UPD.UpdateRegister(spi_base + STM32.SPI_CR1_OFFSET, STM32.SPI_CR1_SPE, 0);
            // read data until FRLVL == 0
            while ((sr & (3 << 9)) != 0)
            {
                Thread.Sleep(1);
                var dr = UPD.GetRegister(spi_base + STM32.SPI_DR_OFFSET);
                sr = UPD.GetRegister(spi_base + STM32.SPI_SR_OFFSET);
            }

            // change the speed
            uint s = (divisor & STM32.SPI_BR_MASK) << STM32.SPI_BR_SHIFT;
            Output.WriteIf(_showSpiDebug, $"Setting SPI_CR1 bits {s:x} divisor={divisor}");
            UPD.UpdateRegister(spi_base + STM32.SPI_CR1_OFFSET, 0, s);
            if (_showSpiDebug)
            {
                var actual = UPD.GetRegister(spi_base + STM32.SPI_CR1_OFFSET);
                Output.WriteIf(_showSpiDebug, $"SPI_CR1 = 0x{actual:x8}");
            }

            // re-start the SPI bus
            UPD.UpdateRegister(spi_base + STM32.SPI_CR1_OFFSET, 0, STM32.SPI_CR1_SPE);
            */
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
                if (desiredSpeed > test)
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


        ///// <summary>
        ///// Create a new SPIBus object using the requested clock phase and polarity.
        ///// </summary>
        ///// <param name="cpha">CPHA - Clock Phase (0 or 1).</param>
        ///// <param name="cpol">CPOL - Clock Polarity (0 or 1).</param>
        ///// <param name="speed">Speed of the SPI bus.</param>
        //protected SpiBus(ushort speed = 1000, byte cpha = 0, byte cpol = 0)
        //{
        //    Configure(module, chipSelect, cpha, cpol, speed);
        //    //_spi = new Spi(Configuration);
        //}

        ///// <summary>
        ///// Create a new SPIBus operating in the specified mode.
        ///// </summary>
        ///// <remarks>
        /////     Mode    CPOL    CPHA
        /////     0       0       0
        /////     1       0       1
        /////     2       1       0
        /////     3       1       1
        ///// </remarks>
        ///// <param name="module">SPI module to configure.</param>
        ///// <param name="chipSelect">Chip select pin.</param>
        ///// <param name="mode">SPI Bus Mode - should be in the range 0 - 3.</param>
        ///// <param name="speed">Speed of the SPI bus.</param>
        //public SpiBus(Spi.SPI_module module, IPin chipSelect, byte mode, ushort speed)
        //{
        //    if (mode > 3) {
        //        throw new ArgumentException("SPI Mode should be in the range 0 - 3.");
        //    }
        //    byte cpha = 0;
        //    byte cpol = 0;
        //    switch (mode) {
        //        case 1:
        //            cpha = 1;
        //            break;
        //        case 2:
        //            cpol = 1;
        //            break;
        //        case 3:
        //            cpol = 1;
        //            cpha = 1;
        //            break;
        //    }
        //    Configure(module, chipSelect, cpha, cpol, speed);
        //    _spi = new Spi(Configuration);
        //}


        ///// <summary>
        ///// Works out how the SPI bus should be configured from the clock polarity and phase.
        ///// </summary>
        ///// <param name="module">SPI module to configure.</param>
        ///// <param name="chipSelect">Chip select pin.</param>
        ///// <param name="cpha">CPHA - Clock phase (0 or 1).</param>
        ///// <param name="cpol">CPOL - Clock polarity (0 or 1).</param>
        ///// <param name="speed">Speed of the SPI bus.</param>
        ///// <returns>SPI Configuration object.</returns>
        //private void Configure(Spi.SPI_module module, IPin chipSelect, byte cpha, byte cpol,
        //    ushort speed)
        //{
        //    if (cpha > 1) {
        //        throw new ArgumentException("Clock phase should be 0 to 1.");
        //    }
        //    if (cpol > 1) {
        //        throw new ArgumentException("Clock polarity should be 0 to 1.");
        //    }
        //    Configuration = new Spi.Configuration(SPI_mod: module,
        //                                       ChipSelect_Port: chipSelect,
        //                                       ChipSelect_ActiveState: false,
        //                                       ChipSelect_SetupTime: 0,
        //                                       ChipSelect_HoldTime: 0,
        //                                       Clock_IdleState: (cpol == 1),
        //                                       Clock_Edge: (cpha == 1),
        //                                       Clock_RateKHz: speed);
        //}

        /*
    /// <summary>
    /// Write a single byte to the peripheral.
    /// </summary>
    /// <param name="value">Value to be written (8-bits).</param>
    public void WriteByte(IDigitalOutputPort chipSelect, byte value)
    {
        WriteBytes(chipSelect, new[] { value });
    }

    /// <summary>
    /// Write a number of bytes to the peripheral.
    /// </summary>
    /// <remarks>
    /// The number of bytes to be written will be determined by the length of the byte array.
    /// </remarks>
    /// <param name="values">Values to be written.</param>
    public void WriteBytes(IDigitalOutputPort chipSelect, byte[] values)
    {
        //_spi.Config = Configuration;
        //_spi.Write(values);
    }

    /// <summary>
    /// Write an unsigned short to the peripheral.
    /// </summary>
    /// <param name="address">Address to write the first byte to.</param>
    /// <param name="value">Value to be written (16-bits).</param>
    /// <param name="order">Indicate if the data should be written as big or little endian.</param>
    public void WriteUShort(IDigitalOutputPort chipSelect, byte address, ushort value,
        ByteOrder order = ByteOrder.LittleEndian)
    {
        var data = new byte[2];
        if (order == ByteOrder.LittleEndian) {
            data[0] = (byte)(value & 0xff);
            data[1] = (byte)((value >> 8) & 0xff);
        } else {
            data[0] = (byte)((value >> 8) & 0xff);
            data[1] = (byte)(value & 0xff);
        }
        WriteRegisters(chipSelect, address, data);
    }

    /// <summary>
    /// Write a number of unsigned shorts to the peripheral.
    /// </summary>
    /// <remarks>
    /// The number of bytes to be written will be determined by the length of the byte array.
    /// </remarks>
    /// <param name="address">Address to write the first byte to.</param>
    /// <param name="values">Values to be written.</param>
    /// <param name="order">Indicate if the data should be written as big or little endian.</param>
    public void WriteUShorts(IDigitalOutputPort chipSelect, byte address, ushort[] values,
        ByteOrder order = ByteOrder.LittleEndian)
    {
        var data = new byte[2 * values.Length];
        for (var index = 0; index < values.Length; index++) {
            if (order == ByteOrder.LittleEndian) {
                data[index * 2] = (byte)(values[index] & 0xff);
                data[(index * 2) + 1] = (byte)((values[index] >> 8) & 0xff);
            } else {
                data[index * 2] = (byte)((values[index] >> 8) & 0xff);
                data[(index * 2) + 1] = (byte)(values[index] & 0xff);
            }
        }
        WriteRegisters(chipSelect, address, data);
    }

    /// <summary>
    /// Write data a register in the peripheral.
    /// </summary>
    /// <param name="address">Address of the register to write to.</param>
    /// <param name="value">Data to write into the register.</param>
    public void WriteRegister(IDigitalOutputPort chipSelect, byte address, byte value)
    {
        WriteRegisters(chipSelect, address, new[] { value });
    }

    /// <summary>
    /// Write data to one or more registers.
    /// </summary>
    /// <param name="address">Address of the first register to write to.</param>
    /// <param name="data">Data to write into the registers.</param>
    public void WriteRegisters(IDigitalOutputPort chipSelect, byte address, byte[] values)
    {
        var data = new byte[values.Length + 1];
        data[0] = address;
        Array.Copy(values, 0, data, 1, values.Length);
        WriteBytes(chipSelect, data);
    }

    /// <summary>
    /// Write data to the peripheral and also read some data from the peripheral.
    /// </summary>
    /// <remarks>
    /// The number of bytes to be written and read will be determined by the length of the byte arrays.
    /// </remarks>
    /// <param name="write">Array of bytes to be written to the device.</param>
    /// <param name="length">Amount of data to read from the device.</param>
    public byte[] WriteRead(IDigitalOutputPort chipSelect, byte[] write, ushort length)
    {
        var result = new byte[length];
        //Config = Configuration;
        //WriteRead(chipSelect, write, result);
        //return result;
        return new byte[] { 0 };
    }

    /// <summary>
    /// Read the specified number of bytes from the I2C peripheral.
    /// </summary>
    /// <returns>The bytes.</returns>
    /// <param name="numberOfBytes">Number of bytes.</param>
    public byte[] ReadBytes(IDigitalOutputPort chipSelect, ushort numberOfBytes)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Read a registers from the peripheral.
    /// </summary>
    /// <param name="address">Address of the register to read.</param>
    public byte ReadRegister(IDigitalOutputPort chipSelect, byte address)
    {
        return WriteRead(chipSelect, new[] { address }, 1)[0];
    }

    /// <summary>
    /// Read one or more registers from the peripheral.
    /// </summary>
    /// <param name="address">Address of the first register to read.</param>
    /// <param name="length">Number of bytes to read from the device.</param>
    public byte[] ReadRegisters(IDigitalOutputPort chipSelect, byte address, ushort length)
    {
        return WriteRead(chipSelect, new[] { address }, length);
    }

    /// <summary>
    /// Read an unsigned short from a pair of registers.
    /// </summary>
    /// <param name="address">Register address of the low byte (the high byte will follow).</param>
    /// <param name="order">Order of the bytes in the register (little endian is the default).</param>
    /// <returns>Value read from the register.</returns>
    public ushort ReadUShort(IDigitalOutputPort chipSelect, byte address,
        ByteOrder order = ByteOrder.LittleEndian)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Read the specified number of unsigned shorts starting at the register
    /// address specified.
    /// </summary>
    /// <param name="address">First register address to read from.</param>
    /// <param name="number">Number of unsigned shorts to read.</param>
    /// <param name="order">Order of the bytes (Little or Big endian)</param>
    /// <returns>Array of unsigned shorts.</returns>
    public ushort[] ReadUShorts(IDigitalOutputPort chipSelect, byte address, ushort number,
        ByteOrder order = ByteOrder.LittleEndian)
    {
        throw new NotImplementedException();
    }
    */
    }
}