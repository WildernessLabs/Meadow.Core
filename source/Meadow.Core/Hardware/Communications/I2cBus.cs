using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Meadow.Devices;
using static Meadow.Core.Interop;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents an I2C communication channel that conforms to the ICommunicationBus
    /// contract.
    /// </summary>
    public class I2cBus : II2cBus
    {
        private SemaphoreSlim _busSemaphore = new SemaphoreSlim(1, 1);

        private IIOController IOController { get; }

        public uint Frequency { get; private set; }

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
          ushort speed,
            ushort transactionTimeout = 100)
        {
            IOController = ioController;
        }

        private void Disable()
        {
            UPD.Ioctl(Nuttx.UpdIoctlFn.I2CShutdown);
        }

        // TODO: Speed should have default?
        public static I2cBus From(IIOController ioController, IPin clock, IPin data, ushort speed, ushort transactionTimeout = 100)
        {
            var clockChannel = clock.SupportedChannels.OfType<II2cChannelInfo>().First();
            if (clockChannel == null || clockChannel.ChannelFunction != I2cChannelFunctionType.Clock)
            {
                throw new Exception($"Pin {clock.Name} does not have I2C Clock capabilities");
            }

            var dataChannel = data.SupportedChannels.OfType<II2cChannelInfo>().First();
            if (dataChannel == null || dataChannel.ChannelFunction != I2cChannelFunctionType.Data)
            {
                throw new Exception($"Pin {clock.Name} does not have I2C Data capabilities");
            }

            return new I2cBus(ioController, clock, clockChannel, data, dataChannel, speed, transactionTimeout);
        }

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
                    Frequency = (int)this.Frequency,
                    TxBufferLength = dataToWrite.Length,
                    TxBuffer = txGch.AddrOfPinnedObject(),
                    RxBufferLength = rxBuffer.Length,
                    RxBuffer = rxGch.AddrOfPinnedObject(),
                };

                //                Console.Write($" +WriteReadData. Sending {dataToWrite.Length} bytes, requesting {byteCountToRead}");
                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);

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
                    Frequency = (int)this.Frequency,
                    TxBufferLength = 0,
                    TxBuffer = IntPtr.Zero,
                    RxBufferLength = rxBuffer.Length,
                    RxBuffer = gch.AddrOfPinnedObject(),
                };

                Console.Write(" +ReadData");
                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);
                Console.WriteLine($" returned {result}");

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
        /// Write a number of bytes to the device.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="values">Values to be written.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WriteData(byte peripheralAddress, params byte[] values)
        {
            SendData(peripheralAddress, values);
        }

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
                    Frequency = (int)this.Frequency,
                    TxBufferLength = data.Length,
                    TxBuffer = gch.AddrOfPinnedObject(),
                    RxBufferLength = 0,
                    RxBuffer = IntPtr.Zero
                };

                Console.Write(" +SendData");
                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);
                Console.WriteLine($" returned {result}");

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
