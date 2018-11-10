using System;
namespace Meadow.Devices
{
    public class F7Micro : DeviceBase
    {
        public F7Micro()
        {
            this.Capabilities = new DeviceCapabilities();

        }

        // TODO: does this work right or instantiate every time?
        public PinAddresses Pins => new PinAddresses();

        public class PinAddresses {
            public readonly byte D01 = 0x01;
            public readonly byte D02 = 0x02;
            public readonly byte D03 = 0x48;
            public readonly byte OnboardLEDRed = 0x16;
            public readonly byte OnboardLEDGreen = 0x16;
            public readonly byte OnboardLEDBlue = 0x16;
        }
    }
}
