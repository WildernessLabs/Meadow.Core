using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            [DllImport(LIBRARY_NAME, SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern IntPtr mq_open(StringBuilder name, QueueOpenFlag oflag);

            [DllImport(LIBRARY_NAME, SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern IntPtr mq_open(string name, QueueOpenFlag oflag, int mode, IntPtr attr);

            [DllImport(LIBRARY_NAME, SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern IntPtr mq_open(StringBuilder name, QueueOpenFlag oflag, int mode, ref QueueAttributes attr);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int mq_close(IntPtr mqdes);

            [DllImport(LIBRARY_NAME, SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern int mq_unlink(string name);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int mq_send(IntPtr mqdes, byte[] msg_ptr, int msg_len, ref int msg_prio);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int mq_timedsend(IntPtr mqdes, byte[] msg_ptr, int msg_len, ref int msg_prio, ref timespec abs_timeout);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int mq_timedreceive(IntPtr mqdes, byte[] msg_ptr, int msg_len, ref int msg_prio, ref timespec abs_timeout);

            [DllImport(LIBRARY_NAME, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
            public static extern int mq_receive(IntPtr mqdes, byte[] msg_ptr, int msg_len, ref int msg_prio);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int mq_notify(IntPtr mqdes, ref SigEvent sevp);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int mq_getattr(IntPtr mqdes, ref QueueAttributes attr);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int mq_setattr(IntPtr mqdes, ref QueueAttributes newattr, ref QueueAttributes oldattr);

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
    }
}