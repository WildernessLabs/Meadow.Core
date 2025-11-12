namespace Meadow.AspNetCore.Builder;

public static class MeadowHostApplicationExtensions
{
    public static IHostApplicationBuilder UseMeadow<TDevice>(this IHostApplicationBuilder builder)
        where TDevice : class, IMeadowDevice
    {
        var meadowTask = Task.Run(() => MeadowOS.Start<ExtensionApp<TDevice>>());
        bool initialized = false;

        // Wait for MeadowOS to start and register services
        MeadowOS.DeviceInitialized += (s, e) =>
        {
            initialized = true;
        };
        while (Resolver.Device == null && !initialized)
        {
            Thread.Sleep(100);
        }

        builder.Services.AddSingleton((s) => (TDevice)Resolver.Device);
        builder.Services.AddSingleton((s) => Resolver.Device);

        return builder;
    }

    public static IHostBuilder UseMeadow<TDevice>(this IHostBuilder builder)
        where TDevice : class, IMeadowDevice
    {
        var meadowTask = Task.Run(() => MeadowOS.Start<ExtensionApp<TDevice>>());
        bool initialized = false;

        // Wait for MeadowOS to start and register services
        MeadowOS.DeviceInitialized += (s, e) =>
        {
            initialized = true;
        };
        while (Resolver.Device == null && !initialized)
        {
            Thread.Sleep(100);
        }

        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton((s) => (TDevice)Resolver.Device);
            services.AddSingleton((s) => Resolver.Device);
        });

        return builder;
    }
}

