using Meadow.Hardware;
using System;
using System.Collections.Generic;

namespace Meadow;

/// <summary>
/// Represents an I2C bus with specified bus number and speed.
/// </summary>
public class KrtklI2CBus : II2cBus, IDisposable
{
    /// <inheritdoc/>
    public I2cBusSpeed BusSpeed { get; set; }

    internal KrtklI2CBus(I2cBusSpeed busSpeed)
    {
        BusSpeed = busSpeed;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Exchange(byte peripheralAddress, Span<byte> writeBuffer, Span<byte> readBuffer)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Read(byte peripheralAddress, Span<byte> readBuffer)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public byte[] ReadData(byte peripheralAddress, int numberOfBytes)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void Write(byte peripheralAddress, Span<byte> writeBuffer)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void WriteData(byte peripheralAddress, params byte[] data)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void WriteData(byte peripheralAddress, byte[] data, int length)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void WriteData(byte peripheralAddress, IEnumerable<byte> data)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public byte[] WriteReadData(byte peripheralAddress, int byteCountToRead, params byte[] dataToWrite)
    {
        throw new NotImplementedException();
    }
}
