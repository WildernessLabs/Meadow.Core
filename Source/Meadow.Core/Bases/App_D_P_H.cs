namespace Meadow;

/// <summary>
/// Provides a base implementation for the Meadow App. Use this
/// class for Meadow applications to get strongly-typed access to the current
/// device information.
/// </summary>
/// <typeparam name="D">The type of the IMeadowDevice this app targets</typeparam>
/// <typeparam name="P">The type of the IMeadowAppEmbeddedHardwareProvider to create</typeparam>
/// <typeparam name="H">The type of the IMeadowAppEmbeddedHardware the Provider will return</typeparam>
public abstract class App<D, P, H> : AppBase
    where D : class, IMeadowDevice
    where P : IMeadowAppEmbeddedHardwareProvider<H>
    where H : IMeadowAppEmbeddedHardware
{
    /// <summary>
    /// The instance if the IMeadowAppEmbeddedHardware on which the stack is running
    /// </summary>
    public static H Hardware { get; internal set; } = default!;
}
