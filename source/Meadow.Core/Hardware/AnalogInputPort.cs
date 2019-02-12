using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;

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

        

        /// <summary>
        /// Gets a value indicating whether the analog input port is currently
        /// sampling the ADC. Call StartSampling() to spin up the sampling process.
        /// </summary>
        /// <value><c>true</c> if sampling; otherwise, <c>false</c>.</value>
        public bool IsSampling { get; protected set; } = false;
        
        /// <summary>
        /// Gets the average value of the values in the buffer. Use in conjunction
        /// with StartSampling() for long-running analog sampling. For occasional
        /// sampling, use Read().
        /// </summary>
        /// <value>The average buffer value.</value>
        public byte AverageBufferValue
        { 
            get { //heh. may be a faster way to do this. 
                return ((byte)(Samples.Select(x => (decimal)x).Sum() / Samples.Count()));
            }
        }

        /// <summary>
        /// Gets the average voltage value of the values in the buffer. Use in conjunction
        /// with StartSampling() for long-running analog sampling. For occasional
        /// sampling, use Read(). Calculates against the reference voltageVoltage.
        /// </summary>
        /// <value>The average buffer value.</value>
        public byte CalculateAverageBufferVoltageValue(float referenceVoltage = 3.3f)
        {
            // TODO: @CTACKE: return (AverageBufferValue / int.Max) * referenceVoltage or whatever
            return 0;
        }

        /// <summary>
        /// Gets the sample buffer. Make sure to call StartSampling() before 
        /// use.
        /// </summary>
        /// <value>The sample buffer.</value>
        public ObservableRangeCollection<byte> Samples { get; protected set; } = new ObservableRangeCollection<byte>();

        private CancellationTokenSource SamplingTokenSource;

        /// <summary>
        /// Starts sampling the ADC. To access the voltage readings, use 
        /// </summary>
        /// <param name="sampleCount">The number of sample readings to take. If
        /// 0, will sample forever.</param>
        /// <param name="sampleInterval">The interval, in milliseconds, between
        /// sample readings.</param>
        public void StartSampling(int sampleCount = 0, int sampleInterval = 40, int minSamplesForNotifications = 1, Predicate<byte> filter = null)
        {
            lock (_lock)
            {
                if (IsSampling) return;

                byte[] buffer = new byte[minSamplesForNotifications];
                int bufferPos = 0;

                SamplingTokenSource = new CancellationTokenSource();
                CancellationToken ct = SamplingTokenSource.Token;

                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        // get sample value (TODO:@CTACKE)
                        byte val = default(byte);

                        if (filter(val))
                        {
                            buffer[bufferPos] = val;
                            bufferPos++;

                            if (bufferPos == minSamplesForNotifications)
                            {
                                Samples.AddRange(buffer);
                                bufferPos = 0;
                            }
                        }

                        if (ct.IsCancellationRequested)
                        {
                            // do task clean up here
                        }
                    }

                }, SamplingTokenSource.Token);

                // state muh-cheen
                IsSampling = true;
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
            //TODO: @CTACKE spin up a task to do:
            if (!IsSampling) { StartSampling(); }

            // 

            StopSampling();
            return Samples.FirstOrDefault<byte>();
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
        public override async Task<byte> ReadVoltage(
            int sampleCount = 10, 
            int sampleInterval = 40,
            float referenceVoltage = 3.3f)
        {
            return 0;

            //TODO: Call Read() and automatically convert to voltage
        }
    }
}