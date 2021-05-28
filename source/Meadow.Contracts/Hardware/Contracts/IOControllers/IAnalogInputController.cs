using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for devices that expose `IAnalogInputPort(s)`.
    /// </summary>
    public interface IAnalogInputController
    {
        /// <summary>
        /// The default Analog to Digital converter reference voltage.
        /// </summary>
        public const float DefaultA2DReferenceVoltage = 3.3f;

        /// <summary>
        /// Initializes the specified pin to be an AnalogInput and returns the
        /// port used to sample the port value.
        /// </summary>
        /// <param name="pin">The pin to created the port on.</param>
        /// <param name="voltageReference">Reference maximum analog input port
        /// voltage in Volts. Default is 3.3V.</param>
        /// <returns></returns>
        IAnalogInputPort CreateAnalogInputPort(
            IPin pin,
            float voltageReference = DefaultA2DReferenceVoltage
        );
    }
}
