﻿using System;
using System.Collections.Generic;

namespace Meadow.Hardware
{
    public interface II2cBus
    {
        /// <summary>
        /// Bus Clock speed in Hz
        /// </summary>
        int Frequency { get; set; }

        /// <summary>
        /// Writes a number of bytes to the bus.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="data">Data to be written.</param>
        void WriteData(byte peripheralAddress, params byte[] data);

        /// <summary>
        /// Writes a number of bytes to the bus.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="data">Data to be written.</param>
        void WriteData(byte peripheralAddress, IEnumerable<byte> data);

        /// <summary>
        /// Writes a number of bytes to the bus.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="data">Data to be written.</param>
        void WriteData(byte peripheralAddress, Span<byte> data);

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
    }
}
