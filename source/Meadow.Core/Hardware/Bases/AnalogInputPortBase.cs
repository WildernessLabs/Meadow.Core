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
        public event EventHandler<CompositeChangeResult<Voltage>> Changed = delegate { };

        /// <summary>
        /// Gets the sample buffer. Make sure to call StartSampling() before 
        /// use.
        /// </summary>
        /// <value>The sample buffer.</value>
        public IList<Voltage> VoltageSampleBuffer { get; } = new List<Voltage>();

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
        protected List<IObserver<CompositeChangeResult<Voltage>>> observers { get; set; } = new List<IObserver<CompositeChangeResult<Voltage>>>();


        protected AnalogInputPortBase(IPin pin, IAnalogChannelInfo channel)
            : base (pin, channel)
        {
        }

        public abstract Task<Voltage> Read(int sampleCount = 10, int sampleInterval = 40);
        public abstract void StartSampling(
            int sampleCount = 10,
            int sampleIntervalDuration = 40,
            int standbyDuration = 100);
        public abstract void StopSampling();


        protected void RaiseChangedAndNotify(CompositeChangeResult<Voltage> changeResult)
        {
            Changed?.Invoke(this, changeResult);
            observers.ForEach(x => x.OnNext(changeResult));
        }

        public IDisposable Subscribe(IObserver<CompositeChangeResult<Voltage>> observer)
        {
            if (!observers.Contains(observer)) observers.Add(observer);
            return new Unsubscriber(observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<CompositeChangeResult<Voltage>>> _observers;
            private IObserver<CompositeChangeResult<Voltage>> _observer;

            public Unsubscriber(List<IObserver<CompositeChangeResult<Voltage>>> observers, IObserver<CompositeChangeResult<Voltage>> observer)
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
