//using System;
//using System.Collections.Generic;
//using Meadow.Hardware;

//namespace Meadow.Devices
//{
//    public partial class F7MicroV2
//    {
//        public partial class F7MicroPinDefinitions : IPinDefinitions
//        {
//            //TODO: are these even used? if they're not, let's delete them.
//            public class F7NamedPinGroups : INamedPinGroups
//            {
//                protected F7MicroPinDefinitions _parent;

//                public IList<NamedPinGroup> AllGroups { get; } = new List<NamedPinGroup>();

//                public NamedPinGroup Com1;
//                public NamedPinGroup Com4;
//                public NamedPinGroup Com7;
//                public NamedPinGroup Spi1;
//                public NamedPinGroup I2c1;

//                public F7NamedPinGroups(F7MicroPinDefinitions parentDefinitions)
//                {
//                    _parent = parentDefinitions;
//                    this.InitNamedPinGroups();
//                }

//                protected void InitNamedPinGroups()
//                {
//                    Com1 = new NamedPinGroup("COM1", new IPin[] { _parent.D12, _parent.D13 });
//                    Com4 = new NamedPinGroup("COM4", new IPin[] { _parent.D01, _parent.D00 });
//                    Com7 = new NamedPinGroup("COM7", new IPin[] { _parent.D05, _parent.D06 });
//                    Spi1 = new NamedPinGroup("SPI1", new IPin[] { _parent.SCK, _parent.COPI, _parent.CIPO });
//                    I2c1 = new NamedPinGroup("I2C1", new IPin[] { _parent.D08, _parent.D07 });
//                }

//            }
//        }
//    }
//}
