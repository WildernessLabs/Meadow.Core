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
    /// Represents an I2C communication channel that conforms to the ICommunicationBus
    /// contract.
    /// </summary>
    public class I2cBus : II2cBus
    {
        private bool _showI2cDebug = false;
        private SemaphoreSlim _busSemaphore = new SemaphoreSlim(1, 1);

        private IIOController IOController { get; }

        /// <summary>
        /// Bus Clock speed in Hz
        /// </summary>
        public int Frequency { get; set; }

        /// <summary>
        /// Default constructor for the I2CBus class.  This is private to prevent the
        /// developer from calling it.
        /// </summary>
        private I2cBus(
            IIOController ioController,
            IPin clock,
            II2cChannelInfo clockChannel,
            IPin data,
            II2cChannelInfo dataChannel,
            int frequencyHz,
            ushort transactionTimeout = 100)
        {
            IOController = ioController;
            Frequency = frequencyHz;
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
                throw new NativeException(error);
            }
        }

        /// <summary>
        /// Creates an I2C bus for a set of given pins and parameters
        /// </summary>
        /// <param name="ioController">The Meadow IO Controller</param>
        /// <param name="clock">Clock (SCL) pin</param>
        /// <param name="data">Data (SDA) pin</param>
        /// <param name="frequencyHz">Bus clock speed, in Hz</param>
        /// <param name="transactionTimeout">Bus transaction timeout</param>
        /// <returns>An I2CBus instance</returns>
        public static I2cBus From(IIOController ioController, IPin clock, IPin data, int frequencyHz, ushort transactionTimeout = 100)
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

            return new I2cBus(ioController, clock, clockChannel, data, dataChannel, frequencyHz, transactionTimeout);
        }

        /// <summary>
        /// Writes data to a specified I2C bus address and reads data back from the same address
        /// </summary>
        /// <param name="peripheralAddress">Peripheral address</param>
        /// <param name="numberOfBytesToRead">Bytes to read after writing</param>
        /// <param name="dataToWrite">Data to write</param>
        /// <returns>Data received from peripheral</returns>
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
                    Frequency = this.Frequency,
                    TxBufferLength = dataToWrite.Length,
                    TxBuffer = txGch.AddrOfPinnedObject(),
                    RxBufferLength = rxBuffer.Length,
                    RxBuffer = rxGch.AddrOfPinnedObject(),
                };

                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);

                if (result != 0)
                {
                    var error = UPD.GetLastError();
                    throw new NativeException(error);
                }

                // TODO: handle ioctl errors.  Common values:
                // -116 = timeout
                // -112 = address not found

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
        /// Reads the specified number of bytes from a peripheral
        /// </summary>
        /// <param name="peripheralAddress">The I2C Address to read</param>
        /// <param name="numberOfBytes">The number of bytes/octets to read</param>
        /// <returns></returns>
        public byte[] ReadData(byte peripheralAddress, int numberOfBytes)
        {
            var rxBuffer = new byte[numberOfBytes];
            var gch = GCHandle.Alloc(rxBuffer, GCHandleType.Pinned);

            _busSemaphore.Wait();

            try
            {
                var command = new Nuttx.UpdI2CCommand()
                {
                    Address = peripheralAddress,
                    Frequency = this.Frequency,
                    TxBufferLength = 0,
                    TxBuffer = IntPtr.Zero,
                    RxBufferLength = rxBuffer.Length,
                    RxBuffer = gch.AddrOfPinnedObject(),
                };

                Output.WriteIf(_showI2cDebug, " +ReadData");
                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);
                if (result != 0)
                {
                    var error = UPD.GetLastError();
                    throw new NativeException(error);
                }
                Output.WriteLineIf(_showI2cDebug, $" returned {result}");

                // TODO: handle ioctl errors.  Common values:
                // -116 = timeout

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

        private void SendData(byte address, byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);

            _busSemaphore.Wait();

            try
            {
                var command = new Nuttx.UpdI2CCommand()
                {
                    Address = address,
                    Frequency = this.Frequency,
                    TxBufferLength = data.Length,
                    TxBuffer = gch.AddrOfPinnedObject(),
                    RxBufferLength = 0,
                    RxBuffer = IntPtr.Zero
                };

                Output.WriteIf(_showI2cDebug, " +SendData");
                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);
                if (result != 0)
                {
                    var error = UPD.GetLastError();
                    throw new NativeException(error);
                }
                Output.WriteLineIf(_showI2cDebug, $" returned {result}");

                // TODO: handle ioctl errors.  Common values:
                // -116 = timeout                                           
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
    }
}
