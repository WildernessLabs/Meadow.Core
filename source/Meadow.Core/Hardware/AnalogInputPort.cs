using Meadow.Units;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Hardware;

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
//  * User needs to take continuous voltage readings. Most likely getting
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
//    //TODO: think through some notification configuration
//    ```
//    analogPort.VoltageChanged += (float newVoltage){ /*do something*/ };

//    StartSampling();
//    ConfigureNotifications();
//    ```
public class AnalogInputPort : AnalogInputPortBase, IObservable<IChangeResult<Voltage>>
{
    // only one ADC across the entire processor can be read at one time.  This is the sync object for that.
    private static readonly object _analogSyncRoot = new();

    /// <summary>
    /// The default sampling interval for the Input (40ms)
    /// </summary>
    public static readonly TimeSpan DefaultSampleInterval = TimeSpan.FromMilliseconds(40);
    /// <summary>
    /// The default reference voltage for the Input (3.3V)
    /// </summary>
    public static readonly Voltage DefaultReferenceVoltage = new Voltage(3.3, Voltage.UnitType.Volts);

    /// <summary>
    /// Gets the IOController device
    /// </summary>
    protected IMeadowIOController IOController { get; }

    // internal thread lock
    private readonly object _lock = new();
    private CancellationTokenSource? SamplingTokenSource;

    /// <summary>
    /// Gets a value indicating whether the analog input port is currently
    /// sampling the ADC. Call StartUpdating() to spin up the sampling process.
    /// </summary>
    /// <value><c>true</c> if sampling; otherwise, <c>false</c>.</value>
    public bool IsSampling { get; protected set; } = false;

    /// <summary>
    /// Gets or sets the previous voltage reading
    /// </summary>
    protected Voltage? PreviousVoltageReading { get; set; }

    /// <summary>
    /// Creates an AnalogInputPort given the provided parameters
    /// </summary>
    /// <param name="pin">The IPin to use for the input port</param>
    /// <param name="ioController">The IMeadowIOController for the pin</param>
    /// <param name="channel">The pin's channel info</param>
    /// <param name="sampleCount">The number of ADC readings to average for a single sample</param>
    /// <param name="sampleInterval">The time between readings used for calculating the average for a sample</param>
    /// <param name="referenceVoltage">The ADCs reference voltage</param>
    /// <exception cref="PortInUseException"></exception>
    protected AnalogInputPort(
                IPin pin, IMeadowIOController ioController, IAnalogChannelInfo channel,
                int sampleCount, TimeSpan sampleInterval,
                Voltage referenceVoltage)
        : base(pin, channel, sampleCount, sampleInterval, referenceVoltage)
    {
        // save all the settings
        this.IOController = ioController;

        // attempt to reserve
        var success = IOController.DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.AnalogInput);
        if (success.Item1)
        {
            // make sure the pin is configured as an analog input
            ioController.ConfigureAnalogInput(pin);
        }
        else
        {
            throw new PortInUseException($"{this.GetType().Name}: Pin {pin.Name} is already in use");
        }
    }

    /// <summary>
    /// Creates an AnalogInputPort given the provided parameters
    /// </summary>
    /// <param name="pin"></param>
    /// <param name="ioController"></param>
    /// <param name="sampleCount"></param>
    /// <returns></returns>
    public static AnalogInputPort From(
        IPin pin,
        IMeadowIOController ioController,
        int sampleCount = 5)
    {
        return From(pin, ioController, sampleCount, DefaultSampleInterval, DefaultReferenceVoltage);
    }

    /// <summary>
    /// Creates an AnalogInputPort given the provided parameters
    /// </summary>
    /// <param name="pin"></param>
    /// <param name="ioController"></param>
    /// <param name="sampleCount"></param>
    /// <param name="sampleInterval"></param>
    /// <param name="referenceVoltage"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="Exception"></exception>
    public static AnalogInputPort From(
        IPin pin,
        IMeadowIOController ioController,
        int sampleCount,
        TimeSpan sampleInterval,
        Voltage referenceVoltage)
    {
        var channel = pin.SupportedChannels.OfType<IAnalogChannelInfo>().FirstOrDefault();
        if (channel != null)
        {
            //TODO: need other checks here.
            if (sampleCount < 1) { throw new ArgumentException("sampleCount must be greater than zero."); }

            return new AnalogInputPort(
                pin, ioController, channel,
                sampleCount, sampleInterval,
                referenceVoltage);
        }
        else
        {
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
    /// <param name="updateInterval">A `TimeSpan` to wait
    /// between sets of sample readings. This value determines how often
    /// `Changed` events are raised and `IObservable` consumers are notified.</param>
    public override void StartUpdating(TimeSpan? updateInterval = null)
    {
        // thread safety
        lock (_lock)
        {
            if (IsSampling) return;

            // if an update interval was passed in, override the default value
            if (updateInterval is { } ui) { base.UpdateInterval = ui; }

            SamplingTokenSource = new CancellationTokenSource();
            CancellationToken ct = SamplingTokenSource.Token;

            Task.Factory.StartNew(async () =>
            {
                // loop until we're supposed to stop
                while (true)
                {
                    // cleanup
                    if (ct.IsCancellationRequested)
                    {
                        // do task clean up here
                        Observers.ForEach(x => x.OnCompleted());
                        break;
                    }

                    var newVoltage = await Read();

                    // create a result set
                    ChangeResult<Voltage> result = new ChangeResult<Voltage>(newVoltage, PreviousVoltageReading);

                    // raise our events and notify our subs
                    base.RaiseChangedAndNotify(result);

                    // save the previous voltage
                    PreviousVoltageReading = newVoltage;


                    // sleep for the appropriate interval
                    await Task.Delay(UpdateInterval);
                }

            }, SamplingTokenSource.Token);
        }
    }

    /// <summary>
    /// Spins down the process sampling the ADC. Any values in the 
    /// SampleBuffer will become stale after calling this method.
    /// </summary>
    public override void StopUpdating()
    {
        lock (_lock)
        {
            if (!IsSampling) return;

            SamplingTokenSource?.Cancel();

            IsSampling = false;
        }
    }

    /// <summary>
    /// Convenience method to get the voltage value. For frequent reads, use
    /// StartUpdating() and StopUpdating() in conjunction with the SampleBuffer.
    /// </summary>
    /// <returns>Read Voltage</returns>
    public override async Task<Voltage> Read()
    {
        IsSampling = true;

        for (int i = 0; i < SampleCount; i++)
        {
            // read into the buffer
            lock (_analogSyncRoot)
            {
                var rawValue = this.IOController.GetAnalogValue(this.Pin);
                VoltageSampleBuffer[i] = new Voltage(base.ConvertReadingToVoltage(rawValue));
            }
            await Task.Delay(SampleInterval);
        }

        IsSampling = false;

        return Voltage;
    }
}