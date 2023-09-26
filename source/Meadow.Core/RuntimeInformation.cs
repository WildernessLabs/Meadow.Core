using Meadow.Hardware;
using System.IO;

namespace Meadow;

/// <summary>
/// A convenience class to retrieve information about the current runtime
/// </summary>
public static class RuntimeInformation
{
    private static MeadowPlatform? _platform;

    /// <summary>
    /// Convenience method to check the current platform
    /// </summary>
    /// <param name="platform">The MeadowPlatform to check against</param>
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
