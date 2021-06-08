using System;

namespace Meadow
{
    internal static partial class Interop
    {
        [Flags]
        public enum DriverFlags
        {
            //#define O_RDONLY 00
            //#define O_WRONLY 01
            //#define O_RDWR 02
            //#define	O_NONBLOCK	0x0004		/* no delay */
            //#define	O_FSYNC		0x0080		/* synchronous writes */
            O_RDONLY = 0x0000,
            O_WRONLY = 0x0001,
            O_RDWR = 0x0002,
            O_NONBLOCK = 0x0004,
            O_FSYNC = 0x0080,
            O_NOCTTY = 0x0400,
        }
    }
}
