using Microsoft.AspNetCore.Mvc;

internal class Program
{
    private static void Main(string[] args)
    {
        new SimulatorHost()
            .Start();

        while (true)
        {
            Thread.Sleep(1000);
        }
    }

    internal class HostAction
    {
        public string action { get; set; }
    }

    public class SimulatorHost
    {
        private Task? _runtime;

        public void Start()
        {
            var builder = WebApplication.CreateBuilder();
            var app = builder.Build();

            app.MapGet("/api", () => "Meadow Simulator Host v0.9");
            app.MapPut("/api/runtime", HandleRuntimeCommand);

            app.Run();
        }

        public void Stop()
        {
        }

        private void HandleRuntimeCommand([FromBody] HostAction action)
        {
            switch (action.action)
            {
                case "start":
                    StartRuntime();
                    break;
                case "stop":
                    StopRuntime();
                    break;
            }
        }

        private void StartRuntime()
        {
            _runtime = Task.Run(async () => await Meadow.MeadowOS.Start(new string[] { "--root", "f:\\temp\\simulator" }));
        }

        private void StopRuntime()
        {
            if (_runtime != null)
            {
                Meadow.MeadowOS.AppAbort.Cancel();
                _runtime = null;
            }
        }
    }
}