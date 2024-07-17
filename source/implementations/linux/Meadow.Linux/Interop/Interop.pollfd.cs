using System;

namespace Meadow
{
    internal static partial class Interop
    {
        [Flags]
        public enum PollEvent : short
        {
            NONE = 0,
            POLLIN = 0x0001,
            POLLPRI = 0x0002,
            POLLOUT = 0x0004,
            POLLERR = 0x0008,
            POLLHUP = 0x0010,
            POLLNVAL = 0x0020
        }

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
        public struct pollfd
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
        {
            public int fd;
            public PollEvent events;
            public PollEvent revents;
        }
    }
}
