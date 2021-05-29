using Meadow.Devices;
using Meadow.Units;
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
    /// Represents an I2C communication channel that conforms to the ICommunicationBus
    /// contract.
    /// </summary>
    public class I2cBus : II2cBus
    {
        private bool _showI2cDebug = false;
        private SemaphoreSlim _busSemaphore = new SemaphoreSlim(1, 1);
        private Frequency _frequency;

        private IMeadowIOController IOController { get; }

        public void Dispose()
        {
        }

        /// <summary>
        /// Bus Clock speed in Hz
        /// </summary>
        public Frequency Frequency 
        {
            get => _frequency;
            set
            {
                switch (value.Hertz)
                {
                    case 100000:
                    case 400000:
                    case 1000000:
                        _frequency = value;
                        break;
                    default:
                        int actual;

                        // always round down (except if we're below the floor)
                        if (value.Hertz > 1000000)
                        {
                            actual = 1000000;
                        }
                        else if (value.Hertz > 400000)
                        {
                            actual = 400000;
                        }
                        else
                        {
                            actual = 100000;
                        }

                        Console.WriteLine($"Warning: Invalid I2C Frequency of {value}. Adjusting to {actual}");
                        _frequency = new Frequency(actual, Frequency.UnitType.Hertz);
                        break;

                }
            }
        }

        /// <summary>
        /// Default constructor for the I2CBus class.  This is private to prevent the
        /// developer from calling it.
        /// </summary>
        private I2cBus(
            IMeadowIOController ioController,
            IPin clock,
            II2cChannelInfo clockChannel,
            IPin data,
            II2cChannelInfo dataChannel,
            Frequency frequency,
            ushort transactionTimeout = 100)
        {
            IOController = ioController;
            Frequency = frequency;
#if !DEBUG
            // ensure this is off in release (in case a dev sets it to true and fogets during check-in
            _showI2cDebug = false;
#endif
        }

        private void Disable()
        {
            var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CShutdown);
            if (result != 0)
            {
                var error = UPD.GetLastError();
                throw new NativeException(error.ToString());
            }
        }

        /// <summary>
        /// Creates an I2C bus for a set of given pins and parameters
        /// </summary>
        /// <param name="ioController">The Meadow IO Controller</param>
        /// <param name="clock">Clock (SCL) pin</param>
        /// <param name="data">Data (SDA) pin</param>
        /// <param name="transactionTimeout">Bus transaction timeout</param>
        /// <returns>An I2CBus instance</returns>
        public static I2cBus From(IMeadowIOController ioController, IPin clock, IPin data, I2cBusSpeed busSpeed, ushort transactionTimeout = 100)
        {
            return From(ioController, clock, data, new Frequency((int)busSpeed, Units.Frequency.UnitType.Hertz), transactionTimeout);
        }

        /// <summary>
        /// Creates an I2C bus for a set of given pins and parameters
        /// </summary>
        /// <param name="ioController">The Meadow IO Controller</param>
        /// <param name="clock">Clock (SCL) pin</param>
        /// <param name="data">Data (SDA) pin</param>
        /// <param name="frequency">Bus clock speed</param>
        /// <param name="transactionTimeout">Bus transaction timeout</param>
        /// <returns>An I2CBus instance</returns>
        public static I2cBus From(IMeadowIOController ioController, IPin clock, IPin data, Frequency frequency, ushort transactionTimeout = 100)
        {
            var clockChannel = clock.SupportedChannels.OfType<II2cChannelInfo>().FirstOrDefault();
            if (clockChannel == null || clockChannel.ChannelFunction != I2cChannelFunctionType.Clock)
            {
                throw new Exception($"Pin {clock.Name} does not have I2C Clock capabilities");
            }

            var dataChannel = data.SupportedChannels.OfType<II2cChannelInfo>().FirstOrDefault();
            if (dataChannel == null || dataChannel.ChannelFunction != I2cChannelFunctionType.Data)
            {
                throw new Exception($"Pin {clock.Name} does not have I2C Data capabilities");
            }

            return new I2cBus(ioController, clock, clockChannel, data, dataChannel, frequency, transactionTimeout);
        }

        /// <summary>
        /// Writes data to a specified I2C bus address and reads data back from the same address
        /// </summary>
        /// <param name="peripheralAddress">Peripheral address</param>
        /// <param name="numberOfBytesToRead">Bytes to read after writing</param>
        /// <param name="dataToWrite">Data to write</param>
        /// <returns>Data received from peripheral</returns>
        [Obsolete("This overload if WriteReadData is obsolete for performance reasons and will be removed in a future release.  Migrate to another overload.", false)]
        public byte[] WriteReadData(byte peripheralAddress, int numberOfBytesToRead, params byte[] dataToWrite)
        {
            var rxBuffer = new byte[numberOfBytesToRead];
            var rxGch = GCHandle.Alloc(rxBuffer, GCHandleType.Pinned);
            var txGch = GCHandle.Alloc(dataToWrite, GCHandleType.Pinned);

            _busSemaphore.Wait();

            try
            {
                var command = new Nuttx.UpdI2CCommand()
                {
                    Address = peripheralAddress,
                    Frequency = (int)this.Frequency.Hertz,
                    TxBufferLength = dataToWrite.Length,
                    TxBuffer = txGch.AddrOfPinnedObject(),
                    RxBufferLength = rxBuffer.Length,
                    RxBuffer = rxGch.AddrOfPinnedObject(),
                };

                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);

                if (result != 0)
                {
                    DecipherI2CError(UPD.GetLastError());
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

        /// <summary>
        /// Writes data to a specified I2C bus address and reads data back from the same address
        /// </summary>
        /// <param name="peripheralAddress">Peripheral address</param>
        /// <param name="writeBuffer">Data buffer holding the data to write</param>
        /// <param name="dataToWrite">Data buffer into which read data will go.  The size of this buffer determines the number of bytes to be read</param>
        public unsafe void WriteReadData(byte peripheralAddress, Span<byte> writeBuffer, Span<byte> readBuffer)
        {
            WriteReadData(peripheralAddress, writeBuffer, writeBuffer.Length, readBuffer, readBuffer.Length);
        }

        public unsafe void WriteReadData(byte peripheralAddress, Span<byte> writeBuffer, int writeCount, Span<byte> readBuffer, int readCount)
        {
            _busSemaphore.Wait();

            try
            {
                fixed (byte* pWrite = &writeBuffer.GetPinnableReference())
                fixed (byte* pRead = &readBuffer.GetPinnableReference())
                {
                    var command = new Nuttx.UpdI2CCommand()
                    {
                        Address = peripheralAddress,
                        Frequency = (int)this.Frequency.Hertz,
                        TxBufferLength = writeCount,
                        TxBuffer = (IntPtr)pWrite,
                        RxBufferLength = readCount,
                        RxBuffer = (IntPtr)pRead,
                    };

                    var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);

                    if (result != 0)
                    {
                        DecipherI2CError(UPD.GetLastError());
                    }
                }

            }
            finally
            {
                _busSemaphore.Release();
            }

        }

        /// <summary>
        /// Reads the specified number of bytes from a peripheral
        /// </summary>
        /// <param name="peripheralAddress">The I2C Address to read</param>
        /// <param name="numberOfBytes">The number of bytes/octets to read</param>
        /// <returns></returns>
        public unsafe byte[] ReadData(byte peripheralAddress, int numberOfBytes)
        {
            _busSemaphore.Wait();
            Span<byte> rxBuffer = stackalloc byte[numberOfBytes];

            try
            {
                fixed (byte* pData = &rxBuffer.GetPinnableReference())
                {
                    var command = new Nuttx.UpdI2CCommand()
                    {
                        Address = peripheralAddress,
                        Frequency = (int)this.Frequency.Hertz,
                        TxBufferLength = 0,
                        TxBuffer = IntPtr.Zero,
                        RxBufferLength = rxBuffer.Length,
                        RxBuffer = (IntPtr)pData
                    };

                    Output.WriteIf(_showI2cDebug, " +SendData");
                    var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);
                    Output.WriteLineIf(_showI2cDebug, $" returned {result}");
                    if (result != 0)
                    {
                        DecipherI2CError(UPD.GetLastError());
                    }

                    return rxBuffer.ToArray();
                }
            }
            finally
            {
                _busSemaphore.Release();
            }
        }

        /// <summary>
        /// Writes a number of bytes to the device.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="data">Data to be written.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WriteData(byte peripheralAddress, params byte[] data)
        {
            SendData(peripheralAddress, data);
        }

        public void WriteData(byte peripheralAddress, byte[] data, int length)
        {
            SendData(peripheralAddress, data, length);
        }

        /// <summary>
        /// Writes a number of bytes to the device.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="data">Data to be written.</param>
        public void WriteData(byte peripheralAddress, IEnumerable<byte> data)
        {
            SendData(peripheralAddress, data.ToArray());
        }

        /// <summary>
        /// Writes a number of bytes to the device.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="data">Data to be written.</param>
        public void WriteData(byte peripheralAddress, Span<byte> data)
        {
            SendData(peripheralAddress, data.ToArray());
        }

        private unsafe void SendData(byte address, Span<byte> data)
        {
            SendData(address, data, data.Length);
        }

        private unsafe void SendData(byte address, Span<byte> data, int count)
        {
            _busSemaphore.Wait();

            try
            {
                fixed (byte* pData = &data.GetPinnableReference())
                {
                    var command = new Nuttx.UpdI2CCommand()
                    {
                        Address = address,
                        Frequency = (int)this.Frequency.Hertz,
                        TxBufferLength = count,
                        TxBuffer = (IntPtr)pData,
                        RxBufferLength = 0,
                        RxBuffer = IntPtr.Zero
                    };

                    Output.WriteIf(_showI2cDebug, " +SendData");
                    var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);
                    Output.WriteLineIf(_showI2cDebug, $" returned {result}");
                    if (result != 0)
                    {
                        DecipherI2CError(UPD.GetLastError());
                    }
                }
            }
            finally
            {
                _busSemaphore.Release();
            }
        }

        private void DecipherI2CError(Nuttx.ErrorCode ec)
        {
            switch(ec)
            {
                case (Nuttx.ErrorCode)125:
                    throw new NativeException("Communication error.  Verify address and that SCL and SDA are not reversed.");
                case (Nuttx.ErrorCode)116:
                    throw new NativeException("Communication error.  Verify device is powered and that SCL is Connected.");
                case (Nuttx.ErrorCode)112:
                    throw new NativeException("Communication error.  No device found at requested address.");
                case Nuttx.ErrorCode.TryAgain:
                    throw new NativeException("Communication error.  Verify SDA Is Connected.");
                default:
                    throw new NativeException($"Communication error.  Error code {(int)ec}");
            }
        }
    }
}
