
namespace Meadow.Blazor;

public static class MeadowServiceExtensions
{
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
