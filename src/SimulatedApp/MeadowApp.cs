
using Meadow;
using Meadow.Simulation;

public class MeadowApp : App<SimulatedMeadow<SimulatedPinout>, MeadowApp>
{
    public MeadowApp()
    {
        Initialize();
    }

    void Initialize()
    {
        Device.Logger.Info("Initialize hardware...");
    }
}