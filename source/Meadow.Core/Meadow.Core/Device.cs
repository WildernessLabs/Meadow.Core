using System;
using Meadow.Hardware;

namespace Meadow
{
    public static class Device
    {
        private static DeviceCapabilities _caps;
        private static Pins _pins;

        static Device()
        {
            _caps = new DeviceCapabilities();
        }

        public static DeviceCapabilities Capabilities{
            get { return _caps; }
        }

        public static Pins Pins {
            get { return _pins; }
        }
    }
}
