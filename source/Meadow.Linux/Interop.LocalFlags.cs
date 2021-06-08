using System;

namespace Meadow
{
    internal static partial class Interop
    {
        [Flags]
        public enum LocalFlags : int
        {
            /* c_lflag bits */
            ISIG = 0000001,
            ICANON = 0000002,
            XCASE = 0000004,
            ECHO = 0000010,
            ECHOE = 0000020,
            ECHOK = 0000040,
            ECHONL = 0000100,
            NOFLSH = 0000200,
            TOSTOP = 0000400,
            ECHOCTL = 0001000,
            ECHOPRT = 0002000,
            ECHOKE = 0004000,
            FLUSHO = 0010000,
            PENDIN = 0040000,
            IEXTEN = 0100000,
            EXTPROC = 0200000
        }
    }
}
