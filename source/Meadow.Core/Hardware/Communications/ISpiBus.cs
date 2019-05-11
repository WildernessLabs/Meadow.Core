using System;
using System.Collections.Generic;

namespace Meadow.Hardware
{
    public interface ISpiBus
    {
        /// <summary>
        /// Write a single byte to the connected peripheral.
        /// </summary>
        /// <param name="value">Value to be written (8-bits).</param>
        void WriteByte(IDigitalOutputPort chipSelect, byte value);

        /// <summary>
        /// Write a number of bytes to the peripheral.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="values">Values to be written.</param>
        void WriteBytes(IDigitalOutputPort chipSelect, byte[] values);

        /// <summary>
        /// Write an unsigned short to the peripheral.
        /// </summary>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="value">Value to be written (16-bits).</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        void WriteUShort(IDigitalOutputPort chipSelect, byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian);

        /// <summary>
        /// Write a number of unsigned shorts to the peripheral.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="values">Values to be written.</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        void WriteUShorts(IDigitalOutputPort chipSelect, byte address, ushort[] values, ByteOrder order = ByteOrder.LittleEndian);

        /// <summary>
        /// Write data a register in the peripheral.
        /// </summary>
        /// <param name="address">Address of the register to write to.</param>
        /// <param name="value">Data to write into the register.</param>
        void WriteRegister(IDigitalOutputPort chipSelect, byte address, byte value);

        /// <summary>
        /// Write data to one or more registers.
        /// </summary>
        /// <param name="address">Address of the first register to write to.</param>
        /// <param name="data">Data to write into the registers.</param>
        void WriteRegisters(IDigitalOutputPort chipSelect, byte address, byte[] data);

        /// <summary>
        /// Write data to the peripheral and also read some data from the peripheral.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written and read will be determined by the length of the byte arrays.
        /// </remarks>
        /// <param name="write">Array of bytes to be written to the device.</param>
        /// <param name="length">Amount of data to read from the device.</param>
        byte[] WriteRead(IDigitalOutputPort chipSelect, byte[] write, ushort length);

        /// <summary>
        /// Read the specified number of bytes from the peripheral.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="numberOfBytes">Number of bytes.</param>
        byte[] ReadBytes(IDigitalOutputPort chipSelect, ushort numberOfBytes);

        /// <summary>
        /// Read a registers from the peripheral.
        /// </summary>
        /// <param name="address">Address of the register to read.</param>
        byte ReadRegister(IDigitalOutputPort chipSelect, byte address);

        /// <summary>
        /// Read one or more registers from the peripheral.
        /// </summary>
        /// <param name="address">Address of the first register to read.</param>
        /// <param name="length">Number of bytes to read from the device.</param>
        byte[] ReadRegisters(IDigitalOutputPort chipSelect, byte address, ushort length);

        /// <summary>
        /// Read an usingned short from a pair of registers.
        /// </summary>
        /// <param name="address">Register address of the low byte (the high byte will follow).</param>
        /// <param name="order">Order of the bytes in the register (little endian is the default).</param>
        /// <returns>Value read from the register.</returns>
        ushort ReadUShort(IDigitalOutputPort chipSelect, byte address, ByteOrder order);

        /// <summary>
        /// Read the specified number of unsigned shorts starting at the register
        /// address specified.
        /// </summary>
        /// <param name="address">First register address to read from.</param>
        /// <param name="number">Number of unsigned shorts to read.</param>
        /// <param name="order">Order of the bytes (Little or Big endian)</param>
        /// <returns>Array of unsigned shorts.</returns>
        ushort[] ReadUShorts(IDigitalOutputPort chipSelect, byte address, ushort number, ByteOrder order = ByteOrder.LittleEndian);
    }
}
