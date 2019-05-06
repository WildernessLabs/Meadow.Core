using Meadow;
using System;
using System.Runtime.CompilerServices;

namespace Meadow.Hardware
{
    /// <summary>
    /// Implements a software version of the SPI communication protocol.
    /// </summary>
    public class SoftwareSPIBus : ISpiBus
    {
        #region Member variables / fields

        /// <summary>
        /// MOSI output port.
        /// </summary>
        private IDigitalOutputPort _mosi;

        /// <summary>
        /// MISO Input port.
        /// </summary>
        private IDigitalInputPort _miso;

        /// <summary>
        /// Clock output port.
        /// </summary>
        private IDigitalOutputPort _clock;

        /// <summary>
        /// Boolean representation of the clock polarity.
        /// </summary>
        private readonly bool _polarity;

        /// <summary>
        /// Boolean representation of the clock phase.
        /// </summary>
        private readonly bool _phase;

        #endregion Member variables / fields

        #region Constructors

        /// <summary>
        /// Default constructor (private to prevent it from being used).
        /// </summary>
        private SoftwareSPIBus()
        {
        }

        /// <summary>
        /// Create a new SoftwareSPIBus object using the specified pins.
        /// </summary>
        /// <param name="mosi">MOSI pin.</param>
        /// <param name="miso">MISO pin</param>
        /// <param name="clock">Clock pin.</param>
        /// <param name="cpha">Clock phase (0 or 1, default is 0).</param>
        /// <param name="cpol">Clock polarity (0 or 1, default is 0).</param>
        public SoftwareSPIBus(IIODevice device, IPin mosi, IPin miso, IPin clock/*, IPin chipSelect*/, byte cpha = 0, byte cpol = 0)
        {
            if (mosi == null)
            {
                throw new ArgumentException("MOSI line cannot be set to None.");
            }
            if (clock == null)
            {
                throw new ArgumentException("Clock line cannot be set to None");
            }

            _phase = (cpha == 1);
            _polarity = (cpol == 1);
            _mosi = device.CreateDigitalOutputPort(mosi, false);
            _miso = miso == null ? null : device.CreateDigitalInputPort(miso, InterruptMode.EdgeRising, ResistorMode.Disabled);
            _clock = device.CreateDigitalOutputPort(clock, _polarity);
            //_chipSelect = chipSelect == null ? null : device.CreateDigitalOutputPort(chipSelect, true);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Write a single byte to the SPI bus.
        /// </summary>
        /// <param name="value">Byte value to write to the SPI bus.</param>
        private void Write(byte value)
        {
            byte mask = 0x80;
            var clock = _phase;

            for (var index = 0; index < 8; index++)
            {
                _mosi.State = ((value & mask) > 0);
                _clock.State = !clock;
                _clock.State = clock;
                mask >>= 1;
            }
        }

        /// <summary>
        /// Write a single byte to the peripheral.
        /// </summary>
        /// <param name="value">Value to be written (8-bits).</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WriteByte(IDigitalOutputPort chipSelect, byte value)
        {
            chipSelect.State = false;
            Write(value);
            chipSelect.State = true;
        }

        /// <summary>
        /// Write a number of bytes to the peripheral.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length 
        /// of the byte array.
        /// </remarks>
        /// <param name="values">Values to be written.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WriteBytes(IDigitalOutputPort chipSelect, byte[] values)
        {
            chipSelect.State = false;
            for (int index = 0; index < values.Length; index++)
            {
                Write(values[index]);
            }
            chipSelect.State = true;
        }

        /// <summary>
        /// Write data a register in the peripheral.
        /// </summary>
        /// <param name="register">Address of the register to write to.</param>
        /// <param name="value">Data to write into the register.</param>
        public void WriteRegister(IDigitalOutputPort chipSelect, byte register, byte value)
        {
            byte[] values = { register, value};
            WriteBytes(chipSelect, values);
        }

        /// <summary>
        /// Write data to one or more registers.
        /// </summary>
        /// <param name="address">Address of the first register to write to.</param>
        /// <param name="data">Data to write into the registers.</param>
        public void WriteRegisters(IDigitalOutputPort chipSelect, byte address, byte[] data)
        {
            //TODO: (Bryan for Mark) does this actually do anything?
            // shouldn't it call a write?
            byte[] values = new byte[data.Length + 1];
            values[0] = address;
            Array.Copy(data, 0, values, 1, data.Length);
        }

        /// <summary>
        /// Write an unsigned short to the peripheral.
        /// </summary>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="value">Value to be written (16-bits).</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        public void WriteUShort(IDigitalOutputPort chipSelect, byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = new byte[2];
            if (order == ByteOrder.LittleEndian)
            {
                data[0] = (byte) (value & 0xff);
                data[1] = (byte) ((value >> 8) & 0xff);
            }
            else
            {
                data[0] = (byte) ((value >> 8) & 0xff);
                data[1] = (byte) (value & 0xff);
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
        public void WriteUShorts(IDigitalOutputPort chipSelect, byte address, ushort[] values, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = new byte[2 * values.Length];
            for (var index = 0; index < values.Length; index++)
            {
                if (order == ByteOrder.LittleEndian)
                {
                    data[2 * index] = (byte) (values[index] & 0xff);
                    data[(2 * index) + 1] = (byte) ((values[index] >> 8) & 0xff);
                }
                else
                {
                    data[2 * index] = (byte) ((values[index] >> 8) & 0xff);
                    data[(2 * index) + 1] = (byte) (values[index] & 0xff);
                }
            }
            WriteRegisters(chipSelect, address, data);
        }

        /// <summary>
        /// Write and read a single byte.
        /// </summary>
        /// <remarks>
        /// This internal method assumes that CS has been asserted correctly
        /// before it is called.
        /// </remarks>
        /// <param name="value">Value to write.</param>
        /// <returns>Byte read from the SPI interface.</returns>
        private byte WriteRead(byte value)
        {
            byte result = 0;
            byte mask = 0x80;
            var clock = _phase;

            for (var index = 0; index < 8; index++)
            {
                _mosi.State = ((value & mask) > 0);
                bool data = false;
                if (!_phase)
                {
                    data = _miso.State;
                }
                _clock.State = (!clock);
                if (_phase)
                {
                    data = _miso.State;
                }
                _clock.State = (clock);
                if (data)
                {
                    result |= mask;
                }
                mask >>= 1;
            }
            return(result);
        }

        /// <summary>
        /// Write data to the peripheral and also read some data from the peripheral.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written and read will be determined by the length of the byte arrays.
        /// </remarks>
        /// <param name="write">Array of bytes to be written to the device.</param>
        /// <param name="length">Amount of data to read from the device.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] WriteRead(IDigitalOutputPort chipSelect, byte[] write, ushort length)
        {
            if (length < write.Length)
            {
                throw new ArgumentException(nameof(length));
            }
            if (_miso == null)
            {
                throw new InvalidOperationException("Cannot read from SPI bus when the MISO pin is set to None");
            }
            chipSelect.State = false;
            byte[] result = new byte[length];
            for (var index = 0; index < length; index++)
            {
                byte value = 0;
                if (index < write.Length)
                {
                    value = write[index];
                }
                result[index] = WriteRead(value);
            }
            chipSelect.State = true;
            _mosi.State = (false);
            return(result);
        }

        /// <summary>
        /// Read the specified number of bytes from the SPI peripheral.
        /// </summary>
        /// <param name="numberOfBytes">Number of bytes.</param>
        /// <returns>The bytes read from the device.</returns>
        public byte[] ReadBytes(IDigitalOutputPort chipSelect, ushort numberOfBytes)
        {
            byte[] values = new byte[numberOfBytes];
            for (int index = 0; index < numberOfBytes; index++)
            {
                values[index] = 0;
            }
            return(WriteRead(chipSelect, values, numberOfBytes));
        }

        /// <summary>
        /// Read a register from the peripheral.
        /// </summary>
        /// <param name="address">Address of the register to read.</param>
        public byte ReadRegister(IDigitalOutputPort chipSelect, byte address)
        {
            //TODO: @Mark, i think this needs a chip select call pull down/up
            // if i unmderstand this correctly, the private methods assume
            // chip select is already taken care of
            return(WriteRead(address));
        }

        /// <summary>
        /// Read one or more registers from the peripheral.
        /// </summary>
        /// <param name="address">Address of the first register to read.</param>
        /// <param name="length">Number of bytes to read from the device.</param>
        public byte[] ReadRegisters(IDigitalOutputPort chipSelect, byte address, ushort length)
        {
            byte[] registerAddress = { address };
            return WriteRead(chipSelect, registerAddress, length);
        }

        /// <summary>
        /// Read an unsigned short from a pair of registers.
        /// </summary>
        /// <param name="address">Register address of the low byte (the high byte will follow).</param>
        /// <param name="order">Order of the bytes in the register (little endian is the default).</param>
        /// <returns>Value read from the register.</returns>
        public ushort ReadUShort(IDigitalOutputPort chipSelect, byte address, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = ReadRegisters(chipSelect, address, 3);
            ushort result = 0;
            if (order == ByteOrder.LittleEndian)
            {
                result = (ushort) ((data[2] << 8) + data[1]);
            }
            else
            {
                result = (ushort) ((data[1] << 8) + data[2]);
            }
            return result;
        }

        /// <summary>
        /// Read the specified number of unsigned shorts starting at the register
        /// address specified.
        /// </summary>
        /// <param name="address">First register address to read from.</param>
        /// <param name="number">Number of unsigned shorts to read.</param>
        /// <param name="order">Order of the bytes (Little or Big endian)</param>
        /// <returns>Array of unsigned shorts.</returns>
        public ushort[] ReadUShorts(IDigitalOutputPort chipSelect, byte address, ushort number, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = ReadRegisters(chipSelect, address, (ushort) ((2 * number) & 0xffff));
            var result = new ushort[number];
            for (var index = 0; index < number; index++)
            {
                if (order == ByteOrder.LittleEndian)
                {
                    result[index] = (ushort) ((data[(2 * index) + 2] << 8) + data[(2 * index) + 1]);
                }
                else
                {
                    result[index] = (ushort) ((data[(2 * index) + 1] << 8) + data[(2 * index) + 2]);
                }
            }
            return result;
        }

        #endregion Methods
    }
}