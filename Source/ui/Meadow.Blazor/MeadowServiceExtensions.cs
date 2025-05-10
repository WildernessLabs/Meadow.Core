namespace Meadow.Blazor;

/// <summary>
/// Extension methods for Meadow service integration in a Blazor application.
/// </summary>
public static class MeadowServiceExtensions
{
    /// <summary>
    /// Registers and starts the Meadow application when the host application starts.
    /// </summary>
    /// <typeparam name="T">The Meadow application type that implements <see cref="IApp"/>.</typeparam>
    /// <param name="app">The <see cref="WebApplication"/> instance.</param>
    public static void UseMeadow<T>(this WebApplication app)
        where T : IApp, new()
    {
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(() =>
        {
            MeadowOS.Start<T>();
        });
    }
}
