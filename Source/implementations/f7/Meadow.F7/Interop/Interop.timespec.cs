using System;
using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
#pragma warning disable 0649
            public struct timespec
            {
                public long tv_sec;
                public long tv_nsec;
            }
#pragma warning disable 0649

            public enum clockid_t : int
            {
                CLOCK_REALTIME = 0,
                CLOCK_MONOTONIC = 1,
                CLOCK_PROCESS_CPUTIME_ID = 2,
                CLOCK_THREAD_CPUTIME_ID = 3
            }
        }
    }
}
