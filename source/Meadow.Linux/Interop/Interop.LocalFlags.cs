using System;

namespace Meadow
{
    internal static partial class Interop
    {
        [Flags]
        public enum LocalFlags : int
        {
            ISIG = 0x01,
            ICANON = 0x02,
            XCASE = 0x04,
            ECHO = 0x08,
            ECHOE = 0x10,
            ECHOK = 0x20,
            ECHONL = 0x40,
            IEXTEN = 0x8000,

            /* c_lflag bits */
            // THESE VALUES ARE OCTAL -- IF YOU USE THEM, CONVERT **
            /*
            NOFLSH = 0000200,
            TOSTOP = 0000400,
            ECHOCTL = 0001000,
            ECHOPRT = 0002000,
            ECHOKE = 0004000,
            FLUSHO = 0010000,
            PENDIN = 0040000,
            EXTPROC = 0200000
            */
        }
    }
}
