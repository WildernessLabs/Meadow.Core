using System;

namespace Meadow
{
    internal static partial class Interop
    {
        [Flags]
        public enum InputFlags : int
        {
            /* c_iflag bits */
            IGNBRK = 0000001,
            BRKINT = 0000002,
            IGNPAR = 0000004,
            PARMRK = 0000010,
            INPCK = 0000020,
            ISTRIP = 0000040,
            INLCR = 0000100,
            IGNCR = 0000200,
            ICRNL = 0000400,
            IUCLC = 0001000,
            IXON = 0002000,
            IXANY = 0004000,
            IXOFF = 0010000,
            IMAXBEL = 0020000,
            IUTF8 = 0040000
        }
    }
}
