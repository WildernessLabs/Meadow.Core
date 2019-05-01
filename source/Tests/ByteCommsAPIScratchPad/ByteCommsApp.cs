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
            ISpiBus spiBus2 = Device.Hubs.Spi.Create(Device.Pins.Groups.Spi1);
            ISpiBus spiBus2 = Device.Hubs.Spi.Create(); // default pins
            //II2cBus i2CBus = Device.Hubs.I2c.Create();

            // add a device
            IDigitalOutputPort spiPeriphChipSelect = Device.CreateDigitalOutputPort(Device.Pins.D03);
            ISpiPeripheral spiPeriph = new SpiPeripheral(spiPeriphChipSelect);

            // hm. which, which?
            //spiBus.Peripherals.Add(spiPeriph);
            //spiBus.CreatePeripheral(spiPeriphChipSelect)
        }
    }
}
