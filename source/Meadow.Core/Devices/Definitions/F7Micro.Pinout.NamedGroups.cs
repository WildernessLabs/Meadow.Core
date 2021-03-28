using System;
using System.Collections.Generic;
using Meadow.Hardware;

namespace Meadow.Devices
{
    public partial class F7Micro
    {
        public partial class Pinout
        {
            // TODO: is anyone really using these?
            public class F7NamedPinGroups : INamedPinGroups
            {
                protected Pinout _parent;

                public IList<NamedPinGroup> AllGroups { get; } = new List<NamedPinGroup>();

                public NamedPinGroup Com1 => new NamedPinGroup("COM1", new IPin[] { _parent.D12, _parent.D13 });
                public NamedPinGroup Com4 => new NamedPinGroup("COM4", new IPin[] { _parent.D01, _parent.D00 });
                public NamedPinGroup Spi1 => new NamedPinGroup("SPI1", new IPin[] { _parent.SCK, _parent.MOSI, _parent.MISO });
                public NamedPinGroup I2c1 => new NamedPinGroup("I2C1", new IPin[] { _parent.D08, _parent.D07 });

                public F7NamedPinGroups(Pinout parentDefinitions)
                {
                    _parent = parentDefinitions;

                    AllGroups.Add(Com1);
                    AllGroups.Add(Com4);
                    AllGroups.Add(Spi1);
                    AllGroups.Add(I2c1);
                }
            }
        }
    }
}
