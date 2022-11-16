using Meadow.Hardware;
using System.IO;
namespace Meadow
{
    public static class RuntimeInformation
    {
        private static MeadowPlatform? _platform;

        public static bool IsPlatform(MeadowPlatform platform)
        {
            if (!_platform.HasValue)
            {
                // this is not a great way to do this, but for now....
                if (Directory.Exists("/meadow0"))
                {
                    // we're an F7
                    _platform = MeadowPlatform.F7FeatherV1;
                }
                else
                {
                    _platform = MeadowPlatform.MeadowForLinux;
                }
            }

            return platform == _platform.Value;
        }
    }
}
