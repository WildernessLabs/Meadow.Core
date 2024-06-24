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
                //#define BRKINT    (1 << 0)  /* Bit 0:  Signal interrupt on break */
                //#define ICRNL     (1 << 1)  /* Bit 1:  Map CR to NL on input */
                //#define IGNBRK    (1 << 2)  /* Bit 2:  Ignore break condition */
                //#define IGNCR     (1 << 3)  /* Bit 3:  Ignore CR */
                //#define IGNPAR    (1 << 4)  /* Bit 4:  Ignore characters with parity errors */
                //#define INLCR     (1 << 5)  /* Bit 5:  Map NL to CR on input */
                //#define INPCK     (1 << 6)  /* Bit 6:  Enable input parity check */
                //#define ISTRIP    (1 << 7)  /* Bit 7:  Strip character */
                //#define IUCLC     (1 << 8)  /* Bit 8:  Map upper-case to lower-case on input (LEGACY)*/
                //#define IXANY     (1 << 9)  /* Bit 9:  Enable any character to restart output */
                //#define IXOFF     (1 << 10) /* Bit 10: Enable start/stop input control */
                //#define IXON      (1 << 11) /* Bit 11: Enable start/stop output control */
                //#define PARMRK    (1 << 12) /* Bit 12: Mark parity errors */

                BRKINT = (1 << 0),/* Bit 0:  Signal interrupt on break */
                ICRNL = (1 << 1),/* Bit 1:  Map CR to NL on input */
                IGNBRK = (1 << 2),/* Bit 2:  Ignore break condition */
                IGNCR = (1 << 3),/* Bit 3:  Ignore CR */
                IGNPAR = (1 << 4),/* Bit 4:  Ignore characters with parity errors */
                INLCR = (1 << 5),/* Bit 5:  Map NL to CR on input */
                INPCK = (1 << 6),/* Bit 6:  Enable input parity check */
                ISTRIP = (1 << 7),/* Bit 7:  Strip character */
                IUCLC = (1 << 8),/* Bit 8:  Map upper-case to lower-case on input (LEGACY)*/
                IXANY = (1 << 9),/* Bit 9:  Enable any character to restart output */
                IXOFF = (1 << 10),/* Bit 10: Enable start/stop input control */
                IXON = (1 << 11),/* Bit 11: Enable start/stop output control */
                PARMRK = (1 << 12),/* Bit 12: Mark parity errors */
            }

            [Flags]
            public enum OutputFlags : short
            {
                //#define OPOST     (1 << 0)  /* Bit 0:  Post-process output */
                //#define OLCUC     (1 << 1)  /* Bit 1:  Map lower-case to upper-case on output (LEGACY)*/
                //#define ONLCR     (1 << 2)  /* Bit 2:  Map NL to CR-NL on output */
                //#define OCRNL     (1 << 3)  /* Bit 3:  Map CR to NL on output */
                //#define ONOCR     (1 << 4)  /* Bit 4:  No CR output at column 0 */
                //#define ONLRET    (1 << 5)  /* Bit 5:  NL performs CR function */
                //#define OFILL     (1 << 6)  /* Bit 6:  Use fill characters for delay */
                //#define NLDLY     (1 << 7)  /* Bit 7:  Select newline delays: */
                //#  define NL0     (0 << 7)  /*   Newline character type 0 */
                //#  define NL1     (1 << 7)  /*   Newline character type 1 */
                //#define CRDLY     (3 << 8)  /* Bits 8-9:  Select carriage-return delays: */
                //#  define CR0     (0 << 8)  /*   Carriage-return delay type 0 */
                //#  define CR1     (1 << 8)  /*   Carriage-return delay type 1 */
                //#  define CR2     (2 << 8)  /*   Carriage-return delay type 2 */
                //#  define CR3     (3 << 8)  /*   Carriage-return delay type 3 */
                //#define TABDLY    (3 << 10) /* Bit 10-11:  Select horizontal-tab delays: */
                //#  define TAB0    (0 << 10) /*   Horizontal-tab delay type 0 */
                //#  define TAB1    (1 << 10) /*   Horizontal-tab delay type 1 */
                //#  define TAB2    (2 << 10) /*   Horizontal-tab delay type 2 */
                //#  define TAB3    (3 << 10) /*   Expand tabs to spaces */
                //#define BSDLY     (1 << 12) /* Bit 12:  Select backspace delays: */
                //#  define BS0     (0 << 12) /*   Backspace-delay type 0 */
                //#  define BS1     (1 << 12) /*   Backspace-delay type 1 */
                //#define VTDLY     (1 << 13) /* Bit 13:  Select vertical-tab delays: */
                //#  define VT0     (0 << 13) /*   Vertical-tab delay type 0 */
                //#  define VT1     (1 << 13) /*   Vertical-tab delay type 1 */
                //#define FFDLY     (1 << 14) /* Bit 14:  Select form-feed delays: */
                //#  define FF0     (0 << 14) /*   Form-feed delay type 0 */
                //#  define FF1     (1 << 14) /*   Form-feed delay type 1 */
                OPOST = (1 << 0), /* Bit 0:  Post-process output */
                OLCUC = (1 << 1), /* Bit 1:  Map lower-case to upper-case on output (LEGACY)*/
                ONLCR = (1 << 2), /* Bit 2:  Map NL to CR-NL on output */
                OCRNL = (1 << 3), /* Bit 3:  Map CR to NL on output */
                ONOCR = (1 << 4), /* Bit 4:  No CR output at column 0 */
                ONLRET = (1 << 5), /* Bit 5:  NL performs CR function */
                OFILL = (1 << 6), /* Bit 6:  Use fill characters for delay */
                NLDLY = (1 << 7), /* Bit 7:  Select newline delays: */
                NL0 = (0 << 7), /*   Newline character type 0 */
                NL1 = (1 << 7), /*   Newline character type 1 */
                CRDLY = (3 << 8), /* Bits 8-9:  Select carriage-return delays: */
                CR0 = (0 << 8), /*   Carriage-return delay type 0 */
                CR1 = (1 << 8), /*   Carriage-return delay type 1 */
                CR2 = (2 << 8), /*   Carriage-return delay type 2 */
                CR3 = (3 << 8), /*   Carriage-return delay type 3 */
                TABDLY = (3 << 10), /* Bit 10-11:  Select horizontal-tab delays: */
                TAB0 = (0 << 10), /*   Horizontal-tab delay type 0 */
                TAB1 = (1 << 10), /*   Horizontal-tab delay type 1 */
                TAB2 = (2 << 10), /*   Horizontal-tab delay type 2 */
                TAB3 = (3 << 10), /*   Expand tabs to spaces */
                BSDLY = (1 << 12), /* Bit 12:  Select backspace delays: */
                BS0 = (0 << 12), /*   Backspace-delay type 0 */
                BS1 = (1 << 12), /*   Backspace-delay type 1 */
                VTDLY = (1 << 13), /* Bit 13:  Select vertical-tab delays: */
                VT0 = (0 << 13), /*   Vertical-tab delay type 0 */
                VT1 = (1 << 13), /*   Vertical-tab delay type 1 */
                FFDLY = (1 << 14), /* Bit 14:  Select form-feed delays: */
                FF0 = (0 << 14), /*   Form-feed delay type 0 */
                FF1 = (1 << 14), /*   Form-feed delay type 1 */
            }

            [Flags]
            public enum ControlFlags : ushort
            {
                //#define CSIZE     (3 << 0)  /* Bits 0-1: Character size: */
                //#  define CS5     (0 << 0)  /*   5 bits */
                //#  define CS6     (1 << 0)  /*   6 bits */
                //#  define CS7     (2 << 0)  /*   7 bits */
                //#  define CS8     (3 << 0)  /*   8 bits */
                //#define CSTOPB    (1 << 2)  /* Bit 2: Send two stop bits, else one */
                //#define CREAD     (1 << 3)  /* Bit 3: Enable receiver */
                //#define PARENB    (1 << 4)  /* Bit 4: Parity enable */
                //#define PARODD    (1 << 5)  /* Bit 5: Odd parity, else even */
                //#define HUPCL     (1 << 6)  /* Bit 6: Hang up on last close */
                //#define CLOCAL    (1 << 7)  /* Bit 7: Ignore modem status lines */
                //#define CCTS_OFLOW (1 << 8) /* Bit 8: CTS flow control of output */
                //#define CRTS_IFLOW (1 << 9) /* Bit 9: RTS flow control of input */
                //#define CRTSCTS   (CRTS_IFLOW | CCTS_OFLOW)

                CSIZE = (3 << 0), /* Bits 0-1: Character size: */
                CS5 = (0 << 0), /*   5 bits */
                CS6 = (1 << 0), /*   6 bits */
                CS7 = (2 << 0), /*   7 bits */
                CS8 = (3 << 0), /*   8 bits */
                CSTOPB = (1 << 2), /* Bit 2: Send two stop bits, else one */
                CREAD = (1 << 3), /* Bit 3: Enable receiver */
                PARENB = (1 << 4), /* Bit 4: Parity enable */
                PARODD = (1 << 5), /* Bit 5: Odd parity, else even */
                HUPCL = (1 << 6), /* Bit 6: Hang up on last close */
                CLOCAL = (1 << 7), /* Bit 7: Ignore modem status lines */
                CCTS_OFLOW = (1 << 8), /* Bit 8: CTS flow control of output */
                CRTS_IFLOW = (1 << 9), /* Bit 9: RTS flow control of input */
                CRTSCTS = (CRTS_IFLOW | CCTS_OFLOW)
            }

            [Flags]
            public enum LocalFlags : short
            {
                //#define ECHO      (1 << 0)  /* Bit 0:  Enable echo */
                //#define ECHOE     (1 << 1)  /* Bit 1:  Echo erase character as error-correcting backspace */
                //#define ECHOK     (1 << 2)  /* Bit 2:  Echo KILL */
                //#define ECHONL    (1 << 3)  /* Bit 3:  Echo NL */
                //#define ICANON    (1 << 4)  /* Bit 4:  Canonical input (erase and kill processing) */
                //#define IEXTEN    (1 << 5)  /* Bit 5:  Enable extended input character processing */
                //#define ISIG      (1 << 6)  /* Bit 6:  Enable signals */
                //#define NOFLSH    (1 << 7)  /* Bit 7:  Disable flush after interrupt or quit */
                //#define TOSTOP    (1 << 8)  /* Bit 8:  Send SIGTTOU for background output */
                //#define XCASE     (1 << 9)  /* Bit 9:  Canonical upper/lower presentation (LEGACY)*/

                ECHO = (1 << 0),/* Bit 0:  Enable echo */
                ECHOE = (1 << 1),/* Bit 1:  Echo erase character as error-correcting backspace */
                ECHOK = (1 << 2),/* Bit 2:  Echo KILL */
                ECHONL = (1 << 3),/* Bit 3:  Echo NL */
                ICANON = (1 << 4),/* Bit 4:  Canonical input (erase and kill processing) */
                IEXTEN = (1 << 5),/* Bit 5:  Enable extended input character processing */
                ISIG = (1 << 6),/* Bit 6:  Enable signals */
                NOFLSH = (1 << 7),/* Bit 7:  Disable flush after interrupt or quit */
                TOSTOP = (1 << 8),/* Bit 8:  Send SIGTTOU for background output */
                XCASE = (1 << 9),/* Bit 9:  Canonical upper/lower presentation (LEGACY)*/
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
                [FieldOffset(17)]
                public int c_speed;
            }

            // from nuttx termios.h
            //typedef uint32_t speed_t;   /* Used for terminal baud rates */
            //typedef uint16_t tcflag_t;  /* Used for terminal modes */
            //typedef int      cc_t;      /* Used for terminal special characters */
            //#define NCCS      9

            //* The termios structure */

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
