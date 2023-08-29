using System.Collections.Generic;

namespace Meadow.Simulation;

public class SimulationState
{
    public Dictionary<string, double> PinStates { get; set; } = new Dictionary<string, double>();
}
