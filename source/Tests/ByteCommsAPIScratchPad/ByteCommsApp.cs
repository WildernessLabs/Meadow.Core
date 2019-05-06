using System;
using Meadow;
using Meadow.Hardware;
using Meadow.Devices;

namespace ByteCommsAPIScratchPad
{
    public class ByteCommsApp : AppBase<F7Micro, ByteCommsApp>
    {
        public ByteCommsApp()
        {
            //==== SPI Example

            // create the SPI bus
            ISpiBus spiBus = Device.CreateSpiBus(Device.Pins.Groups.Spi1);
            

            // add a device
            IDigitalOutputPort spiPeriphChipSelect = Device.CreateDigitalOutputPort(Device.Pins.D03);
            ISpiPeripheral spiPeriph = new SpiPeripheral(spiBus, spiPeriphChipSelect);

            // Can write to the device either way:
            spiPeriph.WriteByte(0x01);
            spiBus.WriteByte(spiPeriph.ChipSelect, 0x01);

            //==== I2C Example
            II2cBus i2cBus = Device.CreateI2cBus(Device.Pins.Groups.I2c1, 100); //TODO: should have a default speed

            // add a device
            II2cPeripheral i2cPeripheral = new I2cPeripheral(i2cBus, 39);

            // write
            //I2cPeripheral.W
        }
    }
}
