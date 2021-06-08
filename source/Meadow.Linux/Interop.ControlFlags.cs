using System;

namespace Meadow
{
    internal static partial class Interop
    {
        [Flags]
        public enum ControlFlags : int
        {
            /* c_cflag bit meaning */
            CBAUD = 0010017,
            B0 = 0000000,       /* hang up */
            B50 = 0000001,
            B75 = 0000002,
            B110 = 0000003,
            B134 = 0000004,
            B150 = 0000005,
            B200 = 0000006,
            B300 = 0000007,
            B600 = 0000010,
            B1200 = 0000011,
            B1800 = 0000012,
            B2400 = 0000013,
            B4800 = 0000014,
            B9600 = 0000015,
            B19200 = 0000016,
            B38400 = 0000017,
            EXTA = B19200,
            EXTB = B38400,
            CSIZE = 0000060,
            CS5 = 0000000,
            CS6 = 0000020,
            CS7 = 0000040,
            CS8 = 0000060,
            CSTOPB = 0000100,
            CREAD = 0000200,
            PARENB = 0000400,
            PARODD = 0001000,
            HUPCL = 0002000,
            CLOCAL = 0004000,
            CBAUDEX = 0010000,
            BOTHER = 0010000,
            B57600 = 0010001,
            B115200 = 0010002,
            B230400 = 0010003,
            B460800 = 0010004,
            B500000 = 0010005,
            B576000 = 0010006,
            B921600 = 0010007,
            B1000000 = 0010010,
            B1152000 = 0010011,
            B1500000 = 0010012,
            B2000000 = 0010013,
            B2500000 = 0010014,
            B3000000 = 0010015,
            B3500000 = 0010016,
            B4000000 = 0010017,
            //CIBAUD = 002003600000,  /* input baud rate */
            //CMSPAR = 010000000000,  /* mark or space (stick) parity */
            //CRTSCTS = 020000000000	/* flow control */

        }
    }
}
