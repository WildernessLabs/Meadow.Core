using System;
using System.Threading;
using Meadow.Hardware;

namespace Meadow.Devices
{
    /// <summary>
    /// Contract for Meadow boards.
    /// </summary>
    public interface IMeadowDevice :
        IDigitalInputOutputController,
        IBiDirectionalController,
        IAnalogInputController,
        IPwmOutputController,
        ISerialController,
        ISerialMessageController,
        ISpiController,
        II2cController,
        IIOController<IF7MicroPinout>
    {
        /// <summary>
        /// Gets the device capabilities.
        /// </summary>
        DeviceCapabilities Capabilities { get; }

        /// <summary>
        /// Sets the device time
        /// </summary>
        /// <param name="dateTime"></param>
        void SetClock(DateTime dateTime);

        /// <summary>
        /// Meadow Internal method for setting the device's primary (i.e. entry) SynchronizationContext
        /// </summary>
        /// <param name="context"></param>
        // TODO: this really should get moved to MeadowOS
        void SetSynchronizationContext(SynchronizationContext context);

    }
}
