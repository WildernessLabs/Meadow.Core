using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using System.Collections.Generic;

namespace Meadow.Hardware
{
    // Common USE CASES: TODO: update
    //
    //  * User needs to take an occasional voltage reading. Most likely it should 
    //    be oversampled to get an accurate reading.
    //
    //    ```
    //    Read();
    //    ```
    //
    //  * User needs to take contuinous voltage readings. Most likely getting
    //    oversampled readings each time for accuracy.
    //
    //    ```
    //    ```
    //
    //  * User wants to take continuous voltage readings and wants to be notified
    //    ```
    //    analogPort.VoltageChanged += (float newVoltage){ /*do something*/ };
    //    StartSampling();
    //    
    //    ```


    /// <summary>
    /// Represents a port that is capable of reading analog input.
    /// 
    /// Note: this class is not yet implemented.
    /// </summary>
    public class AnalogInputPort : AnalogInputPortBase
    {
        protected float _voltageReference;
        protected float _previousVoltageReading = 0;
        protected int _adcMaxValue = 1023; //TODO: Get this from bit depth from Device.Capabilities.Analog or maybe the underlying channel info

        public event EventHandler<FloatChangeResult> Changed = delegate { };

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Meadow.Hardware.AnalogInputPort"/> class.
        /// </summary>
        /// <param name="pin">Pin.</param>
        public AnalogInputPort(IAnalogPin pin, float voltageReference = 3.3f) : base(pin)
        {
            _voltageReference = voltageReference;
        }

        protected object _lock = new object();
        private CancellationTokenSource SamplingTokenSource;

        /// <summary>
        /// Gets a value indicating whether the analog input port is currently
        /// sampling the ADC. Call StartSampling() to spin up the sampling process.
        /// </summary>
        /// <value><c>true</c> if sampling; otherwise, <c>false</c>.</value>
        public bool IsSampling { get; protected set; } = false;

        /// <summary>
        /// Starts sampling the ADC. To access the voltage readings, use 
        /// </summary>
        /// <param name="sampleIntervalDuration">The interval, in milliseconds, between
        /// sample readings.</param>
        /// <param name="sampleSleepDuration">The duration, in milliseconds, to sleep
        /// before taking another sample set.
        /// </param>
        public void StartSampling(int sampleSize = 10, int sampleIntervalDuration = 40, int sampleSleepDuration = 0)
        {
            // thread safety
            lock (_lock) {
                if (IsSampling) return;

                // state muh-cheen
                IsSampling = true;

                //
                // TODO: @CTACKE Turn on sampling under the hood
                //

                SamplingTokenSource = new CancellationTokenSource();
                CancellationToken ct = SamplingTokenSource.Token;

                // sampling happens on a background thread
                Task.Factory.StartNew(async () => {
                    // loop until we're supposed to stop
                    while (true) {
                        float voltage = await Read(sampleSize, sampleIntervalDuration);

                        // create a result set
                        FloatChangeResult result = new FloatChangeResult {
                            New = voltage,
                            Old = _previousVoltageReading,
                        };

                        // notify observers
                        NotifyObservers(result);

                        // raise the classic event (if there are any)
                        // TODO: Decide if this is a good idea.
                        Changed?.Invoke(this, result);

                        // go to sleep for a while
                        await Task.Delay(sampleSleepDuration);

                        // check for cancel (doing this here instead of 
                        // while(!ct.IsCancellationRequested), so we can perform 
                        // cleanup
                        if (ct.IsCancellationRequested) {
                            // do task clean up here
                            break;
                        }
                    }

                }, SamplingTokenSource.Token).Wait();
            }
        }

        /// <summary>
        /// Spins down the process sampling the ADC. Any values in the 
        /// SampleBuffer will become stale after calling this method.
        /// </summary>
        public void StopSampling()
        {
            lock (_lock) {
                if (!IsSampling) return;
                // stop (TODO:@CTACKE)

                if (SamplingTokenSource != null) {
                    SamplingTokenSource.Cancel();
                }

                // state muh-cheen
                IsSampling = false;
            }
        }

        /// <summary>
        /// Convenience method to get the voltage value. Starts sampling 
        /// if not already doing so, and will stop sampling after the read. For
        /// frequent reads, use StartSampling() and StopSampling() in conjunction
        /// with the SampleBuffer. Automatically oversamples (takes multiple readings 
        /// and averages them).  
        /// </summary>
        /// <param name="sampleCount">The number of sample readings to take and 
        /// average (oversample). Must be greater than 0. Pass 1 for no oversampling.</param>
        /// <param name="sampleInterval">The interval, in milliseconds, between
        /// sample readings. Recommended > 20.</param>
        /// <returns></returns>
        public override async Task<float> Read(
            int sampleCount = 10,
            int sampleInterval = 40)
        {
            // buffer of x samples
            int[] sampleBuffer = new int[sampleCount];

            // if we're not sampling already, we need to spin up
            if (!IsSampling) { StartSampling(); }

            // take samples
            // value mockup
            // TODO: get ADC bit depth from Device.Capabilities.Analog or maybe the underlying channel info
            float voltage = 0f;
            for (int i = 0; i < sampleCount; i++) {
                sampleBuffer[i] = GenerateRawVoltageSample(512, 10, _adcMaxValue); // 1.65V +/- 10%;
                await Task.Delay(sampleInterval);
            }

            //TODO: decide how we want to handle STOP()
            // will kill subscribers. maybe only stop if no subs
            if (_observers.Count == 0) { StopSampling(); }

            // convert samples to voltage reading:
            //  a. get the average (casting int to byte), then 
            //  b. % of read * voltageMax = V
            voltage = (float)(sampleBuffer.Average(x => x) / _adcMaxValue) * _voltageReference;

            // voltage out!
            return voltage;
        }

        private Random _rnd = new Random();
        /// <summary>
        /// Generates a psuedo random raw voltage reading based on targe and deviation
        /// </summary>
        /// <returns>The raw voltage sample.</returns>
        private int GenerateRawVoltageSample(int targetValue, byte percentDeviation, int maxValue)
        {
            int value = (int)_rnd.Next(
                targetValue - (targetValue / percentDeviation), // min
                targetValue + (targetValue / percentDeviation)  // max
                ); // + or - x% deviation
            if (value > maxValue) { return maxValue; }
            if (value < 0) { return 0; }
            return value;
        }
    }
}