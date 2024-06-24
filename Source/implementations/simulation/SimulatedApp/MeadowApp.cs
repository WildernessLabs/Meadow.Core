
using Meadow;
using Meadow.Hardware;
using Meadow.Simulation;

public class MeadowApp : App<SimulatedMeadow<SimulatedPinout>>
{
    private IDigitalOutputPort _out1;

    public override Task Initialize()
    {
        Device.Logger.Info("Initialize hardware...");

        _out1 = Device.CreateDigitalOutputPort(Device.Pins.D00);
        return base.Initialize();
    }

    public override Task Run()
    {
        bool state = false;

        while (true)
        {
            if (MeadowOS.AppAbort.IsCancellationRequested) break;

            Device.Logger.Info($"Setting {_out1.Pin.Name} to {state}");
            _out1.State = state;
            state = !state;
            Thread.Sleep(1000);
        }

        return Task.CompletedTask;
    }

    public override Task OnShutdown()
    {
        return base.OnShutdown();
    }

}