using System;
using System.Collections.Generic;
using Meadow.Hardware;

namespace Meadow.Devices
{
    public partial class F7Micro
    {
        public partial class F7MicroPinDefinitions : IPinDefinitions
        {

            public class F7NamedPinGroups : INamedPinGroups
            {
                protected F7MicroPinDefinitions _parent;

                public IList<NamedPinGroup> AllGroups { get; } = new List<NamedPinGroup>();

                public NamedPinGroup Com1;
                public NamedPinGroup Spi1;
                public NamedPinGroup I2c1;

                public F7NamedPinGroups(F7MicroPinDefinitions parentDefinitions)
                {
                    this.InitNamedPinGroups();
                }

                protected void InitNamedPinGroups()
                {
                    Com1 = new NamedPinGroup("COM1", new IPin[] { _parent.D12, _parent.D13 });
                    Spi1 = new NamedPinGroup("SPI1", new IPin[] { _parent.SCK, _parent.MISO, _parent.MOSI });
                    I2c1 = new NamedPinGroup("I2C1", new IPin[] { _parent.D08, _parent.D09 });
                }

            }
        }
    }
}
