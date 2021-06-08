using System;

namespace Meadow
{
    internal static partial class Interop
    {
        [Flags]
        public enum InputFlags : int
        {
            /* c_iflag bits */
            IGNBRK = 0x01,
            BRKINT = 0x02,
            IGNPAR = 0x04,
            PARMRK = 0x08,
            INPCK = 0x10,
            ISTRIP = 0x20,
            INLCR = 0x40,
            IGNCR = 0x80,
            ICRNL = 0x100,
            IUCLC = 0x200,
            IXON = 0x400,
            IXANY = 0x800,
            /*
            IXOFF = 0010000,
            IMAXBEL = 0020000,
            IUTF8 = 0040000
            */
        }
    }
}
