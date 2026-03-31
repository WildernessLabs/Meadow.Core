using System.Runtime.InteropServices;
using System.Security;

namespace Meadow
{
    [SuppressUnmanagedCodeSecurity]
    internal static partial class Interop
    {
        public const string LIBC = "libc";

        public const int TCSANOW = 0;
        public const int TCSADRAIN = 1;
        public const int TCSAFLUSH = 2;

        [LibraryImport(LIBC, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
        public static partial int open(string pathname, DriverFlags flags);

        [LibraryImport(LIBC, SetLastError = true)]
        public static partial int close(int handle);

        [LibraryImport(LIBC, SetLastError = true)]
        public static partial int write(int handle, byte[] buf, int count);

        [LibraryImport(LIBC, SetLastError = true)]
        public static unsafe partial int write(int handle, byte* buf, int count);

        [LibraryImport(LIBC, SetLastError = true)]
        public static partial int read(int handle, byte[] buf, int count);

        [LibraryImport(LIBC, SetLastError = true)]
        public static unsafe partial int read(int handle, byte* buf, int count);

        //int tcgetattr(int fildes, struct termios *termios_p);
        [LibraryImport(LIBC, SetLastError = true)]
        public static partial int tcgetattr(int fd, ref termios termios_p);

        // int tcsetattr(int fildes, int optional_actions, const struct termios *termios_p);
        [LibraryImport(LIBC, SetLastError = true)]
        public static partial int tcsetattr(int fd, int optional_actions, ref termios termios_p);

        [LibraryImport(LIBC, SetLastError = true)]
        public static partial int cfsetspeed(ref termios termiosp, int speed);

        [LibraryImport(LIBC, SetLastError = true)]
        public static partial int lseek(int fd, int offset, SeekWhence whence);

        [LibraryImport(LIBC, SetLastError = true)]
        public static partial int poll(pollfd[] fds, int nfds, int timeout);

        [LibraryImport(LIBC, SetLastError = true)]
        public static partial int poll(ref pollfd fds, int nfds, int timeout);

        public enum SeekWhence
        {
            SEEK_SET = 0, /* set file offset to offset */
            SEEK_CUR = 1, /* set file offset to current plus offset */
            SEEK_END = 2 /* set file offset to EOF plus offset */
        }

        public enum Errors
        {
            None = 0,
            NoPermissions = 1,
            NoSuchFile = 2,
            BadFileNumber = 9,
            DeviceBusy = 16
        }
    }
}
