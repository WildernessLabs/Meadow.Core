namespace Meadow;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides a base implementation for the Meadow App. Use this
/// class for Meadow applications to get strongly-typed access to the current
/// device information.
/// </summary>
public abstract class App<D> : IApp, IAsyncDisposable
        where D : class, IMeadowDevice
{
    private ExecutionContext executionContext;

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; internal set; } = default!;

    /// <inheritdoc/>
    public Dictionary<string, string> Settings { get; internal set; } = default!;

    /// <summary>
    /// Base constructor for the App class
    /// </summary>
    protected App()
    {
        executionContext = Thread.CurrentThread.ExecutionContext;

        Device = MeadowOS.CurrentDevice as D ?? throw new ArgumentException($"Current device is not {typeof(D).Name}"); // 'D' is guaranteed to be initialized and the same type
        Abort = MeadowOS.AppAbort.Token;

        Resolver.Services.Add<IApp>(this);
    }

    /// <summary>
    /// Invokes an action in the context of the applications main thread
    /// </summary>
    /// <param name="action">The action to call</param>
    /// <param name="state">An optional state object to pass to the Action</param>
    public void InvokeOnMainThread(Action<object?> action, object? state = null)
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
                ExecutionContext.Run(executionContext, new ContextCallback(action), state);
                break;
        }
    }

    /// <summary>
    /// Called by MeadowOS when everything is ready for the App to run
    /// </summary>
    public virtual Task Run()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called by MeadowOS to initialize the App
    /// </summary>
    public virtual Task Initialize() { return Task.CompletedTask; }

    /// <summary>
    /// Called when a request to shut down the App occurs
    /// </summary>
    /// <remarks>This is called by the Update Service before applying an update</remarks>
    public virtual Task OnShutdown() { return Task.CompletedTask; }

    /// <summary>
    /// Called when the MeadowOS encounters an error
    /// </summary>
    /// <param name="e">The exception from MeadowOS</param>
    public virtual Task OnError(Exception e) { return Task.CompletedTask; }

    /// <summary>
    /// Called when the application is about to update itself.
    /// </summary>
    public void OnUpdate(Version newVersion, out bool approveUpdate)
    {
        approveUpdate = true;
    }

    /// <summary>
    /// Called when the application has updated itself.
    /// </summary>
    public void OnUpdateComplete(Version oldVersion, out bool rollbackUpdate)
    {
        rollbackUpdate = false;
    }

    /// <summary>
    /// The root Device interface
    /// </summary>
    public static D Device { get; protected set; } = default!;

    /// <summary>
    /// The app cancellation token
    /// </summary>
    public static CancellationToken Abort { get; protected set; }

    /// <summary>
    /// Virtual method provided for App implementations to clean up resources on Disposal
    /// </summary>
    public virtual ValueTask DisposeAsync() { return new ValueTask(Task.CompletedTask); }
}