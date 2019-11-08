using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public event EventHandler<FloatChangeResult> Changed = delegate { };

        /// <summary>
        /// Gets the sample buffer. Make sure to call StartSampling() before 
        /// use.
        /// </summary>
        /// <value>The sample buffer.</value>
        public IList<float> VoltageSampleBuffer { get; } = new List<float>();

        public float ReferenceVoltage { get; protected set; }

        /// <summary>
        /// Gets the average value of the values in the buffer. Use in conjunction
        /// with StartSampling() for long-running analog sampling. For occasional
        /// sampling, use Read().
        /// </summary>
        /// <value>The average buffer value.</value>
        public float AverageVoltageBufferValue {
            get { //heh. may be a faster way to do this. 
                return ((float)(VoltageSampleBuffer.Select(x => (float)x).Sum() / VoltageSampleBuffer.Count()));
            }
        }


        // collection of observers
        protected List<IObserver<FloatChangeResult>> _observers { get; set; } = new List<IObserver<FloatChangeResult>>();


        protected AnalogInputPortBase(IPin pin, IAnalogChannelInfo channel)
            : base (pin, channel)
        {
        }

        public abstract Task<float> Read(int sampleCount = 10, int sampleInterval = 40);
        public abstract void StartSampling(
            int sampleCount = 10,
            int sampleIntervalDuration = 40,
            int standbyDuration = 100);
        public abstract void StopSampling();


        protected void RaiseChangedAndNotify(FloatChangeResult changeResult)
        {
            Changed?.Invoke(this, changeResult);
            _observers.ForEach(x => x.OnNext(changeResult));
        }

        public IDisposable Subscribe(IObserver<FloatChangeResult> observer)
        {
            if (!_observers.Contains(observer)) _observers.Add(observer);
            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<FloatChangeResult>> _observers;
            private IObserver<FloatChangeResult> _observer;

            public Unsubscriber(List<IObserver<FloatChangeResult>> observers, IObserver<FloatChangeResult> observer)
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
