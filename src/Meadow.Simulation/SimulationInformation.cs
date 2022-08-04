using Meadow.Hardware;

namespace Meadow.Simulation
{
    internal class SimulationInformation : IDeviceInformation
    {
        public string DeviceName { get; set; } = "Meadow Simulator";
        public string Model { get; set; }
        public MeadowPlatform Platform => MeadowPlatform.MeadowSimulation;
        public string ProcessorType => "Unknown";
        public string ProcessorSerialNumber => "SIMULATOR";
        public string ChipID => "SIM";
        public string CoprocessorType => "None";
        public string? CoprocessorOSVersion => null;
    }
}