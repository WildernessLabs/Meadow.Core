using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Threading.Tasks;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of reading analog input.
    /// </summary>
    // Common USE CASES:
    //
    //  * User needs to take an occasional voltage reading. Most likely it should 
    //    be oversampled to get an accurate reading.
    //
    //    ```
    //    ReadVoltage();
    //    ```
    //
    //  * User needs to take contuinous voltage readings. Most likely getting
    //    oversampled readings each time for accuracy.
    //
    //    ```
    //    StartSampling();
    //    Task(() => while (_running) { 
    //      var voltage = analogPort.CalculateAverageBufferVoltageValue();
    //      Task.Delay(4000);
    //    }.Start();
    //    ```
    //
    //  * User wants to take continuous voltage readings and wants to be notified
    //    //TODO: think through some notifcation configuration
    //    ```
    //    analogPort.VoltageChanged += (float newVoltage){ /*do something*/ };

    //    StartSampling();
    //    ConfigureNotifications();
    //    ```
    public class AnalogInputPort : AnalogInputPortBase, IObservable<FloatChangeResult>
    {
        // only one ADC across the entire processor can be read at one time.  This is the sync object for that.
        static readonly object _analogSyncRoot = new object();

        protected IIOController IOController { get; }

        // internal thread lock
        private object _lock = new object();
        private CancellationTokenSource SamplingTokenSource;

        /// <summary>
        /// Gets a value indicating whether the analog input port is currently
        /// sampling the ADC. Call StartSampling() to spin up the sampling process.
        /// </summary>
        /// <value><c>true</c> if sampling; otherwise, <c>false</c>.</value>
        public bool IsSampling { get; protected set; } = false;

        protected float _previousVoltageReading = 0;

        protected AnalogInputPort(
                    IPin pin,
                    IIOController ioController,
                    IAnalogChannelInfo channel,
                    float referenceVoltage)
            : base(pin, channel)
        {
            base.Pin = pin;
            this.IOController = ioController;
            base.Channel = channel;
            base.ReferenceVoltage = referenceVoltage;

            // attempt to reserve
            var success = DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.AnalogInput);
            if (success.Item1) {
                // make sure the pin is configured as an analog input
                ioController.ConfigureAnalogInput(pin);
            } else {
                throw new PortInUseException();
            }
        }

        internal static AnalogInputPort From(
            IPin pin,
            IIOController ioController,
            float referenceVoltage)
        {
            var channel = pin.SupportedChannels.OfType<IAnalogChannelInfo>().FirstOrDefault();
            if (channel != null) {
                //TODO: need other checks here.
                return new AnalogInputPort(pin, ioController, channel, referenceVoltage);
            } else {
                var supported = pin.SupportedChannels.Select(c => c.Name);
                var msg = $"Pin {pin.Name} does not support an analog input channel. It supports: {string.Join(",", supported)}";
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// Starts sampling the ADC and populating the sample buffer with values.
        ///
        /// When sampling, the AnalogInputPort will take multiple readings
        /// (samples); waiting for the `sampleIntervalDuration` in between them,
        /// and fill the sample buffer with those values, then sleep for the
        /// duration specified in `readIntervalDuration`.
        ///
        /// This method also starts the raising of events and IObservable
        /// subscribers to get notified. Use the `readIntervalDuration` parameter
        /// to specify how often events and notifications are raised/sent.
        /// </summary>
        /// <param name="sampleCount">The number of samples to take within any
        /// given reading. If 0, it will sample forever.</param>
        /// <param name="sampleIntervalDuration">The interval, in milliseconds, between
        /// sample readings.</param>
        /// <param name="standbyDuration">The time, in milliseconds, to wait
        /// between sets of sample readings. This value determines how often
        /// `Changed` events are raised and `IObservable` consumers are notified.</param>
        public override void StartSampling(
            int sampleCount = 10,
            int sampleIntervalDuration = 40,
            int standbyDuration = 100)
        {
            // thread safety
            lock (_lock) {
                if (IsSampling) return;

                // state muh-cheen
                IsSampling = true;

                SamplingTokenSource = new CancellationTokenSource();
                CancellationToken ct = SamplingTokenSource.Token;

                Task.Factory.StartNew(async () => {
                    int currentSampleCount = 0;
                    float[] sampleBuffer = new float[sampleCount];
                    // loop until we're supposed to stop
                    while (true) {
                        // TODO: someone please review; is this the correct
                        // place to do this?
                        // check for cancel (doing this here instead of 
                        // while(!ct.IsCancellationRequested), so we can perform 
                        // cleanup
                        if (ct.IsCancellationRequested) {
                            // do task clean up here
                            _observers.ForEach(x => x.OnCompleted());
                            break;
                        }

                        // read into the buffer
                        lock (_analogSyncRoot) {
                            var rawValue = this.IOController.GetAnalogValue(this.Pin);
                            // convert the raw valute into an actual voltage.
                            sampleBuffer[currentSampleCount] = ((float)rawValue / (float)MeadowOS.CurrentDevice.Capabilities.Analog.MaxRawAdcVoltageValue) * ReferenceVoltage;
                        }

                        // increment our counter
                        currentSampleCount++;

                        // if we still have more samples to take
                        if (currentSampleCount < sampleCount) {
                            // go to sleep for a while
                            await Task.Delay(sampleIntervalDuration);
                        }
                        // if we've filled our temp sample buffer, dump it into
                        // the class one
                        else {
                            // dump our buffer into the base buffer
                            // TODO: this probably isn't thread safe, might need
                            // to create a buffer access lock.
                            // ZOMG functional folks would laugh at us.
                            base.VoltageSampleBuffer.Clear();
                            for(int i = 0; i < sampleBuffer.Length; i++) {
                                base.VoltageSampleBuffer.Add(sampleBuffer[i]);
                            }

                            var newVoltage = Voltage;

                            // create a result set
                            FloatChangeResult result = new FloatChangeResult(newVoltage, _previousVoltageReading);

                            // raise our events and notify our subs
                            base.RaiseChangedAndNotify(result);

                            // save the previous voltage
                            _previousVoltageReading = newVoltage;

                            // reset our counter
                            currentSampleCount = 0;

                            // sleep for the appropriate interval
                            await Task.Delay(standbyDuration);
                        }
                    }
                }, SamplingTokenSource.Token);
            }
        }

        /// <summary>
        /// Spins down the process sampling the ADC. Any values in the 
        /// SampleBuffer will become stale after calling this method.
        /// </summary>
        public override void StopSampling()
        {
            lock (_lock)
            {
                if (!IsSampling) return;

                if (SamplingTokenSource != null) {
                    SamplingTokenSource.Cancel();
                }

                // state muh-cheen
                IsSampling = false;
            }
        }

        /// <summary>
        /// Convenience method to get the voltage value. For frequent reads, use
        /// StartSampling() and StopSampling() in conjunction with the SampleBuffer.
        /// </summary>
        /// <param name="sampleCount">The number of sample readings to take. 
        /// must be greater than 0.</param>
        /// <param name="sampleInterval">The interval, in milliseconds, between
        /// sample readings.</param>
        /// <returns>The raw value between 0 and x. TODO: @Ctacke 0 and what? Int.Max?</returns>
        public async override Task<float> Read(int sampleCount = 10, int sampleInterval = 40)
        {
            float[] sampleBuffer = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++) {
                // read into the buffer
                lock (_analogSyncRoot) {
                    var rawValue = this.IOController.GetAnalogValue(this.Pin);
                    // convert the raw valute into an actual voltage.
                    sampleBuffer[i] = ((float)rawValue / (float)MeadowOS.CurrentDevice.Capabilities.Analog.MaxRawAdcVoltageValue) * ReferenceVoltage;
                }
                await Task.Delay(sampleInterval);
            }

            // return the average of the samples
            return (float)(sampleBuffer.Select(x => (float)x).Sum() / sampleCount);
        }

        public override void Dispose()
        {
            
        }
    }
}