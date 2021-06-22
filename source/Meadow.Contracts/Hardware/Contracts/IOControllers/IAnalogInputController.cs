using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for devices that expose `IAnalogInputPort(s)`.
    /// </summary>
    public interface IAnalogInputController
    {
        // TODO: if Microsoft ever gets around to fixing the compile time const
        // thing, we should make this a `Voltage` 
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
            int sampleCount = 5,
            int sampleIntervalMs = 40,
            float voltageReference = DefaultA2DReferenceVoltage
        );
    }
}
