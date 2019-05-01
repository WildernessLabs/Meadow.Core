using System;
using System.Collections.Generic;
using Meadow.Hardware;

namespace Meadow.Devices
{
    public partial class F7Micro
    {
        public partial class F7MicroPinDefinitions : IPinDefinitions
        {
            public List<NamedPinGroup> Groups { get; } = new List<NamedPinGroup>();

            public NamedPinGroup Com1;
            public NamedPinGroup Spi1;
            public NamedPinGroup I2c1;

            protected void InitGroups()
            {
                Com1 = new NamedPinGroup("COM1", new IPin[] { D12, D13 });
                Spi1 = new NamedPinGroup("SPI1", new IPin[] { this.SCK, this.MISO, this.MOSI });
                I2c1 = new NamedPinGroup("I2C1", new IPin[] { this.D08, this.D09 });
            }
        }
    }
}
