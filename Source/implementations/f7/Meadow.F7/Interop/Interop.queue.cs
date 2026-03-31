using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Meadow.Core
{
    // we create some structs for P/Invoke that give 0649's.  It's non-applicable for these, so ignore
#pragma warning disable 0649
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            [LibraryImport(LIBRARY_NAME, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
            public static partial IntPtr mq_open(StringBuilder name, QueueOpenFlag oflag);

            [LibraryImport(LIBRARY_NAME, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
            public static partial IntPtr mq_open(string name, QueueOpenFlag oflag, int mode, IntPtr attr);

            [LibraryImport(LIBRARY_NAME, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
            public static partial IntPtr mq_open(StringBuilder name, QueueOpenFlag oflag, int mode, ref QueueAttributes attr);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int mq_close(IntPtr mqdes);

            [LibraryImport(LIBRARY_NAME, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
            public static partial int mq_unlink(string name);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int mq_send(IntPtr mqdes, byte[] msg_ptr, int msg_len, ref int msg_prio);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int mq_timedsend(IntPtr mqdes, byte[] msg_ptr, int msg_len, ref int msg_prio, ref timespec abs_timeout);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int mq_timedreceive(IntPtr mqdes, byte[] msg_ptr, int msg_len, ref int msg_prio, ref timespec abs_timeout);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int mq_receive(IntPtr mqdes, byte[] msg_ptr, int msg_len, ref int msg_prio);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int mq_notify(IntPtr mqdes, ref SigEvent sevp);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int mq_getattr(IntPtr mqdes, ref QueueAttributes attr);

            [LibraryImport(LIBRARY_NAME, SetLastError = true)]
            public static partial int mq_setattr(IntPtr mqdes, ref QueueAttributes newattr, ref QueueAttributes oldattr);

            [Flags]
            public enum QueueOpenFlag
            {
                ReadOnly = 1 << 0,
                WriteOnly = 1 << 1,
                ReadWrite = ReadOnly | WriteOnly,
                Create = 1 << 2,
                Exclusive = 1 << 3,
                NonBlocking = 1 << 6
            }

            public struct QueueAttributes
            {
                public int mq_flags;
                public int mq_maxmsg;
                public int mq_msgsize;
                public int mq_curmsgs;
            }
        }
#pragma warning restore 0649
    }
}