using Meadow.Hardware;
using Meadow.Units;
using System;

namespace Meadow.Devices;

public abstract partial class F7MicroBase
{
    /// <summary>
    /// The default resolution for analog inputs
    /// </summary>
    public const int DefaultA2DResolution = 12;

    /// <summary>
    /// Creates an `IAnalogInputPort` on the given pin. 
    /// </summary>
    /// <param name="pin">The analog input capable `IPin` on which to create the input port.</param>
    /// <param name="sampleCount">Number of samples to take per reading. If > `1` then the port will
    /// take multiple readings and These are automatically averaged to
    /// reduce noise, a process known as _oversampling_.</param>
    /// <param name="sampleInterval">Duration in between samples when oversampling</param>
    /// <param name="voltageReference">Reference voltage, in Volts, of the maximum input value.</param>
    public IObservableAnalogInputPort CreateAnalogInputPort(
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

    /// <inheritdoc/>
    public IAnalogInputPort CreateAnalogInputPort(
        IPin pin,
        Voltage? referenceVoltage)
    {
        return CreateAnalogInputPort(
            pin,
            1,
            AnalogInputPort.DefaultSampleInterval,
            AnalogInputPort.DefaultReferenceVoltage);
    }

    /// <summary>
    /// Creates an `IAnalogInputPort` on the given pin. 
    /// </summary>
    /// <param name="pin">The analog input capable `IPin` on which to create the input port.</param>
    /// <param name="sampleCount">Number of samples to take per reading. If > `1` then the port will
    /// take multiple readings and These are automatically averaged to
    /// reduce noise, a process known as _oversampling_. Default is `5` samples.</param>
    public IObservableAnalogInputPort CreateAnalogInputPort(
        IPin pin,
        int sampleCount = 5)
    {
        return CreateAnalogInputPort(
            pin,
            sampleCount,
            AnalogInputPort.DefaultSampleInterval,
            AnalogInputPort.DefaultReferenceVoltage);
    }

    /// <summary>
    /// Creates an `IAnalogInputPort` on the given pin. 
    /// </summary>
    /// <param name="pin">The analog input capable `IPin` on which to create the input port.</param>
    /// <param name="sampleCount">Number of samples to take per reading. If > `1` then the port will
    /// take multiple readings and These are automatically averaged to
    /// reduce noise, a process known as _oversampling_.</param>
    /// <param name="sampleInterval">Duration in between samples when oversampling</param>
    public IObservableAnalogInputPort CreateAnalogInputPort(
        IPin pin,
        int sampleCount,
        TimeSpan sampleInterval)
    {
        return CreateAnalogInputPort(
            pin,
            sampleCount,
            sampleInterval,
            AnalogInputPort.DefaultReferenceVoltage);
    }

    /// <inheritdoc/>
    public IAnalogInputArray CreateAnalogInputArray(params IPin[] pins)
    {
        return new F7AnalogInputArray(this.IoController, pins);
    }

}
