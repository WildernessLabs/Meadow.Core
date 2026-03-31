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

            [LibraryImport(LIBRARY_NAME, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
            public static partial int mount(string source, string target, string filesystemtype, int mountflags, IntPtr data);

            // int umount2(const char *target, int flags);
            // MNT_FORCE
            // MNT_DETACH
            [LibraryImport(LIBRARY_NAME, SetLastError = true, StringMarshalling = StringMarshalling.Utf8)]
            public static partial int umount2(string target, int flags);
        }
    }
}
