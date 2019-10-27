using System;
using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            [Flags]
            public enum InputFlags : short
            {
                IGNBRK = 0x00000001,      /* ignore BREAK condition */
                BRKINT = 0x00000002,      /* map BREAK to SIGINTR */
                IGNPAR = 0x00000004,      /* ignore (discard) parity errors */
                PARMRK = 0x00000008,      /* mark parity and framing errors */
                INPCK = 0x00000010,      /* enable checking of parity errors */
                ISTRIP = 0x00000020,      /* strip 8th bit off chars */
                INLCR = 0x00000040,      /* map NL into CR */
                IGNCR = 0x00000080,      /* ignore CR */
                ICRNL = 0x00000100,      /* map CR to NL (ala CRMOD) */
                IXON = 0x00000200,      /* enable output flow control */
                IXOFF = 0x00000400,      /* enable input flow control */
                IXANY = 0x00000800,      /* any char will restart after stop */
                IMAXBEL = 0x00002000,      /* ring bell on input queue full */
                IUTF8 = 0x00004000,      /* maintain state for UTF-8 VERASE */

                //#define IGNBRK          0x00000001      /* ignore BREAK condition */
                //#define BRKINT          0x00000002      /* map BREAK to SIGINTR */
                //#define IGNPAR          0x00000004      /* ignore (discard) parity errors */
                //#define PARMRK          0x00000008      /* mark parity and framing errors */
                //#define INPCK           0x00000010      /* enable checking of parity errors */
                //#define ISTRIP          0x00000020      /* strip 8th bit off chars */
                //#define INLCR           0x00000040      /* map NL into CR */
                //#define IGNCR           0x00000080      /* ignore CR */
                //#define ICRNL           0x00000100      /* map CR to NL (ala CRMOD) */
                //#define IXON            0x00000200      /* enable output flow control */
                //#define IXOFF           0x00000400      /* enable input flow control */
                //#define IXANY           0x00000800      /* any char will restart after stop */
                //#if !defined(_POSIX_C_SOURCE) || defined(_DARWIN_C_SOURCE)
                //#define IMAXBEL         0x00002000      /* ring bell on input queue full */
                //#define IUTF8           0x00004000      /* maintain state for UTF-8 VERASE */
            }

            [Flags]
            public enum OutputFlags : short
            {
                OPOST = 0x00000001,     /* enable following output processing */
                ONLCR = 0x00000002,     /* map NL to CR-NL (ala CRMOD) */
                OXTABS = 0x00000004,     /* expand tabs to spaces */
                ONOEOT = 0x00000008,     /* discard EOT's (^D) on output) */

                //#define OPOST           0x00000001      /* enable following output processing */
                //#define ONLCR           0x00000002      /* map NL to CR-NL (ala CRMOD) */
                //#if !defined(_POSIX_C_SOURCE) || defined(_DARWIN_C_SOURCE)
                //#define OXTABS          0x00000004      /* expand tabs to spaces */
                //#define ONOEOT          0x00000008      /* discard EOT's (^D) on output) */
            }

            [Flags]
            public enum ControlFlags : ushort
            {
                CSIZE = 0x00000300,  /* character size mask */
                CS5 = 0x00000000,      /* 5 bits (pseudo) */
                CS6 = 0x00000100,      /* 6 bits */
                CS7 = 0x00000200,      /* 7 bits */
                CS8 = 0x00000300,      /* 8 bits */
                CSTOPB = 0x00000400,  /* send 2 stop bits */
                CREAD = 0x00000800,  /* enable receiver */
                PARENB = 0x00001000,  /* parity enable */
                PARODD = 0x00002000,  /* odd parity, else even */
                HUPCL = 0x00004000,  /* hang up on last close */
                CLOCAL = 0x00008000,  /* ignore modem status lines */

                //CCTS_OFLOW = 0x00010000,  /* CTS flow control of output */
                //CRTSCTS = (CCTS_OFLOW | CRTS_IFLOW),
                //CRTS_IFLOW = 0x00020000,   /* RTS flow control of input */
                //CDTR_IFLOW = 0x00040000,   /* DTR flow control of input */
                //CDSR_OFLOW = 0x00080000,   /* DSR flow control of output */
                //CCAR_OFLOW = 0x00100000,   /* DCD flow control of output */
                //MDMBUF = 0x00100000,   /* old name for CCAR_OFLOW */

                //#define CSIZE           0x00000300      /* character size mask */
                //#define     CS5             0x00000000      /* 5 bits (pseudo) */
                //#define     CS6             0x00000100      /* 6 bits */
                //#define     CS7             0x00000200      /* 7 bits */
                //#define     CS8             0x00000300      /* 8 bits */
                //#define CSTOPB          0x00000400      /* send 2 stop bits */
                //#define CREAD           0x00000800      /* enable receiver */
                //#define PARENB          0x00001000      /* parity enable */
                //#define PARODD          0x00002000      /* odd parity, else even */
                //#define HUPCL           0x00004000      /* hang up on last close */
                //#define CLOCAL          0x00008000      /* ignore modem status lines */
                //#if !defined(_POSIX_C_SOURCE) || defined(_DARWIN_C_SOURCE)
                //#define CCTS_OFLOW      0x00010000      /* CTS flow control of output */
                //#define CRTSCTS         (CCTS_OFLOW | CRTS_IFLOW)
                //#define CRTS_IFLOW      0x00020000      /* RTS flow control of input */
                //#define CDTR_IFLOW      0x00040000      /* DTR flow control of input */
                //#define CDSR_OFLOW      0x00080000      /* DSR flow control of output */
                //#define CCAR_OFLOW      0x00100000      /* DCD flow control of output */
                //#define MDMBUF          0x00100000      /* old name for CCAR_OFLOW */
            }

            [Flags]
            public enum LocalFlags : short
            {
                ECHO = 0x00000008,      /* enable echoing */
                ECHONL = 0x00000010,      /* echo NL even if ECHO is off */
                ISIG = 0x00000080,      /* enable signals INTR, QUIT, [D]SUSP */
                ICANON = 0x00000100,      /* canonicalize input lines */
                IEXTEN = 0x00000400,      /* enable DISCARD and LNEXT */
            }

            [StructLayout(LayoutKind.Explicit)]
            public struct Termios
            {
                [FieldOffset(0)]
                public InputFlags c_iflag;
                [FieldOffset(2)]
                public OutputFlags c_oflag;
                [FieldOffset(4)]
                public ControlFlags c_cflag;
                [FieldOffset(6)]
                public LocalFlags c_lflag;
                [FieldOffset(8)]
                public byte c_cc; // this is actually 9 bytes, but we don't use it, so whatevs
                // there are probably 3 bytes of padding right here to get dword alignment below
                [FieldOffset(20)]
                public int c_speed;
            }

            //typedef uint32_t speed_t;   /* Used for terminal baud rates */
            //typedef uint16_t tcflag_t;  /* Used for terminal modes */
            //typedef int      cc_t;      /* Used for terminal special characters */
            //#define NCCS      9

            ///* The termios structure */

            //struct termios
            //{
            //  /* Exposed fields defined by POSIX */

            //  tcflag_t  c_iflag;        /* Input modes */
            //  tcflag_t  c_oflag;        /* Output modes */
            //  tcflag_t  c_cflag;        /* Control modes */
            //  tcflag_t  c_lflag;        /* Local modes */
            //  cc_t      c_cc[NCCS];     /* Control chars */

            //  /* Implementation specific fields.  For portability reasons, these fields
            //   * should not be accessed directly, but rather through only through the
            //   * cf[set|get][o|i]speed() POSIX interfaces.
            //   */

            //  speed_t c_speed;          /* Input/output speed (non-POSIX)*/
            //};
        }
    }
}
