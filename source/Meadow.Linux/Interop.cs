using System;
using System.Runtime.InteropServices;

namespace Meadow
{
    internal static partial class Interop
    {
        public const string LIBC = "libc";

        [Flags]
        public enum DriverFlags
        {
            DontCare = 0,
            ReadOnly = 1 << 0,   // O_RDONLY
            WriteOnly = 1 << 1,  // O_WRONLY
            ReadWrite = ReadOnly | WriteOnly,   // O_RDWR
            Create = 1 << 2,
            Exclusive = 1 << 3,
            Append = 1 << 4,
            Truncate = 1 << 5,
            NonBlocking = 1 << 6,
            SynchronizeOutput = 1 << 7,
            Binary = 1 << 8,
            Direct = 1 << 9

            //#define O_RDONLY    (1 << 0)        /* Open for read access (only) */
            //#define O_RDOK      O_RDONLY        /* Read access is permitted (non-standard) */
            //#define O_WRONLY    (1 << 1)        /* Open for write access (only) */
            //#define O_WROK      O_WRONLY        /* Write access is permitted (non-standard) */
            //#define O_RDWR      (O_RDOK|O_WROK) /* Open for both read & write access */
            //#define O_CREAT     (1 << 2)        /* Create file/sem/mq object */
            //#define O_EXCL      (1 << 3)        /* Name must not exist when opened  */
            //#define O_APPEND    (1 << 4)        /* Keep contents, append to end */
            //#define O_TRUNC     (1 << 5)        /* Delete contents */
            //#define O_NONBLOCK  (1 << 6)        /* Don't wait for data */
            //#define O_NDELAY    O_NONBLOCK      /* Synonym for O_NONBLOCK */
            //#define O_SYNC      (1 << 7)        /* Synchronize output on write */
            //#define O_DSYNC     O_SYNC          /* Equivalent to OSYNC in NuttX */
            //#define O_BINARY    (1 << :sunglasses:        /* Open the file in binary (untranslated) mode. */
            //#define O_DIRECT    (1 << 9)        /* Avoid caching, write directly to hardware */
            //                /* Unsupported, but required open flags */
            //#define O_RSYNC     0               /* Synchronize input on read */
            //#define O_ACCMODE   O_RDWR          /* Mask for access mode */
            //#define O_NOCTTY    0               /* Required by POSIX */
            //#define O_TEXT      0               /* Open the file in text (translated) mode.
        }

        [DllImport(LIBC, SetLastError = true)]
        public static extern int open(string pathname, DriverFlags flags);
    }
}
