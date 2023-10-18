using Meadow.Devices;
using System.IO;
using System.Linq;

namespace Meadow;


public partial class F7PlatformOS : IPlatformOS
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
        internal static F7StorageInformation Create(IMeadowDevice device)
        {
            long totalFlashAvailable;
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

            return new F7StorageInformation
            {
                Name = "Internal Flash",
                Size = new Units.DigitalStorage(totalFlashAvailable),
                SpaceAvailable = new Units.DigitalStorage(totalFlashAvailable - spaceConsumed)
            };
        }
    }

    /// <summary>
    /// Meadow F7-specific storage information
    /// </summary>
    public class F7StorageInformation : StorageInformation
    {
        internal static F7StorageInformation Create(IMeadowDevice device)
        {
            long totalFlashAvailable;
            if (device is F7FeatherV1)
            {
                totalFlashAvailable = 33_554_432 // 32MB
                    - 3_145_728 // allocation for runtime
                    - 2_097_152; // OtA

                // 16_510_459_314_176.10
                // 29_686_813_949_952.1MB
            }
            else
            {
                totalFlashAvailable = 67_108_864 // 64MB
                    - 3_145_728 // allocation for runtime
                    - 2_097_152; // OtA


            }

            var spaceConsumed = new DirectoryInfo("/meadow0")
                .EnumerateFiles("*", SearchOption.AllDirectories)
                .Sum(file => file.Length);

            return new F7StorageInformation
            {
                Name = "Internal Flash",
                Size = new Units.DigitalStorage(totalFlashAvailable),
                SpaceAvailable = new Units.DigitalStorage(totalFlashAvailable - spaceConsumed)
            };
        }
    }

    /// <summary>
    /// Gets the file system information for the platform.
    /// </summary>
    public Meadow.IPlatformOS.FileSystemInfo FileSystem { get; private set; } = default!;

    /// <inheritdoc/>
    public IStorageInformation[] GetStorageInformation()
    {
        // TODO: support external storage for MMC-enabled CCM devices

        return new IStorageInformation[]
            {
                F7StorageInformation.Create(Resolver.Device)
            };
    }
}
