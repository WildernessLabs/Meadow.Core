using Meadow.Foundation.Displays;

namespace Meadow;

/// <summary>
/// Extension methods for registering Silk.NET/SkiaSharp display support on Linux devices.
/// </summary>
public static class SilkLinuxExtensions
{
    /// <summary>
    /// Registers a Silk.NET/SkiaSharp display factory with the device, enabling
    /// <see cref="Linux.CreateDisplay"/> to return a <see cref="SilkDisplay"/> instance.
    /// Call this during app initialization for any Linux device that needs a windowed display.
    /// </summary>
    /// <typeparam name="T">The concrete Linux device type.</typeparam>
    /// <param name="device">The Linux device instance.</param>
    /// <returns>The same device instance, for chaining.</returns>
    public static T UsesSilkDisplay<T>(this T device) where T : Linux
    {
        Linux.DisplayFactory = (width, height) => new SilkDisplay(width, height);
        return device;
    }
}
