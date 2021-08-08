
using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Define a contract for general peripheral communications classes.
    /// </summary>
    public interface IByteCommunications
    {
        /// <summary>
        /// Reads data from the peripheral.
        /// </summary>
        /// <param name="readBuffer">The buffer to read from the peripheral into.</param>
        /// <remarks>
        /// The number of bytes to be read is determined by the length of the
        /// `readBuffer`.
        /// </remarks>
        void Read(Span<byte> readBuffer);

        /// <summary>
        /// Reads data from the peripheral starting at the specified address.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="readBuffer"></param>
        void ReadRegister(byte address, Span<byte> readBuffer);

        /// <summary>
        /// Read a register from the peripheral.
        /// </summary>
        /// <param name="address">Address of the register to read.</param>
        byte ReadRegister(byte address);

        /// <summary>
        /// Read an usingned short from a register.
        /// </summary>
        /// <param name="address">Register address of the low byte (the high byte will follow).</param>
        /// <param name="order">Order of the bytes in the register (little endian is the default).</param>
        /// <returns>Value read from the register.</returns>
        ushort ReadRegisterAsUShort(byte address, ByteOrder order = ByteOrder.LittleEndian);

        // TODO: implement and Doc
        //uint ReadRegisterAsUInt(byte address, ByteOrder order = ByteOrder.LittleEndian);
        //ulong ReadRegisterAsULong(byte address, ByteOrder order = ByteOrder.LittleEndian);

        /// <summary>
        /// Write a single byte to the peripheral.
        /// </summary>
        /// <param name="value">Value to be written (8-bits).</param>
        void Write(byte value);

        /// <summary>
        /// Write an array of bytes to the peripheral.
        /// </summary>
        /// <param name="writeBuffer">A buffer of byte values to be written.</param>
        void Write(Span<byte> writeBuffer);

        /// <summary>
        /// Write data tp a register in the peripheral.
        /// </summary>
        /// <param name="address">Address of the register to write to.</param>
        /// <param name="value">Data to write into the register.</param>
        void WriteRegister(byte address, byte value);

        /// <summary>
        /// Write data to a register in the peripheral.
        /// </summary>
        /// <param name="address">Address of the register to write to.</param>
        /// <param name="writeBuffer">A buffer of byte values to be written.</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        void WriteRegister(byte address, Span<byte> writeBuffer, ByteOrder order = ByteOrder.LittleEndian);

        /// <summary>
        /// Write an unsigned short to the peripheral.
        /// </summary>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="value">Value to be written (16-bits).</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        void WriteRegister(byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian);

        /// <summary>
        /// Write an unsigned integer to the peripheral.
        /// </summary>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="value">Value to be written.</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        void WriteRegister(byte address, uint value, ByteOrder order = ByteOrder.LittleEndian);

        /// <summary>
        /// Write an unsigned long to the peripheral.
        /// </summary>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="value">Value to be written.</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        void WriteRegister(byte address, ulong value, ByteOrder order = ByteOrder.LittleEndian);

        ///// <summary>
        ///// Write a number of unsigned shorts to the peripheral.
        ///// </summary>
        ///// <remarks>
        ///// The number of bytes to be written will be determined by the length of the byte array.
        ///// </remarks>
        ///// <param name="address">Address to write the first byte to.</param>
        ///// <param name="values">Values to be written.</param>
        ///// <param name="order">Indicate if the data should be written as big or little endian.</param>
        //void WriteUShorts(byte address, Span<ushort> values, ByteOrder order = ByteOrder.LittleEndian);

        /// <summary>
        /// Write data to followed by read data from the peripheral.
        /// </summary>
        /// <param name="writeBuffer">Data to write</param>
        /// <param name="readBuffer">Buffer where read data will be written.  Number of bytes read is determined by buffer size</param>
        void Exchange(Span<byte> writeBuffer, Span<byte> readBuffer);

        //==== OLD AND BUSTED //TODO: Delete after M.Foundation update

        /// <summary>
        /// Write an array of bytes to the peripheral.
        /// </summary>
        /// <param name="values">Values to be written.</param>
        [Obsolete]
        void WriteBytes(byte[] values);

        /// <summary>
        /// Write a number of unsigned shorts to the peripheral.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="values">Values to be written.</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        [Obsolete("Use Span<ushort> overload.")]
        void WriteUShorts(byte address, ushort[] values, ByteOrder order = ByteOrder.LittleEndian);

        /// <summary>
        /// Write data to one or more registers.
        /// </summary>
        /// <param name="address">Address of the first register to write to.</param>
        /// <param name="data">Data to write into the registers.</param>
        [Obsolete("Use Span<ushort> overload.")]
        void WriteRegisters(byte address, byte[] data);

        /// <summary>
        /// Write data to the peripheral and also read some data from the peripheral.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written and read will be determined by the length of the byte arrays.
        /// </remarks>
        /// <param name="write">Array of bytes to be written to the device.</param>
        /// <param name="length">Amount of data to read from the device.</param>
        [Obsolete("Use Exchange")]
        byte[] WriteRead(byte[] write, ushort length);

        /// <summary>
        /// Read the specified number of bytes from the peripheral.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="numberOfBytes">Number of bytes.</param>
        [Obsolete("Use Span<byte> overload.")]
        byte[] ReadBytes(ushort numberOfBytes);

        /// <summary>
        /// Read one or more registers from the peripheral.
        /// </summary>
        /// <param name="address">Address of the first register to read.</param>
        /// <param name="length">Number of bytes to read from the device.</param>
        [Obsolete]
        byte[] ReadRegisters(byte address, ushort length);

        /// <summary>
        /// Read the specified number of unsigned shorts starting at the register
        /// address specified.
        /// </summary>
        /// <param name="address">First register address to read from.</param>
        /// <param name="number">Number of unsigned shorts to read.</param>
        /// <param name="order">Order of the bytes (Little or Big endian)</param>
        /// <returns>Array of unsigned shorts.</returns>
        [Obsolete("Use Span<byte> overload.")]
        ushort[] ReadUShorts(byte address, ushort number, ByteOrder order = ByteOrder.LittleEndian);
    }
}