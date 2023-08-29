using Meadow.Units;

namespace Meadow.Simulation;

internal abstract class SimulationEnvironment
{
    internal static Voltage ActiveVoltage = new Voltage(3.3d);
    internal static Voltage InactiveVoltage = new Voltage(0d);
}
