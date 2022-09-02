// See https://aka.ms/new-console-template for more information
using Meadow;
using Meadow.Logging;
using Meadow.Update;

Resolver.Services.Add(new Logger(new ConsoleLogProvider()));
Resolver.Log.Loglevel = LogLevel.Trace;

Resolver.Log.Info("Updater Test Harness");

var svc = new UpdateService(new UpdateConfig());
Resolver.Services.Add<IUpdateService>(svc);
svc.ClearUpdates(); // uncomment to clear persisted info
svc.Start();

svc.OnUpdateAvailable += (updateService, info) =>
{
    Resolver.Log.Info("Update available!");

    // queue it for retreival "later"
    Task.Run(async () =>
    {
        await Task.Delay(5000);
        updateService.RetrieveUpdate(info);
    });
};

svc.OnUpdateRetrieved += (updateService, info) =>
{
    Resolver.Log.Info("Update retrieved!");

    Task.Run(async () =>
    {
        await Task.Delay(5000);
        updateService.ApplyUpdate(info);
    });
};

while (true)
{
    await Task.Delay(1000);
}
