using Meadow.Devices;
using Meadow.Units;
using System.IO;
using System.Linq;
using static Meadow.Core.Interop;
using static Meadow.Core.Interop.Nuttx;

namespace Meadow;


public partial class F7PlatformOS
{
    private const long FeatherV1TotalFlash = 33_554_432; // 32MB
    private const long FeatherV2TotalFlash = 33_554_432; // 32MB
    private const long RuntimeAllocationSize = 3_145_728; // 3MB
    private const long OtAAllocationSize = 2_097_152; // 2MB

    /// <summary>
    /// Meadow F7-specific storage information
    /// </summary>
    public class F7StorageInformation : StorageInformation
    {
        private F7StorageInformation(string name, DigitalStorage size, DigitalStorage available)
            : base(name, size, available)
        {
        }

        internal static F7StorageInformation Create(IMeadowDevice device)
        {
            bool statfsSuccess;
            long totalFlashAvailable;
            long totalFlashRemaining;

            StatFs statFs = new();

            try
            {
                // try to use statfs first
                var result = Nuttx.statfs("/meadow0", ref statFs);

                statfsSuccess = result >= 0;
            }
            catch
            {
                statfsSuccess = false;
            }

            if (statfsSuccess)
            {
                totalFlashAvailable = statFs.f_bsize * statFs.f_blocks;
                totalFlashRemaining = statFs.f_bsize * statFs.f_bfree;
            }
            else
            {
                // if that fails (i.e. pre 1.5 meadow), do some calculations instead
                if (device is F7FeatherV1)
                {
                    totalFlashAvailable = FeatherV1TotalFlash - RuntimeAllocationSize - OtAAllocationSize;
                }
                else
                {
                    totalFlashAvailable = FeatherV2TotalFlash - RuntimeAllocationSize - OtAAllocationSize;
                }

                var spaceConsumed = new DirectoryInfo("/meadow0")
                    .EnumerateFiles("*", SearchOption.AllDirectories)
                    .Sum(file => file.Length);

                totalFlashRemaining = totalFlashAvailable - spaceConsumed;
            }

            return new F7StorageInformation
                (
                "Internal Flash",
                new Units.DigitalStorage(totalFlashAvailable),
                new Units.DigitalStorage(totalFlashRemaining)
                );
        }
    }

    /// <summary>
    /// Gets the file system information for the platform.
    /// </summary>
    public IPlatformOS.FileSystemInfo FileSystem { get; private set; } = default!;
}
