using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Meadow
{
    [SuppressUnmanagedCodeSecurity]
    internal static class Interop
    {
        public const string LIBC = "libc";

        [Flags]
        public enum DriverFlags
        {
            //#define O_RDONLY 00
            //#define O_WRONLY 01
            //#define O_RDWR 02
            O_RDONLY =00,
            O_WRONLY =01,
            O_RDWR =02
        }

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
    }
}
