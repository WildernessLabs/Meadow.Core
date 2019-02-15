using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using System.Collections.Generic;

namespace Meadow.Hardware
{
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


    /// <summary>
    /// Represents a port that is capable of reading analog input.
    /// 
    /// Note: this class is not yet implemented.
    /// </summary>
    public class AnalogInputPort : AnalogInputPortBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Meadow.Hardware.AnalogInputPort"/> class.
        /// </summary>
        /// <param name="pin">Pin.</param>
        public AnalogInputPort(IAnalogPin pin) : base (pin)
        {
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
        public override void StartSampling(int sampleSize = 10, int sampleIntervalDuration = 40, int sampleSleepDuration = 0)
        {
            lock (_lock)
            {
                if (IsSampling) return;

                // state muh-cheen
                IsSampling = true;

                List<float> sampleBuffer = new List<float>();

                SamplingTokenSource = new CancellationTokenSource();
                CancellationToken ct = SamplingTokenSource.Token;

                Task.Factory.StartNew(async () =>
                {
                    float lastSampleValue = 0;

                    while (true)
                    {
                        var val = await ReadVoltage();

                        sampleBuffer.Add(val);

                        if (sampleBuffer.Count == sampleSize)
                        {
                            var avg = sampleBuffer.Average();

                            foreach (var observer in observers)
                            {
                                switch (observer.Value.SubscriptionMode)
                                {
                                    case Bases.SubscriptionMode.Absolute:
                                        if (observer.Value.Filter(avg))
                                        {
                                            observer.Key.OnNext(avg);
                                        }
                                        break;
                                    case Bases.SubscriptionMode.Relative:
                                        if (lastSampleValue == 0)
                                        {
                                            lastSampleValue = avg;
                                        }
                                        else
                                        {
                                            var delta = sampleBuffer.Average() - lastSampleValue;
                                            if (observer.Value.Filter(avg))
                                            {
                                                observer.Key.OnNext(avg);
                                            }
                                        }
                                        break;
                                    case Bases.SubscriptionMode.Percentage:
                                        if (lastSampleValue == 0)
                                        {
                                            lastSampleValue = avg;
                                        }
                                        else
                                        {
                                            var delta = sampleBuffer.Average() - lastSampleValue;
                                            var percentChange = (delta / lastSampleValue) * 100;
                                            if (observer.Value.Filter(avg))
                                            {
                                                observer.Key.OnNext(avg);
                                            }
                                        }
                                        break;
                                }
                            }

                            sampleBuffer.Clear();
                            Thread.Sleep(sampleSleepDuration);
                        }
                        else
                        {
                            Thread.Sleep(sampleIntervalDuration);
                        }

                        if (ct.IsCancellationRequested)
                        {
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
            lock (_lock)
            {
                if (!IsSampling) return;
                // stop (TODO:@CTACKE)

                if(SamplingTokenSource != null)
                {
                    SamplingTokenSource.Cancel();
                }

                // state muh-cheen
                IsSampling = false;
            }
        }

        /// <summary>
        /// Convenience method to get the raw voltage value. Starts sampling 
        /// if not already doing so, and will stop sampling after the read. For
        /// frequent reads, use StartSampling() and StopSampling() in conjunction
        /// with the SampleBuffer.
        /// </summary>
        /// <param name="sampleCount">The number of sample readings to take. 
        /// must be greater than 0.</param>
        /// <param name="sampleInterval">The interval, in milliseconds, between
        /// sample readings.</param>
        /// <returns>The raw value between 0 and x. TODO: @Ctacke 0 and what? Int.Max?</returns>
        public override async Task<byte> Read(int sampleCount = 10, int sampleInterval = 40)
        {
            throw new NotImplementedException();
            ////TODO: @CTACKE spin up a task to do:
            //if (!IsSampling) { StartSampling(); }

            //// 

            //StopSampling();
            //return Samples.FirstOrDefault<byte>();
        }

        /// <summary>
        /// Convenience method to get the voltage reading. Starts sampling 
        /// if not already doing so, and will stop sampling after the read. For
        /// frequent reads, use StartSampling() and StopSampling() in conjunction
        /// with the SampleBuffer.
        /// </summary>
        /// <returns>The voltage.</returns>
        /// <param name="sampleCount">The number of sample readings to take. 
        /// must be greater than 0.</param>
        /// <param name="sampleInterval">The interval, in milliseconds, between
        /// sample readings.</param>
        /// <param name="referenceVoltage">The maximum voltage value to compare 
        /// against.</param>
        public override async Task<float> ReadVoltage(
            int sampleCount = 10, 
            int sampleInterval = 40,
            float referenceVoltage = 3.3f)
        {
            throw new NotImplementedException();

            //TODO: Call Read() and automatically convert to voltage
        }
    }
}