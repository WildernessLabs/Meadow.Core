using Meadow.Hardware;

namespace Meadow
{
    /// <summary>
    /// Contract for Meadow devices.
    /// </summary>
    public interface IIODevice//<P> where P : IPinDefinitions
    {
        /// <summary>
        /// Gets the device capabilities.
        /// </summary>
        DeviceCapabilities Capabilities { get; }

        //P Pins { get; }
        //IPinDefinitions Pins { get; }

        /// <summary>
        /// Gets the GPIO Manager.
        /// </summary>
        //IGPIOManager GPIOManager { get; }

        // generic?
        //IPort<C> CreatePort<P>(P portConfig); 

        // TODO: consider specializing IIODevice
        IDigitalOutputPort CreateDigitalOutputPort(
            IPin pin,bool initialState = false);

        IDigitalInputPort CreateDigitalInputPort(
            IPin pin,
            bool interruptEnabled = true,
            bool glitchFilter = false,
            ResistorMode resistorMode = ResistorMode.Disabled
            );

    }
}
