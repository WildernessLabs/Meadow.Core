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
        internal int BusNumber { get; set; } = 1;

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
        /// <param name="busSpeed">I2C bus speed</param>
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

        //==== NEW HOTNESS

        /// <summary>
        /// Reads bytes from a peripheral.
        /// </summary>
        /// <param name="peripheralAddress">The I2C Address to read</param>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length
        /// of the byte array.
        /// </remarks>
        /// <returns></returns>
        public unsafe void Read(byte peripheralAddress, Span<byte> readBuffer)
        {
            _busSemaphore.Wait();

            try {
                fixed (byte* pData = readBuffer) {
                    var command = new Nuttx.UpdI2CCommand() {
                        Address = peripheralAddress,
                        Frequency = (int)this.Frequency.Hertz,
                        TxBufferLength = 0,
                        TxBuffer = IntPtr.Zero,
                        RxBufferLength = readBuffer.Length,
                        RxBuffer = (IntPtr)pData,
                        BusNumber = this.BusNumber
                    };

                    Output.WriteIf(_showI2cDebug, " +SendData");
                    var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);
                    Output.WriteLineIf(_showI2cDebug, $" returned {result}");
                    if (result != 0) {
                        DecipherI2CError(UPD.GetLastError());
                    }
                }
            } finally {
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

            try {
                fixed (byte* pData = data) {
                    var command = new Nuttx.UpdI2CCommand() {
                        Address = peripheralAddress,
                        Frequency = (int)this.Frequency.Hertz,
                        TxBufferLength = data.Length,
                        TxBuffer = (IntPtr)pData,
                        RxBufferLength = 0,
                        RxBuffer = IntPtr.Zero,
                        BusNumber = this.BusNumber
                    };

                    Output.WriteIf(_showI2cDebug, " +Write");
                    var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);
                    Output.WriteLineIf(_showI2cDebug, $" returned {result}");
                    if (result != 0) {
                        DecipherI2CError(UPD.GetLastError());
                    }
                }
            } finally {
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
            try {
                fixed (byte* pWrite = writeBuffer)
                fixed (byte* pRead = readBuffer) {
                    var command = new Nuttx.UpdI2CCommand() {
                        Address = peripheralAddress,
                        Frequency = (int)this.Frequency.Hertz,
                        TxBufferLength = writeBuffer.Length,
                        TxBuffer = (IntPtr)pWrite,
                        RxBufferLength = readBuffer.Length,
                        RxBuffer = (IntPtr)pRead,
                        BusNumber = this.BusNumber
                    };

                    var result = UPD.Ioctl(Nuttx.UpdIoctlFn.I2CData, ref command);

                    if (result != 0) {
                        DecipherI2CError(UPD.GetLastError());
                    }
                }
            } finally {
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
