using Meadow.Units;
using System;

namespace Meadow
{
    public partial class F7PlatformOS : IPlatformOS
    {
        public bool SdCardPresent { get; private set; }

        public INtpClient NtpClient { get; }

        public F7PlatformOS()
        {
            NtpClient = new NtpClient();
        }

        public Temperature GetCpuTemperature()
        {
            throw new NotSupportedException();
        }

        public void Initialize()
        {
            SdCardPresent = TryMountSD();
        }

        private bool TryMountSD()
        {
            //Resolver.Log.Info("Looking for driver...");
            //var di = new FileInfo("/dev/mmcsd0");
            //Resolver.Log.Info($"{di.FullName} exists: {di.Exists}");
            try
            {
                // ret = mount("/dev/mmcsd0", "/sdcard", "vfat", 0, NULL);
                Resolver.Log.Trace("Mounting SD card...");
                var result = Core.Interop.Nuttx.mount("/dev/mmcsd0", "/sdcard", "vfat", 0, IntPtr.Zero);
                if (result == 0)
                {
                    Resolver.Log.Info("SD Card detected and mounted");
                    return true;
                }

                Resolver.Log.Trace($"Mount returned {result}");
                Resolver.Log.Info("No SD Card detected");
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Exception mounting SD card: {ex.Message}");
            }

            return false;
        }
    }
}
