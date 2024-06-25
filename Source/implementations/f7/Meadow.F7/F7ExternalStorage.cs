using Meadow.Units;
using System;
using System.IO;

namespace Meadow;

/// <summary>
/// External/pluggable storage volume for the F7 platform
/// </summary>
public class F7ExternalStorage : IExternalStorage
{
    private DirectoryInfo _directory;

    private F7ExternalStorage(DirectoryInfo directory)
    {
        _directory = directory;
        Name = directory.Name;

        // TODO: get size and available space
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public DigitalStorage SpaceAvailable => throw new NotSupportedException();

    /// <inheritdoc/>
    public DigitalStorage Size => throw new NotSupportedException();

    internal static bool TryMount(string driver, string mountPoint, out F7ExternalStorage store)
    {
        try
        {
            // ret = mount("/dev/mmcsd0", "/sdcard", "vfat", 0, NULL);
            var result = Core.Interop.Nuttx.mount(driver, mountPoint, "vfat", 0, IntPtr.Zero);
            if (result == 0)
            {
                store = new F7ExternalStorage(new DirectoryInfo(mountPoint));
                return true;
            }

            Resolver.Log.Debug($"SD Card Mount returned {result}");
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Exception mounting SD card: {ex.Message}");
        }
        store = default!;
        return false;
    }

    /// <inheritdoc/>
    public void Eject()
    {
        var result = Core.Interop.Nuttx.umount2(_directory.FullName, 0);
        if (result != 0)
        {
            Resolver.Log.Warn($"SD Card unmount failure: {result}");
        }
    }
}
