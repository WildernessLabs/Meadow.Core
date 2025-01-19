using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Meadow
{
    [SuppressUnmanagedCodeSecurity]
    internal static partial class Interop
    {

        [DllImport(LIBC, SetLastError = true)]
        public static extern void tzset();

        [DllImport(LIBC, SetLastError = true)]
        public static extern ref Rtc_time localtime(ref long time_t);

        [DllImport(LIBC, SetLastError = true)]
        public static extern IntPtr localtime_r(ref long time_t, ref Tm tm);

        [DllImport(LIBC, SetLastError = true)]
        public static extern IntPtr gmtime_r(ref long time_t, ref Tm tm);

        [DllImport(LIBC, SetLastError = true)]
        public static extern int clock_settime(Clock clock, ref Timespec timespec);

        public enum Clock
        {
            REALTIME = 0,
            PROCESS_CPUTIME_ID = 2,
            MONOTONIC = 3,
            THREAD_CPUTIME_ID = 4,
            UPTIME = 5,
            BOOTTIME = 6
            //#define CLOCK_REALTIME			0
            //#define CLOCK_PROCESS_CPUTIME_ID	2
            //#define CLOCK_MONOTONIC			3
            //#define CLOCK_THREAD_CPUTIME_ID		4
            //#define CLOCK_UPTIME			5
            //#define CLOCK_BOOTTIME			6
        }

        public struct Tm
        {
            public int tm_sec;
            public int tm_min;
            public int tm_hour;
            public int tm_mday;
            public int tm_mon;
            public int tm_year;
            public int tm_wday;
            public int tm_yday;
            public int tm_isdst;
            //            public long tm_gmtoff;           /* Seconds east of UTC */
            //            public string tm_zone;      /* Timezone abbreviation */
        }

        public struct Rtc_time
        {
            /*
                private struct rtc_time
                {
                    private int tm_sec;
                    private int tm_min;
                    private int tm_hour;
                    private int tm_mday;
                    private int tm_mon;
                    private int tm_year;
                    private int tm_wday; // unused
                    private int tm_yday; // unused
                    private int tm_isdst;// unused
                };
            */
            public int tm_sec;
            public int tm_min;
            public int tm_hour;
            public int tm_mday;
            public int tm_mon;
            public int tm_year;
            public int tm_wday;
            public int tm_yday;
            public int tm_isdst;

            public static Rtc_time From(DateTime dt)
            {
                return From((DateTimeOffset)dt);
            }

            public static Rtc_time From(DateTimeOffset dto)
            {
                return new Rtc_time
                {
                    tm_sec = dto.Second,
                    tm_min = dto.Minute,
                    tm_hour = dto.Hour,
                    tm_mday = dto.Day,
                    tm_mon = dto.Month,
                    tm_year = dto.Year
                };
            }
        }

        public struct Timespec
        {
            /*
                typedef	__int64_t	__time_t;
                struct timespec {
                    time_t	tv_sec;		
                    long tv_nsec;   
                };
            */
            public long tv_sec;
            public long tv_nsec;

            public static Timespec From(DateTime dt)
            {
                return From((DateTimeOffset)dt);
            }

            public static Timespec From(DateTimeOffset dto)
            {
                return new Timespec
                {
                    tv_sec = dto.ToUnixTimeSeconds(),
                    tv_nsec = dto.Millisecond * 1000000
                };
            }
        }
    }
}
