using Meadow.Gateways;

namespace Meadow
{
    /// <summary>
    /// A contract for Meadow devices built on the STM32F7 hardware
    /// </summary>
    public interface IF7MeadowDevice : IMeadowDevice
    {
        /// <summary>
        /// The bluetooth adapter on the device
        /// </summary>
        IBluetoothAdapter? BluetoothAdapter { get; }

        /// <summary>
        /// The Coprocessor on the device
        /// </summary>
        ICoprocessor? Coprocessor { get; }
    }
}
