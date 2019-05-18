using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        private Queue<byte> _transmitQueue = new Queue<byte>();
        private List<byte> _receiveBuffer = new List<byte>();
        private SemaphoreSlim _busSemaphore = new SemaphoreSlim(1, 1);

        private const int SCL_PIN = 6;
        private const int SDA_PIN = 7;

        /// <summary>
        /// I2C bus used to communicate with a device (sensor etc.).
        /// </summary>
        /// <remarks>
        /// This I2CDevice is static and shared across all instances of the I2CBus.
        /// Communication with difference devices is made possible by changing the
        /// </remarks>
        private static I2cPeripheral _device;

        ///// <summary>
        ///// Configuration property for this I2CDevice.
        ///// </summary>
        //private readonly I2cPeripheral.Configuration _configuration;

        /// <summary>
        ///     Timeout for I2C transactions.
        /// </summary>
        private readonly ushort _transactionTimeout = 100;

        private IIOController IOController { get;  }

        /// <summary>
        /// Default constructor for the I2CBus class.  This is private to prevent the
        /// developer from calling it.
        /// </summary>
        private I2cBus(IIOController ioController)
        {
            IOController = ioController;
            this.Enable();
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="T:Meadow.Foundation.Core.I2CBus" /> class.
        ///// </summary>
        ///// <param name="address">Address of the device.</param>
        ///// <param name="speed">Bus speed in kHz.</param>
        ///// <param name="transactionTimeout">Transaction timeout in milliseconds.</param>
        //public I2cBus(ushort speed, ushort transactionTimeout = 100)
        //{
        //    _configuration = new I2cPeripheral.Configuration(address, speed);
        //    if (_device == null) {
        //        _device = new I2cPeripheral(_configuration);
        //    }
        //    _transactionTimeout = transactionTimeout;
        //}

        // TODO: Speed should have default?
        public static I2cBus From(IIOController ioController, IPin clock, IPin data, ushort speed, ushort transactionTimeout = 100)
        {
            return new I2cBus(ioController);
        }

        /// <summary>
        /// Write a single byte to the device.
        /// </summary>
        /// <param name="value">Value to be written (8-bits).</param>
        public void WriteByte(byte peripheralAddress, byte value)
        {
            byte[] data = { value };
            WriteBytes(peripheralAddress, data);
        }

        /// <summary>
        /// Write a number of bytes to the device.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="values">Values to be written.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WriteBytes(byte peripheralAddress, byte[] values)
        {
            SendData(peripheralAddress, values);
        }

        /// <summary>
        /// Write an unsigned short to the device.
        /// </summary>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="value">Value to be written (16-bits).</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        public void WriteUShort(byte peripheralAddress, byte address, ushort value, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = new byte[2];
            if (order == ByteOrder.LittleEndian) {
                data[0] = (byte)(value & 0xff);
                data[1] = (byte)((value >> 8) & 0xff);
            } else {
                data[0] = (byte)((value >> 8) & 0xff);
                data[1] = (byte)(value & 0xff);
            }
            WriteRegisters(peripheralAddress, address, data);
        }

        /// <summary>
        /// Write a number of unsigned shorts to the device.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written will be determined by the length of the byte array.
        /// </remarks>
        /// <param name="address">Address to write the first byte to.</param>
        /// <param name="values">Values to be written.</param>
        /// <param name="order">Indicate if the data should be written as big or little endian.</param>
        public void WriteUShorts(byte peripheralAddress, byte address, ushort[] values, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = new byte[2 * values.Length];
            for (var index = 0; index < values.Length; index++) {
                if (order == ByteOrder.LittleEndian) {
                    data[2 * index] = (byte)(values[index] & 0xff);
                    data[(2 * index) + 1] = (byte)((values[index] >> 8) & 0xff);
                } else {
                    data[2 * index] = (byte)((values[index] >> 8) & 0xff);
                    data[(2 * index) + 1] = (byte)(values[index] & 0xff);
                }
            }
            WriteRegisters(peripheralAddress, address, data);
        }

        /// <summary>
        /// Write data to the device and also read some data from the device.
        /// </summary>
        /// <remarks>
        /// The number of bytes to be written and read will be determined by the length of the byte arrays.
        /// </remarks>
        /// <param name="write">Array of bytes to be written to the device.</param>
        /// <param name="length">Amount of data to read from the device.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] WriteRead(byte peripheralAddress, byte[] write, ushort length)
        {
            //_device.Config = _configuration;
            //var read = new byte[length];
            //I2cPeripheral.I2CTransaction[] transaction =
            //{
            //    I2cPeripheral.CreateWriteTransaction(write),
            //    I2cPeripheral.CreateReadTransaction(read)
            //};
            //var bytesTransferred = 0;
            //var retryCount = 0;

            //while (_device.Execute(transaction, _transactionTimeout) != (write.Length + read.Length)) {
            //    if (retryCount > 3) {
            //        throw new Exception("WriteRead: Retry count exceeded.");
            //    }
            //    retryCount++;
            //}

            ////while (bytesTransferred != (write.Length + read.Length))
            ////{
            ////    if (retryCount > 3)
            ////    {
            ////        throw new Exception("WriteRead: Retry count exceeded.");
            ////    }
            ////    retryCount++;
            ////    bytesTransferred = _device.Execute(transaction, _transactionTimeout);
            ////}
            //return read;

            throw new NotImplementedException();
        }

        /// <summary>
        /// Write data into a single register.
        /// </summary>
        /// <param name="address">Address of the register to write to.</param>
        /// <param name="value">Value to write into the register.</param>
        public void WriteRegister(byte peripheralAddress, byte address, byte value)
        {
            byte[] data = { address, value };
            WriteBytes(peripheralAddress, data);
        }

        /// <summary>
        /// Write data to one or more registers.
        /// </summary>
        /// <remarks>
        /// This method assumes that the register address is written first followed by the data to be
        /// written into the first register followed by the data for subsequent registers.
        /// </remarks>
        /// <param name="address">Address of the first register to write to.</param>
        /// <param name="data">Data to write into the registers.</param>
        public void WriteRegisters(byte peripheralAddress, byte address, byte[] data)
        {
            var registerAndData = new byte[data.Length + 1];
            registerAndData[0] = address;
            Array.Copy(data, 0, registerAndData, 1, data.Length);
            WriteBytes(peripheralAddress, registerAndData);
        }

        /// <summary>
        ///  Read the specified number of bytes from the I2C device.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="numberOfBytes">Number of bytes.</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] ReadBytes(byte peripheralAddress, ushort numberOfBytes)
        {
            //_device.Config = _configuration;
            //var result = new byte[numberOfBytes];
            //I2cPeripheral.I2CTransaction[] transaction =
            //{
            //    I2cPeripheral.CreateReadTransaction(result)
            //};
            //var retryCount = 0;
            //while (_device.Execute(transaction, _transactionTimeout) != numberOfBytes) {
            //    if (retryCount > 3) {
            //        throw new Exception("ReadBytes: Retry count exceeded.");
            //    }
            //    retryCount++;
            //}
            //return result;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read a register from the device.
        /// </summary>
        /// <param name="address">Address of the register to read.</param>
        public byte ReadRegister(byte peripheralAddress, byte address)
        {
            byte[] registerAddress = { address };
            var result = WriteRead(peripheralAddress, registerAddress, 1);
            return result[0];
        }

        /// <summary>
        /// Read one or more registers from the device.
        /// </summary>
        /// <param name="address">Address of the first register to read.</param>
        /// <param name="length">Number of bytes to read from the device.</param>
        public byte[] ReadRegisters(byte peripheralAddress, byte address, ushort length)
        {
            byte[] registerAddress = { address };
            return WriteRead(peripheralAddress, registerAddress, length);
        }

        /// <summary>
        /// Read an usingned short from a pair of registers.
        /// </summary>
        /// <param name="address">Register address of the low byte (the high byte will follow).</param>
        /// <param name="order">Order of the bytes in the register (little endian is the default).</param>
        /// <returns>Value read from the register.</returns>
        public ushort ReadUShort(byte peripheralAddress, byte address, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = ReadRegisters(peripheralAddress, address, 2);
            ushort result = 0;
            if (order == ByteOrder.LittleEndian) {
                result = (ushort)((data[1] << 8) + data[0]);
            } else {
                result = (ushort)((data[0] << 8) + data[1]);
            }
            return result;
        }

        /// <summary>
        /// Read the specified number of unsigned shorts starting at the register
        /// address specified.
        /// </summary>
        /// <param name="address">First register address to read from.</param>
        /// <param name="number">Number of unsigned shorts to read.</param>
        /// <param name="order">Order of the bytes (Little or Big endian)</param>
        /// <returns>Array of unsigned shorts.</returns>
        public ushort[] ReadUShorts(byte peripheralAddress, byte address, ushort number, ByteOrder order = ByteOrder.LittleEndian)
        {
            var data = ReadRegisters(peripheralAddress, address, (ushort)((2 * number) & 0xffff));
            var result = new ushort[number];
            for (var index = 0; index < number; index++) {
                if (order == ByteOrder.LittleEndian) {
                    result[index] = (ushort)((data[(2 * index) + 1] << 8) + data[2 * index]);
                } else {
                    result[index] = (ushort)((data[2 * index] << 8) + data[(2 * index) + 1]);
                }
            }
            return result;
        }

        public void Reset()
        {
            uint clock_count;
            uint stretch_count;
            uint frequency;

            if (!_busSemaphore.Wait(1000))
            {
                Console.WriteLine("-I2CReset A");
                return;
            }

            try
            {
                // Save the current frequency
                frequency = Frequency;

                // TODO: De-init the port
                //  stm32_i2c_deinit(priv);

                // Use GPIO configuration to un-wedge the bus
                ConfigureGPIOs(false);

                // Let SDA go high
                (IOController as F7GPIOManager).SetDiscrete(STM32.GPIOB_BASE, STM32.GpioPort.PortB, SDA_PIN, true);

                // Clock the bus until any slaves currently driving it let it go.
                clock_count = 0;

                while (!(IOController as F7GPIOManager).GetDiscrete(STM32.GPIOB_BASE, STM32.GpioPort.PortB, SDA_PIN))
                {
                    // Give up if we have tried too hard
                    if (clock_count++ > 10)
                    {
                        Console.WriteLine("-I2CReset B");
                        return;
                    }

                    // Sniff to make sure that clock stretching has finished.
                    // If the bus never relaxes, the reset has failed.
                    stretch_count = 0;
                    while (!(IOController as F7GPIOManager).GetDiscrete(STM32.GPIOB_BASE, STM32.GpioPort.PortB, SCL_PIN))
                    {
                        // Give up if we have tried too hard
                        if (stretch_count++ > 10)
                        {
                            Console.WriteLine("-I2CReset C");
                            return;
                        }

                        Thread.Sleep(10);
                    }

                    // Drive SCL low
                    (IOController as F7GPIOManager).SetDiscrete(STM32.GPIOB_BASE, STM32.GpioPort.PortB, SCL_PIN, false);
                    Thread.Sleep(10);

                    // Drive SCL high again
                    (IOController as F7GPIOManager).SetDiscrete(STM32.GPIOB_BASE, STM32.GpioPort.PortB, SCL_PIN, true);
                    Thread.Sleep(10);

                }

                // Generate a start followed by a stop to reset slave
                // state machines
                (IOController as F7GPIOManager).SetDiscrete(STM32.GPIOB_BASE, STM32.GpioPort.PortB, SDA_PIN, false);
                Thread.Sleep(10);
                (IOController as F7GPIOManager).SetDiscrete(STM32.GPIOB_BASE, STM32.GpioPort.PortB, SCL_PIN, false);
                Thread.Sleep(10);
                (IOController as F7GPIOManager).SetDiscrete(STM32.GPIOB_BASE, STM32.GpioPort.PortB, SCL_PIN, true);
                Thread.Sleep(10);
                (IOController as F7GPIOManager).SetDiscrete(STM32.GPIOB_BASE, STM32.GpioPort.PortB, SDA_PIN, true);
                Thread.Sleep(10);

                // Revert the GPIO configuration.
                ConfigureGPIOs(true);

                // Re-init the port
                Enable();

                // Restore the frequency
                SetClock(frequency);

                return;

            }
            finally
            {
                _busSemaphore.Release();
                Console.WriteLine("-Reset");
            }
        }

        private void SetBytesToTransfer(int count)
        {
            GPD.UpdateRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR2_OFFSET, 
                STM32.I2C_CR2_NBYTES_MASK, 
                (uint)count << STM32.I2C_CR2_NBYTES_SHIFT);
        }

        private void Set7BitDestinationAddress(byte address)
        {
            GPD.UpdateRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR2_OFFSET,
                STM32.I2C_CR2_SADD7_MASK,
                (uint)address & 0x7f << STM32.I2C_CR2_SADD7_SHIFT);
        }

        private uint GetBusStatus()
        {
            return GPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_ISR_OFFSET);
        }

        private void SetEnableBit(bool enable)
        {
            if (enable)
            {
                GPD.UpdateRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR1_OFFSET,
                    0,
                    STM32.I2C_CR1_PE);
            }
            else
            {
                GPD.UpdateRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR1_OFFSET,
                    STM32.I2C_CR1_PE,
                    0);
            }
        }

        private void SetTransferDirection(bool write)
        {
            if (write)
            {
                GPD.UpdateRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR2_OFFSET,
                    STM32.I2C_CR2_RD_WRN,
                    0);
            }
            else
            {
                GPD.UpdateRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR2_OFFSET,
                    0,
                    STM32.I2C_CR2_RD_WRN);
            }
        }

        private void SetStartBit()
        {
            GPD.UpdateRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR2_OFFSET,
                0,
                STM32.I2C_CR2_START);
        }

        private void SetStopBit()
        {
            GPD.UpdateRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR2_OFFSET,
                0,
                STM32.I2C_CR2_STOP);
        }

        private void EnableReloadMode(bool reload)
        {
            if (reload)
            {
                GPD.UpdateRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR2_OFFSET,
                    0,
                    STM32.I2C_CR2_RELOAD);
            }
            else
            {
                GPD.UpdateRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR2_OFFSET,
                    STM32.I2C_CR2_RELOAD,
                    0);
            }
        }

        private void SetTxRegister(byte value)
        {
            GPD.SetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_TXDR_OFFSET, value);
        }

        private void DumpRegisters()
        {
            var cr1 = GPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR1_OFFSET);
            var cr2 = GPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR2_OFFSET);
            var timing = GPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_TIMINGR_OFFSET);
            var enr = GPD.GetRegister(STM32.RCC_BASE + STM32.RCC_APB1ENR_OFFSET);
            var rstr = GPD.GetRegister(STM32.RCC_BASE + STM32.RCC_APB1RSTR_OFFSET);

            Console.WriteLine($"I2C_CR1:{cr1:X}");
            Console.WriteLine($"I2C_CR2:{cr2:X}");
            Console.WriteLine($"I2C_TIMINGR:{timing:X}");
            Console.WriteLine($"RCC_APB1ENR:{enr:X}");
            Console.WriteLine($"RCC_APB1RSTR:{rstr:X}");


        }

        private byte SetRxRegister()
        {
            return (byte)GPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_RXDR_OFFSET);
        }

        public uint Frequency { get; private set; }

        private void SetClock(uint frequency)
        {
            uint presc, scl_delay, sda_delay, scl_h_period, scl_l_period;

            //if (Frequency == frequency) return;

            if (frequency == 100000)
            {
                presc = 0;
                scl_delay = 5;
                sda_delay = 0;
                scl_h_period = 61;
                scl_l_period = 89;

            }
            else if (frequency == 400000)
            {
                presc = 0;
                scl_delay = 3;
                sda_delay = 0;
                scl_h_period = 6;
                scl_l_period = 24;
            }
            else if (frequency == 1000000)
            {
                presc = 0;
                scl_delay = 2;
                sda_delay = 0;
                scl_h_period = 1;
                scl_l_period = 5;
            }
            else
            {
                presc = 7;
                scl_delay = 0;
                sda_delay = 0;
                scl_h_period = 35;
                scl_l_period = 162;
            }

            uint timingr =
              (presc << STM32.I2C_TIMINGR_PRESC_SHIFT) |
              (scl_delay << STM32.I2C_TIMINGR_SCLDEL_SHIFT) |
              (sda_delay << STM32.I2C_TIMINGR_SDADEL_SHIFT) |
              (scl_h_period << STM32.I2C_TIMINGR_SCLH_SHIFT) |
              (scl_l_period << STM32.I2C_TIMINGR_SCLL_SHIFT);

            Console.WriteLine($"set TIMING: {timingr:X}");
            GPD.SetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_TIMINGR_OFFSET, timingr);
            Console.WriteLine($"get TIMING: {GPD.GetRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_TIMINGR_OFFSET):X}");

            Frequency = frequency;
        }

        private void ConfigureGPIOs(bool forI2C)
        {
            if (forI2C)
            {
                // Configure pins
                // #define GPIO_I2C1_SCL_1       (GPIO_ALT|GPIO_AF4 |GPIO_SPEED_50MHz|GPIO_OPENDRAIN|GPIO_PORTB|GPIO_PIN6)
                // #define GPIO_I2C1_SDA_1       (GPIO_ALT|GPIO_AF4 |GPIO_SPEED_50MHz|GPIO_OPENDRAIN|GPIO_PORTB|GPIO_PIN7)
                (IOController as F7GPIOManager).ConfigureAlternateFunction(STM32.GpioPort.PortB,
                    SCL_PIN,
                    STM32.GPIOSpeed.Speed_50MHz,
                    STM32.OutputType.OpenDrain,
                    4);

                (IOController as F7GPIOManager).ConfigureAlternateFunction(STM32.GpioPort.PortB,
                    SDA_PIN,
                    STM32.GPIOSpeed.Speed_50MHz,
                    STM32.OutputType.OpenDrain,
                    4);
            }
            else
            { // make them outputs (we'll drive them manually, e.g. in a reset)
                (IOController as F7GPIOManager).ConfigureOutput(STM32.GpioPort.PortB,
                    SCL_PIN,
                    STM32.ResistorMode.Float,
                    STM32.GPIOSpeed.Speed_50MHz,
                    STM32.OutputType.PushPull,
                    false);

                (IOController as F7GPIOManager).ConfigureOutput(STM32.GpioPort.PortB,
                    SDA_PIN,
                    STM32.ResistorMode.Float,
                    STM32.GPIOSpeed.Speed_50MHz,
                    STM32.OutputType.PushPull,
                    false);
            }
        }

        private void Enable()
        {
            ConfigureGPIOs(true);

            // enable the peripheral clock
            GPD.UpdateRegister(STM32.RCC_BASE + STM32.RCC_APB1ENR_OFFSET, 0, STM32.RCC_APB1ENR_I2C1EN);

            // pulse the reset bit
            GPD.UpdateRegister(STM32.RCC_BASE + STM32.RCC_APB1RSTR_OFFSET, 0, STM32.RCC_APB1RSTR_I2C1RST);
            GPD.UpdateRegister(STM32.RCC_BASE + STM32.RCC_APB1RSTR_OFFSET, STM32.RCC_APB1RSTR_I2C1RST, 0);

            // The TIMINGR can only be set when the PE bit == 0
            GPD.UpdateRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR1_OFFSET, STM32.I2C_CR1_PE, 0);

            SetClock(100000);

            GPD.UpdateRegister(STM32.MEADOW_I2C1_BASE + STM32.I2C_CR1_OFFSET, 0, STM32.I2C_CR1_PE);
        }

        private void Disable()
        {
        }

        private void SendData(byte address, byte[] data)
        {
            // TODO: we need to enable the ability to restart

            if (_transmitQueue.Count > 0)
            {
                // TODO: handle this - already data to go out .  amybe a semaphore?
            }

            foreach(var b in data)
            {
                _transmitQueue.Enqueue(b);
            }

            // TODO: put this in a background thread

            var status = GetBusStatus();
            Console.WriteLine($"{status:X}");

            DumpRegisters();

            // set address, data length, direction
            SendStart(data.Length, address, true);

            DumpRegisters();

            Console.WriteLine($"Sending first byte of {_transmitQueue.Count}");

            var timeout = 0;
            while (_transmitQueue.Count > 0)
            {
                // wait for ready
                status = GetBusStatus();

                Console.WriteLine($"{status:X}");

                // while transmit empty is false
                while ((status & STM32.I2C_ISR_TXE) == 0)
                {
                    Thread.Sleep(1); // just a simple yield
                    status = GetBusStatus();

                    // TODO: add a timeout
                    if(timeout++ %100 == 0)
                    {
                        Console.WriteLine($"{status:X}");
                    }
                }

                // TODO: check for error bits

                // if this is the last byte, we'll need to set the stop condition afterward, and the byte latches it, so set the register now
                if (_transmitQueue.Count == 1)
                {
                    // send stop
                    SetStopBit();
                }

                // write the next byte to the tx register
                SetTxRegister(_transmitQueue.Dequeue());
            }
            Console.WriteLine("done");
        }

        private void SendStart(int byteCount, byte destination, bool write)
        { 
            // Set the number of bytes to transfer (I2C_CR2->NBYTES) to the number of
            // bytes in the current message or 255, whichever is lower so as to not
            // exceed the hardware maximum allowed.   
            SetBytesToTransfer(byteCount);

            // Set the (7 bit) address.
            // 10 bit addressing is not yet supported.
            Set7BitDestinationAddress(destination);

            // The flag of the current message is used to determine the direction of
            // transfer required for the current message.
            SetTransferDirection(write);

            // Set the I2C_CR2->START bit to 1 to instruct the hardware to send the
            // START condition using the address and transfer direction data entered.
            SetStartBit();


        }

        /*
        public bool I2CTransfer(byte address, byte[] data)
        {
            uint status = 0;
            uint cr1;
            uint cr2;
            int errval = 0;
            int waitrc = 0;

            // TODO: Wait for any STOP in progress (bus semaphore)
            // stm32_i2c_sem_waitstop(priv);

            // Clear any pending error interrupts
            // stm32_i2c_clearinterrupts(priv);

            // Old transfers are done

            // Reset I2C trace logic
            // stm32_i2c_tracereset(priv);

            // Set I2C clock frequency toggles I2C_CR1_PE performing a SW reset!

            stm32_i2c_setclock(priv, msgs->frequency);

            // Trigger start condition, then the process moves into the ISR.  I2C
            // interrupts will be enabled within stm32_i2c_waitdone().

            // TODO: enable interrupts and make transfers interrupt driven
            // Enable transmit and receive interrupts here so when we send the start
            // condition below the ISR will fire if the data was sent and some
            // response from the slave received.  All other interrupts relevant to
            // our needs are enabled in stm32_i2c_sem_waitdone() below.
             
            //stm32_i2c_enableinterrupts(priv);

            // Trigger START condition generation, which also sends the slave address
            // with read/write flag and the data in the first message
            //
            //  stm32_i2c_sendstart(priv);

            // Wait for the ISR to tell us that the transfer is complete by attempting
            // to grab the semaphore that is initially locked by the ISR.  If the ISR
            // does not release the lock so we can obtain it here prior to the end of
            // the timeout period waitdone returns error and we report a timeout.
            waitrc = stm32_i2c_sem_waitdone(priv);

            cr1 = stm32_i2c_getreg32(priv, STM32_I2C_CR1_OFFSET);
            cr2 = stm32_i2c_getreg32(priv, STM32_I2C_CR2_OFFSET);

            // Status after a normal / good exit is usually 0x00000001, meaning the TXE
            // bit is set.  That occurs as a result of the I2C_TXDR register being
            // empty, and it naturally will be after the last byte is transmitted.
            // This bit is cleared when we attempt communications again and re-enable
            // the peripheral.  The priv->status field can hold additional information
            // like a NACK, so we reset the status field to include that information.
            status = stm32_i2c_getstatus(priv);

            // The priv->status field can hold additional information like a NACK
            // event so we include that information.
            status = priv->status & 0xffffffff;

            if (waitrc < 0)
            {
                // Connection timed out
                errval = ETIMEDOUT;
                i2cerr("ERROR: Waitdone timed out CR1: 0x%08x CR2: 0x%08x status: 0x%08x\n",
                       cr1, cr2, status);
            }


            // Check for error status conditions
            if ((status & (I2C_INT_BERR |
                         I2C_INT_ARLO |
                         I2C_INT_OVR |
                         I2C_INT_PECERR |
                         I2C_INT_TIMEOUT |
                         I2C_INT_NACK)) != 0)
            {
                // one or more errors in the mask are present
                if (status & I2C_INT_BERR)
                {
                    // Bus Error, ignore it because of errata (revision A,Z)
                    // i2cerr("ERROR: I2C Bus Error\n");
                }
                else if (status & I2C_INT_ARLO)
                {
                    // Arbitration Lost (master mode)
                    // i2cerr("ERROR: I2C Arbitration Lost\n");
                    errval = EAGAIN;
                }
                else if (status & I2C_INT_OVR)
                {
                    // Overrun/Underrun
                    //i2cerr("ERROR: I2C Overrun/Underrun\n");
                    errval = EIO;
                }
                else if (status & I2C_INT_PECERR)
                {
                    // PEC Error in reception (SMBus Only)
                    // i2cerr("ERROR: I2C PEC Error\n");
                    errval = EPROTO;
                }
                else if (status & I2C_INT_TIMEOUT)
                {
                    // Timeout or Tlow Error (SMBus Only
                    //i2cerr("ERROR: I2C Timeout / Tlow Error\n");
                    errval = ETIME;
                }
                else if (status & I2C_INT_NACK)
                {
                    // NACK Received, flag as "communication error on send"
                    if (priv->astart == TRUE)
                    {
                        //i2cwarn("WARNING: I2C Address NACK\n");
                        errval = EADDRNOTAVAIL;
                    }
                    else
                    {
                        //i2cwarn("WARNING: I2C Data NACK\n");
                        errval = ECOMM;
                    }
                }
                else
                {
                    // Unrecognized error
                    //i2cerr("ERROR: I2C Unrecognized Error");
                    errval = EINTR;
                }
            }


            //stm32_i2c_sem_post(dev);


        }
        */       
    }
}