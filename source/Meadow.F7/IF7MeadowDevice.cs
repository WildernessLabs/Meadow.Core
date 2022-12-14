using Meadow.Gateways;

namespace Meadow
{
    /// <summary>
    /// A contract for Meadow devices built on the STM32F7 hardware
    /// </summary>
    public interface IF7MeadowDevice : IMeadowDevice
    {
        IBluetoothAdapter? BluetoothAdapter { get; }
        ICoprocessor? Coprocessor { get; }
    }
}
