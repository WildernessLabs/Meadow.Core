using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meadow.Units;

namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for ports that implement an analog input channel.
    /// </summary>
    public interface IAnalogInputPort : IAnalogPort, IObservable<IChangeResult<Voltage>>
    {
        /// <summary>
        /// Raised when the value of the reading changes.
        /// </summary>
        event EventHandler<IChangeResult<Voltage>> Updated;

        // TODO should this be a Span<Voltage> or something? can Span<x> be
        // implicitly cast to IList? maybe it should be IEnumerable?
        /// <summary>
        /// Gets the sample buffer.
        /// </summary>
        /// <value>The sample buffer.</value>
        IList<Voltage> VoltageSampleBuffer { get; }

        /// <summary>
        /// The maximum voltage that the Analog Port can read. Typically 3.3V.
        /// This value is used to convert raw ADC values into voltage.
        /// </summary>
        Voltage ReferenceVoltage { get; }

        /// <summary>
        /// Gets the average value of the values in the buffer. Use in conjunction
        /// with StartSampling() for long-running analog sampling. For occasional
        /// sampling, use Read().
        /// </summary>
        /// <value>The average buffer value.</value>
        Voltage Voltage { get; }

        /// <summary>
        /// A `TimeSpan` that specifies how long to
        /// wait between readings. This value influences how often `*Updated`
        /// events are raised and `IObservable` consumers are notified.
        /// </summary>
        public TimeSpan UpdateInterval { get; }

        /// <summary>
        /// Number of samples to take per reading. If > `0` then the port will
        /// take multiple readings and These are automatically averaged to
        /// reduce noise, a process known as _oversampling_.
        /// </summary>
        public int SampleCount { get; }

        /// <summary>
        /// Duration in between samples when oversampling.
        /// </summary>
        public TimeSpan SampleInterval { get; }

        /// <summary>
        /// Convenience method to get the current voltage. For frequent reads, use
        /// StartSampling() and StopSampling() in conjunction with the SampleBuffer.
        /// </summary>
        /// <param name="sampleCount">The number of sample readings to take. 
        /// Must be greater than 0. These samples are automatically averaged.</param>
        /// <param name="sampleIntervalDuration">The time, in milliseconds,
        /// to wait in between samples during a reading.</param>
        /// <returns>A float value that's ann average value of all the samples taken.</returns>

        // TODO: get rid of these from here, and add properties:
        // public int SampleCount { get; set; } = 5;
        // public TimeSpan SampleInterval { get; set; } = TimeSpan.FromMilliseconds(40);
        Task<Voltage> Read(int sampleCount = 10, int sampleInterval = 40);

        /// <summary>
        /// Starts continuously sampling the analog port.
        ///
        /// This method also starts raising `Changed` events and IObservable
        /// subscribers getting notified. Use the `readIntervalDuration` parameter
        /// to specify how often events and notifications are raised/sent.
        /// </summary>
        void StartUpdating();

        /// <summary>
        /// Stops sampling the analog port.
        /// </summary>
        void StopUpdating();

        public static FilterableChangeObserver<Voltage>
            CreateObserver(
                Action<IChangeResult<Voltage>> handler,
                Predicate<IChangeResult<Voltage>>? filter = null)
        {
            return new FilterableChangeObserver<Voltage>(
                handler, filter);
        }

    }
}
