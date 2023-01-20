using System;

namespace Meadow
{
    internal static partial class Interop
    {
        [Flags]
        public enum OutputFlags : int
        {
            OPOST = 0x01,
            /* c_oflag bits */
            // THESE VALUES ARE OCTAL -- IF YOU USE THEM, CONVERT **
            /*
            OLCUC = 0000002,
            ONLCR = 0000004,
            OCRNL = 0000010,
            ONOCR = 0000020,
            ONLRET = 0000040,
            OFILL = 0000100,
            OFDEL = 0000200,
            NLDLY = 0000400,
            NL0 = 0000000,
            NL1 = 0000400,
            CRDLY = 0003000,
            CR0 = 0000000,
            CR1 = 0001000,
            CR2 = 0002000,
            CR3 = 0003000,
            TABDLY = 0014000,
            TAB0 = 0000000,
            TAB1 = 0004000,
            TAB2 = 0010000,
            TAB3 = 0014000,
            XTABS = 0014000,
            BSDLY = 0020000,
            BS0 = 0000000,
            BS1 = 0020000,
            VTDLY = 0040000,
            VT0 = 0000000,
            VT1 = 0040000,
            FFDLY = 0100000,
            FF0 = 0000000,
            FF1 = 0100000
            */
        }
    }
}
