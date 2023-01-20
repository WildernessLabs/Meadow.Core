using System;
using System.IO;

namespace Meadow
{
    public class F7ExternalStorage : IExternalStorage
    {
        private F7ExternalStorage(string folder)
        {
            Directory = new DirectoryInfo(folder);
        }

        public DirectoryInfo Directory { get; }

        internal static bool TryMount(string driver, string mountPoint, out F7ExternalStorage? store)
        {
            try
            {
                // ret = mount("/dev/mmcsd0", "/sdcard", "vfat", 0, NULL);
                var result = Core.Interop.Nuttx.mount(driver, mountPoint, "vfat", 0, IntPtr.Zero);
                if (result == 0)
                {
                    store = new F7ExternalStorage(mountPoint);
                    return true;
                }

                Resolver.Log.Debug($"SD Card Mount returned {result}");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Exception mounting SD card: {ex.Message}");
            }
            store = default;
            return false;
        }

        public void Eject()
        {
            var result = Core.Interop.Nuttx.umount2(this.Directory.FullName, 0);
            if (result != 0)
            {
                Resolver.Log.Warn($"SD Card unmount failure: {result}");
            }
        }
    }
}
