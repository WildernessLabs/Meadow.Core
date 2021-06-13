using System;
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

        [DllImport(LIBC, SetLastError = true)]
        public static extern int open(string pathname, DriverFlags flags);

        [DllImport(LIBC, SetLastError = true)]
        public static extern int close(int handle);
        
        [DllImport(LIBC, SetLastError = true)]
        public static extern int write(int handle, byte[] buf, int count);

        [DllImport(LIBC, SetLastError = true)]
        public unsafe static extern int write(int handle, byte* buf, int count);

        [DllImport(LIBC, SetLastError = true)]
        public static extern int read(int handle, byte[] buf, int count);

        [DllImport(LIBC, SetLastError = true)]
        public unsafe static extern int read(int handle, byte* buf, int count);

        [DllImport(LIBC, SetLastError = true)]
        public static extern int ioctl(int fd, int request, byte data);

        [DllImport(LIBC, SetLastError = true)]
        public static extern int ioctl(int fd, int request, IntPtr pData);

        //int tcgetattr(int fildes, struct termios *termios_p);
        [DllImport(LIBC, SetLastError = true)]
        public static extern int tcgetattr(int fd, ref termios termios_p);

        // int tcsetattr(int fildes, int optional_actions, const struct termios *termios_p);
        [DllImport(LIBC, SetLastError = true)]
        public static extern int tcsetattr(int fd, int optional_actions, ref termios termios_p);

        [DllImport(LIBC, SetLastError = true)]
        public static extern int cfsetspeed(ref termios termiosp, int speed);

        [DllImport(LIBC, SetLastError = true)]
        public static extern int lseek(int fd, int offset, SeekWhence whence);
        
        [DllImport(LIBC, SetLastError = true)]
        public static extern int poll(pollfd[] fds, int nfds, int timeout);

        [DllImport(LIBC, SetLastError = true)]
        public static extern int poll(ref pollfd fds, int nfds, int timeout);

        public enum SeekWhence
        {
            SEEK_SET = 0, /* set file offset to offset */
            SEEK_CUR = 1, /* set file offset to current plus offset */
            SEEK_END = 2 /* set file offset to EOF plus offset */
        }
    }
}
