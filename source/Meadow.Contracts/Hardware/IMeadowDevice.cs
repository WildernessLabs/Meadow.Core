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
        IWatchdogController
        //IIOController<IPinDefinitions>
    {
        IPlatformOS PlatformOS { get; }

        /// <summary>
        /// Gets the device capabilities.
        /// </summary>
        DeviceCapabilities Capabilities { get; }

        /// <summary>
        /// Sets the device time
        /// </summary>
        /// <param name="dateTime"></param>
        // TODO: move this to PlatformOS, exposed via MeadowOS
        void SetClock(DateTime dateTime);

        void Initialize();

        void Reset();
    }
}
