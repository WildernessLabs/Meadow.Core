namespace Meadow.AspNetCore.Builder;

/// <summary>
/// Extension methods for Meadow service integration in a Blazor application.
/// </summary>
public static class MeadowWebApplicationExtensions
{
    /// <summary>
    /// Registers and starts the Meadow application when the host application starts.
    /// </summary>
    /// <typeparam name="TApp">The Meadow application type that implements <see cref="IApp"/>.</typeparam>
    /// <param name="app">The <see cref="WebApplication"/> instance.</param>
    public static void UseMeadow<TApp>(this WebApplication app)
        where TApp : IApp, new()
    {
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(() =>
        {
            MeadowOS.Start<TApp>();
        });
    }
}