using System;
using System.Collections.Generic;

namespace Meadow.Hardware
{
    // TODO: @Mark; i don't love `peripheralAddress`, but we need a way to distinguish
    // between peripheral and register addresses.
    public interface II2cBus
    {
        /// <summary>
        /// Write a number of bytes to the device.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="data">Data to be written.</param>
        void WriteData(byte peripheralAddress, params byte[] data);

        void WriteData(byte peripheralAddress, IEnumerable<byte> data);

        /// <summary>
        /// Writes data to the bus, followed by a restart and then reads a specified number of bytes
        /// </summary>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="byteCountToRead">Number of bytes to read</param>
        /// <param name="dataToWrite">Data to be written.</param>
        /// <returns>The data read from the bus</returns>
        byte[] WriteReadData(byte peripheralAddress, int byteCountToRead, params byte[] dataToWrite);


        /// <summary>
        ///  Read the specified number of bytes from the I2C device.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="numberOfBytes">Number of bytes.</param>
        byte[] ReadData(byte peripheralAddress, int numberOfBytes);




        /*
        /// <summary>
        /// Resets the bus
        /// </summary>
        void Reset();

        /// <summary>
        /// Write a single byte to the device.
        /// </summary>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="value">Value to be written (8-bits).</param>
        void WriteByte(byte peripheralAddress, byte value);

        /// <summary>
        /// Write an unsigned short to the device.
        /// </summary>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="value">Value to be written (16-bits).</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        void WriteUShort(byte peripheralAddress, byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian);

        /// <summary>
        /// Write a number of unsigned shorts to the device.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="values">Values to be written.</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        void WriteUShorts(byte peripheralAddress, byte address, ushort[] values, ByteOrder order = ByteOrder.LittleEndian);

        /// <summary>
        /// Write data to the device and also read some data from the device.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written and read will be determined by the length of the byte arrays.
        /// </remarks>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="write">Array of bytes to be written to the device.</param>
        /// <param name="length">Amount of data to read from the device.</param>
        byte[] WriteRead(byte peripheralAddress, byte[] write, ushort length);

        /// <summary>
        /// Write data into a single register.
        /// </summary>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="address">Address of the register to write to.</param>
        /// <param name="value">Value to write into the register.</param>
        void WriteRegister(byte peripheralAddress, byte address, byte value);

        /// <summary>
        /// Write data to one or more registers.
        /// </summary>
        /// <remarks>
        /// This method assumes that the register address is written first followed by the data to be
        /// written into the first register followed by the data for subsequent registers.
        /// </remarks>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="address">Address of the first register to write to.</param>
        /// <param name="data">Data to write into the registers.</param>
        void WriteRegisters(byte peripheralAddress, byte address, byte[] data);

        /// <summary>
        /// Read a register from the device.
        /// </summary>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="address">Address of the register to read.</param>
        byte ReadRegister(byte peripheralAddress, byte address);

        /// <summary>
        /// Read one or more registers from the device.
        /// </summary>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="address">Address of the first register to read.</param>
        /// <param name="length">Number of bytes to read from the device.</param>
        byte[] ReadRegisters(byte peripheralAddress, byte address, ushort length);

        /// <summary>
        /// Read an usingned short from a pair of registers.
        /// </summary>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="address">Register address of the low byte (the high byte will follow).</param>
        /// <param name="order">Order of the bytes in the register (little endian is the default).</param>
        /// <returns>Value read from the register.</returns>
        ushort ReadUShort(byte peripheralAddress, byte address, ByteOrder order = ByteOrder.LittleEndian);

        /// <summary>
        /// Read the specified number of unsigned shorts starting at the register
        /// address specified.
        /// </summary>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="address">First register address to read from.</param>
        /// <param name="number">Number of unsigned shorts to read.</param>
        /// <param name="order">Order of the bytes (Little or Big endian)</param>
        /// <returns>Array of unsigned shorts.</returns>
        ushort[] ReadUShorts(byte peripheralAddress, byte address, ushort number, ByteOrder order = ByteOrder.LittleEndian);
        */
    }
}
