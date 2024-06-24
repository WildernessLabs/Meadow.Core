using System.Runtime.InteropServices;

namespace Meadow.Core;

internal static partial class Interop
{
    public static partial class Nuttx
    {
        //struct statfs
        //{
        //    uint32_t f_type;     /* Type of filesystem (see definitions above) */
        //    size_t f_namelen;  /* Maximum length of filenames */
        //    size_t f_bsize;    /* Optimal block size for transfers */
        //    fsblkcnt_t f_blocks;   /* Total data blocks in the file system of this size */
        //    fsblkcnt_t f_bfree;    /* Free blocks in the file system */
        //    fsblkcnt_t f_bavail;   /* Free blocks avail to non-superuser */
        //    fsfilcnt_t f_files;    /* Total file nodes in the file system */
        //    fsfilcnt_t f_ffree;    /* Free file nodes in the file system */
        //    fsid_t f_fsid;     /* Encode device type, not yet in use */
        //};

        public struct StatFs
        {
            public uint f_type { get; set; }
            public ushort f_namelen { get; set; }
            public ushort f_bsize { get; set; }
            public ushort f_blocks { get; set; }
            public ushort f_bfree { get; set; }
            public ushort f_bavail { get; set; }
            public ushort f_files { get; set; }
            public ushort f_ffree { get; set; }
            public ulong f_fsid { get; set; }
        }

        // int statfs(FAR const char *path, FAR struct statfs *buf);
        [DllImport(LIBRARY_NAME, SetLastError = true)]
        public static extern int statfs(string path, ref StatFs buf);
    }
}
