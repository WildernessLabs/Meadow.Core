using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        protected object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Meadow.Hardware.AnalogInputPort"/> class.
        /// </summary>
        /// <param name="pin">Pin.</param>
        protected AnalogInputPort(
            IPin pin,
            IIOController ioController,
            IAnalogChannelInfo channel,
            float voltageReference = 3.3f
            )
            : base (pin, channel)
        {
        }

        public static AnalogInputPort From(
            IPin pin,
            IIOController ioController,
            float voltageReference = 3.3f)
        {
            var chan = pin.SupportedChannels.OfType<IAnalogChannelInfo>().First();
            if (chan != null) {
                //TODO: need other checks here.
                return new AnalogInputPort(pin, ioController, chan);
            } else {
                throw new Exception("Unable to create an analog input port on the pin, because it doesn't have an analog channel");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the analog input port is currently
        /// sampling the ADC. Call StartSampling() to spin up the sampling process.
        /// </summary>
        /// <value><c>true</c> if sampling; otherwise, <c>false</c>.</value>
        public bool Sampling
        {
            get => _sampling;
        } protected bool _sampling = false;



        public override void StartSampling(int sampleSize = 10, int sampleIntervalDuration = 40, int sampleSleepDuration = 0) { 
            //lock (_lock) {
            //    if (_sampling) return;
            //    // start (TODO:@CTACKE)
            //    // state muh-cheen
            //    _sampling = true;
            //}
        }

        /// <summary>
        /// Spins down the process sampling the ADC. Any values in the 
        /// SampleBuffer will become stale after calling this method.
        /// </summary>
        public override void StopSampling()
        {
            //lock (_lock)
            //{
            //    if (!_sampling) return;
            //    // stop (TODO:@CTACKE)

            //    // state muh-cheen
            //    _sampling = false;
            //}
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
        public override async Task<float> Read(int sampleCount = 10, int sampleInterval = 40)
        {
            await Task.Delay(001); // so compiler will shut its mouth.
            return 0;
        }

        public override void Dispose()
        {
            //TODO: this.
        }
    }
}