using System;
using Meadow;
using Meadow.Hardware;
using Meadow.Devices;

namespace ByteCommsAPIScratchPad
{
    public class ByteCommsApp : App<F7Micro, ByteCommsApp>
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

            // TODO: This code needs to be fixed, commented to unbreak the build - Joao
            //spiBus.WriteByte(spiPeriph.ChipSelect, 0x01);

            //==== I2C Example
            II2cBus i2cBus = Device.CreateI2cBus(Device.Pins.Groups.I2c1, 100); //TODO: should have a default speed

            // add a device
            II2cPeripheral i2cPeripheral = new I2cPeripheral(i2cBus, 39);

            // write (same as SPI, can write directly to the peripheral or via the bus)
            i2cPeripheral.WriteByte(0x01);
            //i2cBus.WriteByte(i2cPeripheral.Address, 0x01);

            // accessing the device from the core:
            IPin[] spiPins;
            switch (MeadowOS.CurrentDevice) {
                case F7Micro f7:
                    spiPins = f7.Pins.Groups.Spi1;
                    // do whatever
                    break;
            }
        }
    }
}
