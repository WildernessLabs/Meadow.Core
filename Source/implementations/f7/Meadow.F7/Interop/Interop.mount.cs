using System;
using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            // int mount(const char *source, const char *target,
            //     const char *filesystemtype, unsigned long mountflags,
            //     const void *data);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int mount(string source, string target, string filesystemtype, int mountflags, IntPtr data);

            // int umount2(const char *target, int flags);
            // MNT_FORCE 
            // MNT_DETACH 
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int umount2(string target, int flags);
        }
    }
}
