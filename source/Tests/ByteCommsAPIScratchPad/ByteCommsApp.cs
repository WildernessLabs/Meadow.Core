using System;
using Meadow;
using Meadow.Hardware;
using Meadow.Devices;
using Meadow.Hardware.Communications;

namespace ByteCommsAPIScratchPad
{
    public class ByteCommsApp : AppBase<F7Micro, ByteCommsApp>
    {
        public ByteCommsApp()
        {
            // create the SPI bus
            ISpiBus shortBus = Device.CreateSpiBus(Device.Pins.Groups.Spi1);
            //II2cBus iBus = Device.CreateI2cBus(Device.Pins.Groups.I2c1);

            // add a device
            IDigitalOutputPort spiPeriphChipSelect = Device.CreateDigitalOutputPort(Device.Pins.D03);
            ISpiPeripheral spiPeriph = new SpiPeripheral(shortBus, spiPeriphChipSelect);

            // Can write to the device either way:
            spiPeriph.WriteByte(0x01);
            shortBus.WriteByte(spiPeriph.ChipSelect, 0x01);
        }
    }
}
