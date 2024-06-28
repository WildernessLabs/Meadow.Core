namespace Meadow;

using System;


/// <summary>
/// Provides a base implementation for the Meadow App. Use this
/// class for Meadow applications to get strongly-typed access to the current
/// device information.
/// </summary>
public abstract class App<D> : AppBase
        where D : class, IMeadowDevice
{
    /// <summary>
    /// The root Device interface
    /// </summary>
    public static D Device { get; protected set; } = default!;

    /// <summary>
    /// Base constructor for the App class
    /// </summary>
    public App()
    {
        Device = MeadowOS.CurrentDevice as D ?? throw new ArgumentException($"Current device is not {typeof(D).Name}"); // 'D' is guaranteed to be initialized and the same type
    }

    /// <inheritdoc/>
    public override void InvokeOnMainThread(Action<object?> action, object? state = null)
    {
        switch (Device.Information.Platform)
        {
            // ExecutionContext in Mono on the F7 isn't fully working - but we also don't worry about a MainThread there either
            case Hardware.MeadowPlatform.F7FeatherV1:
            case Hardware.MeadowPlatform.F7FeatherV2:
            case Hardware.MeadowPlatform.F7CoreComputeV2:
                action.Invoke(state);
                break;
            default:
                base.InvokeOnMainThread(action, state);
                break;
        }
    }
}