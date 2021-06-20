using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meadow.Units;

namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for much of the common tasks of 
    /// implementing IAnalogInputPort
    /// </summary>
    public abstract class AnalogInputPortBase : AnalogPortBase, IAnalogInputPort
    {
        //public bool IsSampling { get; protected set; } = false;

        /// <summary>
        /// Raised when the value of the reading changes.
        /// </summary>
        public event EventHandler<IChangeResult<Voltage>> Updated = delegate { };

        /// <summary>
        /// Gets the sample buffer. Make sure to call StartSampling() before 
        /// use.
        /// </summary>
        /// <value>The sample buffer.</value>
        // TODO: make this a Memory<Voltage> if possible.
        public IList<Voltage> VoltageSampleBuffer { get; } = new List<Voltage>();

        /// <summary>
        /// A `TimeSpan` that specifies how long to
        /// wait between readings. This value influences how often `*Updated`
        /// events are raised and `IObservable` consumers are notified.
        /// </summary>
        public TimeSpan UpdateInterval { get; protected set; }

        /// <summary>
        /// Number of samples to take per reading. If > `1` then the port will
        /// take multiple readings and These are automatically averaged to
        /// reduce noise, a process known as _oversampling_.
        /// </summary>
        public int SampleCount { get; protected set; }

        /// <summary>
        /// Duration in between samples when oversampling.
        /// </summary>
        public TimeSpan SampleInterval { get; protected set; }


        public Voltage ReferenceVoltage { get; protected set; }

        /// <summary>
        /// Gets the average value of the values in the buffer. Use in conjunction
        /// with StartSampling() for long-running analog sampling. For occasional
        /// sampling, use Read().
        /// </summary>
        /// <value>The average buffer value.</value>
        public Voltage Voltage {
            get { //heh. may be a faster way to do this. 
                return new Voltage((VoltageSampleBuffer.Select(x => x.Volts).Sum() / VoltageSampleBuffer.Count()), Voltage.UnitType.Volts);
            }
        }

        // collection of observers
        protected List<IObserver<IChangeResult<Voltage>>> observers { get; set; } = new List<IObserver<IChangeResult<Voltage>>>();

        protected AnalogInputPortBase(
            IPin pin, IAnalogChannelInfo channel,
            int updateIntervalMs,
            int sampleCount, int sampleIntervalMs,
            float referenceVoltage)
            : base (pin, channel)
        {
            Pin = pin;
            UpdateInterval = TimeSpan.FromMilliseconds(updateIntervalMs);
            SampleCount = sampleCount;
            SampleInterval = TimeSpan.FromMilliseconds(sampleIntervalMs);
            ReferenceVoltage = new Voltage(referenceVoltage, Voltage.UnitType.Volts);
        }

        public abstract Task<Voltage> Read();

        public abstract void StartUpdating();

        public abstract void StopUpdating();


        protected void RaiseChangedAndNotify(IChangeResult<Voltage> changeResult)
        {
            Updated?.Invoke(this, changeResult);
            observers.ForEach(x => x.OnNext(changeResult));
        }

        public IDisposable Subscribe(IObserver<IChangeResult<Voltage>> observer)
        {
            if (!observers.Contains(observer)) observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<IChangeResult<Voltage>>> _observers;
            private IObserver<IChangeResult<Voltage>> _observer;

            public Unsubscriber(List<IObserver<IChangeResult<Voltage>>> observers, IObserver<IChangeResult<Voltage>> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }

    }
}
