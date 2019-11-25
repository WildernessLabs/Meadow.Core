using System;
using System.Text;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace SPITest
{
    public class SampleSpi : SpiPeripheral
    {
        public SampleSpi(ISpiBus bus, IDigitalOutputPort chipSelect) : base(bus, chipSelect)
        {
        }
    }

    public class SPITestApplication : App<F7Micro, SPITestApplication>
    {
        private readonly IDigitalOutputPort _chipSelect;
        private readonly ISpiBus _spiBus;
        private SampleSpi _peripheral;

        public SPITestApplication()
        {
            Console.WriteLine($"SPI TEST");

            _chipSelect = Device.CreateDigitalOutputPort(Device.Pins.D04);
            _spiBus = Device.CreateSpiBus();
            _peripheral = new SampleSpi(_spiBus, _chipSelect);
        }

        public void Run()
        {
            var result = _peripheral.ReadRegister(0x42);

            Console.WriteLine($"Read result: {result}");
        }
    }
}
