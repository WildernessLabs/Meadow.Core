﻿using Meadow.Devices;
using System;
using System.Linq;
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
        private readonly SemaphoreSlim _busSemaphore = new(1, 1);

        private IMeadowIOController IOController { get; }
        internal int BusNumber { get; set; } = 1;


        /// <summary>
        /// Bus Clock speed
        /// </summary>
        public I2cBusSpeed BusSpeed { get; set; }

        /// <summary>
        /// Default constructor for the I2cBus class. This is private to prevent the developer from calling it.
        /// </summary>
        private I2cBus(
            IMeadowIOController ioController,
            I2cBusSpeed busSpeed)
        {
            IOController = ioController;
            BusSpeed = busSpeed;
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
        /// <param name="busSpeed">I2C bus speed</param>
        /// <param name="transactionTimeout">Bus transaction timeout</param>
        /// <returns>An I2CBus instance</returns>
        public static I2cBus From(IMeadowIOController ioController, IPin clock, IPin data, I2cBusSpeed busSpeed, ushort transactionTimeout = 100)
        {
            var clockChannel = clock.SupportedChannels.OfType<II2cChannelInfo>().FirstOrDefault();
            if (clockChannel == null || clockChannel.ChannelFunction != I2cChannelFunctionType.Clock)
            {
                throw new Exception($"Pin {clock.Name} does not have I2C Clock capabilities");
            }

            var dataChannel = data.SupportedChannels.OfType<II2cChannelInfo>().FirstOrDefault();
            if (dataChannel == null || dataChannel.ChannelFunction != I2cChannelFunctionType.Data)
            {
                throw new Exception($"Pin {data.Name} does not have I2C Data capabilities");
            }
            var success = true;

            success &= ioController.DeviceChannelManager.ReservePin(clock, ChannelConfigurationType.I2C).Item1;
            success &= ioController.DeviceChannelManager.ReservePin(data, ChannelConfigurationType.I2C).Item1;

            return new I2cBus(ioController, busSpeed);
        }

        /// <summary>
        /// Reads bytes from a peripheral
        /// </summary>
        /// <param name="peripheralAddress">The I2C Address to read</param>
        /// <param name="readBuffer"></param>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array
        /// </remarks>
        public unsafe void Read(byte peripheralAddress, Span<byte> readBuffer)
        {
            _busSemaphore.Wait();

            try
            {
                fixed (byte* pData = readBuffer)
                {
                    var command = new Nuttx.UpdI2cCommand()
                    {
                        Address = peripheralAddress,
                        Frequency = (int)BusSpeed,
                        TxBufferLength = 0,
                        TxBuffer = IntPtr.Zero,
                        RxBufferLength = readBuffer.Length,
                        RxBuffer = (IntPtr)pData,
                        BusNumber = this.BusNumber
                    };

                    var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);
                    if (result != 0)
                    {
                        DecipherI2cError(UPD.GetLastError());
                    }
                }
            }
            finally
            {
                _busSemaphore.Release();
            }
        }

        /// <summary>
        /// Writes a number of bytes to the bus.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="data">Data to be written.</param>
        public unsafe void Write(byte peripheralAddress, Span<byte> data)
        {
            _busSemaphore.Wait();

            try
            {
                fixed (byte* pData = data)
                {
                    var command = new Nuttx.UpdI2cCommand()
                    {
                        Address = peripheralAddress,
                        Frequency = (int)this.BusSpeed,
                        TxBufferLength = data.Length,
                        TxBuffer = (IntPtr)pData,
                        RxBufferLength = 0,
                        RxBuffer = IntPtr.Zero,
                        BusNumber = this.BusNumber
                    };

                    var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);
                    if (result != 0)
                    {
                        DecipherI2cError(UPD.GetLastError());
                    }
                }
            }
            finally
            {
                _busSemaphore.Release();
            }
        }

        /// <summary>
        /// Writes data from the write buffer to a peripheral on the bus, then
        /// resets the bus and reads the return data into the read buffer.
        /// </summary>
        /// <param name="peripheralAddress">Address of the I2C peripheral.</param>
        /// <param name="writeBuffer">Buffer to read data from.</param>
        /// <param name="readBuffer">Buffer to read returning data into.</param>
        public unsafe void Exchange(byte peripheralAddress, Span<byte> writeBuffer, Span<byte> readBuffer)
        {
            _busSemaphore.Wait();
            try
            {
                fixed (byte* pWrite = writeBuffer)
                fixed (byte* pRead = readBuffer)
                {
                    var command = new Nuttx.UpdI2cCommand()
                    {
                        Address = peripheralAddress,
                        Frequency = (int)BusSpeed,
                        TxBufferLength = writeBuffer.Length,
                        TxBuffer = (IntPtr)pWrite,
                        RxBufferLength = readBuffer.Length,
                        RxBuffer = (IntPtr)pRead,
                        BusNumber = this.BusNumber
                    };

                    var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);

                    if (result != 0)
                    {
                        DecipherI2cError(UPD.GetLastError());
                    }
                }
            }
            finally
            {
                _busSemaphore.Release();
            }
        }

        private void DecipherI2cError(Nuttx.ErrorCode ec)
        {
            throw ec switch
            {
                (Nuttx.ErrorCode)125 => new NativeException("Communication error. Verify address and that SCL and SDA are not reversed."),
                (Nuttx.ErrorCode)116 => new NativeException("Communication error. Verify device is powered and that SCL is Connected."),
                (Nuttx.ErrorCode)112 => new NativeException("Communication error. No device found at requested address."),
                Nuttx.ErrorCode.TryAgain => new NativeException("Communication error. Verify SDA Is Connected."),
                _ => new NativeException($"Communication error.  Error code {(int)ec}"),
            };
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}
