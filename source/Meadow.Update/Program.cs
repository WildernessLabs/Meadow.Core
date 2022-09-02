// See https://aka.ms/new-console-template for more information
using Meadow;
using Meadow.Logging;

Resolver.Services.Add(new Logger(new ConsoleLogProvider()));
Resolver.Log.Loglevel = LogLevel.Trace;

Resolver.Log.Info("Updater Test Harness");

var svc = new UpdateService(new UpdateConfig());
Resolver.Services.Add<IUpdateService>(svc);

svc.Start();

while (true)
{
    await Task.Delay(1000);
}
