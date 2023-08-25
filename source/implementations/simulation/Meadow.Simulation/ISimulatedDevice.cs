using Meadow.Hardware;

namespace Meadow.Simulation;

public interface ISimulatedDevice<TPinDefinitions> : IMeadowDevice
    where TPinDefinitions : IPinDefinitions
{
    TPinDefinitions Pins { get; }
}
