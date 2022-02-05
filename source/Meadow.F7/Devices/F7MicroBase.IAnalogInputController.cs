using System;
using Meadow.Hardware;
using Meadow.Units;

namespace Meadow.Devices
{
    public abstract partial class F7MicroBase
    {
        /// <summary>
        /// The default resolution for analog inputs
        /// </summary>
        // TODO: should this be public?
        public const int DefaultA2DResolution = 12;

        /// <summary>
        /// Creates an `IAnalogInputPort` on the given pin. 
        /// </summary>
        /// <param name="pin">The analog input capable `IPin` on which to create the input port.</param>
        /// <param name="voltageReference">Reference voltage, in Volts, of the maximum input value.</param>
        /// <param name="sampleCount">Number of samples to take per reading. If > `1` then the port will
        /// take multiple readings and These are automatically averaged to
        /// reduce noise, a process known as _oversampling_.</param>
        /// <param name="sampleInterval">Duration in between samples when oversampling</param>
        /// <returns></returns>
        public IAnalogInputPort CreateAnalogInputPort(
            IPin pin,
            int sampleCount,
            TimeSpan sampleInterval,
            Voltage voltageReference)
        {
            return AnalogInputPort.From(
                pin, this.IoController,
                sampleCount, 
                sampleInterval,
                voltageReference);
        }

        /// <summary>
        /// Creates an `IAnalogInputPort` on the given pin. 
        /// </summary>
        /// <param name="pin">The analog input capable `IPin` on which to create the input port.</param>
        /// <param name="sampleCount">Number of samples to take per reading. If > `1` then the port will
        /// take multiple readings and These are automatically averaged to
        /// reduce noise, a process known as _oversampling_. Default is `5` samples.</param>
        /// <returns></returns>
        public IAnalogInputPort CreateAnalogInputPort(
            IPin pin,
            int sampleCount = 5)
        {
            return AnalogInputPort.From(
                pin, this.IoController,
                sampleCount);
        }
    }
}
