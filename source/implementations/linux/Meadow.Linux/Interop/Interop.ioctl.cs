using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Meadow
{
    [SuppressUnmanagedCodeSecurity]
    internal static partial class Interop
    {
        private const int _IOC_NRBITS = 8;
        private const int _IOC_TYPEBITS = 8;

        private const int _IOC_SIZEBITS = 14;
        private const int _IOC_DIRBITS = 2;
        private const int _IOC_NRMASK = ((1 << _IOC_NRBITS) - 1);
        private const int _IOC_TYPEMASK = ((1 << _IOC_TYPEBITS) - 1);
        private const int _IOC_SIZEMASK = ((1 << _IOC_SIZEBITS) - 1);
        private const int _IOC_DIRMASK = ((1 << _IOC_DIRBITS) - 1);
        private const int _IOC_NRSHIFT = 0;
        private const int _IOC_TYPESHIFT = (_IOC_NRSHIFT + _IOC_NRBITS);
        private const int _IOC_SIZESHIFT = (_IOC_TYPESHIFT + _IOC_TYPEBITS);
        private const int _IOC_DIRSHIFT = (_IOC_SIZESHIFT + _IOC_SIZEBITS);
        private const int _IOC_NONE = 0;
        private const int _IOC_WRITE = 1;
        private const int _IOC_READ = 2;

        public static int _IOC(int dir, int type, int nr, int size)
        {
            return   ((dir) << _IOC_DIRSHIFT) |
                     ((type) << _IOC_TYPESHIFT) |
                     ((nr) << _IOC_NRSHIFT) |
                     ((size) << _IOC_SIZESHIFT);
        }

        public static int _IO(int type, int nr, int size)
        {
            return _IOC(_IOC_NONE, type, nr, 0);
        }

        public static int _IOR(int type, int nr, int size)
        {
            return _IOC(_IOC_READ, type, nr, size);
        }

        public static int _IOW(int type, int nr, int size)
        {
            return _IOC(_IOC_WRITE, type, nr, size);
        }

        public static int _IOWR(int type, int nr, int size)
        {
            return _IOC(_IOC_READ | _IOC_WRITE, type, nr, size);
        }

        [DllImport(LIBC, SetLastError = true)]
        public static extern int ioctl(int fd, int request, byte data);

        [DllImport(LIBC, SetLastError = true)]
        public unsafe static extern int ioctl(int fd, int request, byte* data);

        [DllImport(LIBC, SetLastError = true)]
        public static extern int ioctl(int fd, int request, IntPtr pData);
    }
}
