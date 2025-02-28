using Meadow.Units;
using System.Threading;

namespace Meadow.Hardware;

/// <summary>
/// A Pulse Width Modulation Generator that can
/// generates waveforms in software. The maximum
/// Frequency is about 100 Hz.
/// </summary>
public class SoftPwmPort : IPwmPort
{
    /// <summary>
    /// Digital output port used for PWM
    /// </summary>
    protected IDigitalOutputPort Port { get; set; }

    /// <summary>
    /// PWM duration in ms
    /// </summary>
    public TimePeriod Duration
    {
        get => TimePeriod.FromSeconds(Period.Seconds * DutyCycle);
        set { }
    }

    /// <summary>
    /// Period of PWM 
    /// </summary>
    public TimePeriod Period
    {
        get => TimePeriod.FromSeconds(1 / frequency.Hertz);
        set => frequency = new Frequency(1 / value.Seconds, Frequency.UnitType.Hertz);
    }

    /// <summary>
    /// Is the PWM signal inverted
    /// </summary>
    public bool Inverted { get; set; }


    /// <summary>
    /// Duty cycle of PWM
    /// </summary>
    public double DutyCycle
    {
        get => dutyCycle;
        set
        {
            dutyCycle = value;
            onTimeMilliseconds = CalculateOnTimeMillis();
            offTimeMilliseconds = CalculateOffTimeMillis();
        }
    }

    private double dutyCycle;

    /// <summary>
    /// Frequency of soft PWM
    /// </summary>
    public Frequency Frequency
    {
        get => frequency;
        set
        {
            frequency = value;
            onTimeMilliseconds = CalculateOnTimeMillis();
            offTimeMilliseconds = CalculateOffTimeMillis();
        }
    }

    private Frequency frequency = new Frequency(1.0, Units.Frequency.UnitType.Hertz); // in the case it doesn't get set before duty cycle, initialize to 1

    /// <summary>
    /// Channel info for PWM port
    /// </summary>
    public IPwmChannelInfo Channel { get; protected set; }

    /// <summary>
    /// State of PWM port (running / not running)
    /// </summary>
    public bool State => running;

    /// <summary>
    /// Pin used for soft PWM
    /// </summary>
    public IPin Pin => Port.Pin;

    IDigitalChannelInfo IPort<IDigitalChannelInfo>.Channel => new DigitalChannelInfo("soft-pwm", false, false, false, false, false, false);

    private Thread? thread = null;
    private int onTimeMilliseconds = 0;
    private int offTimeMilliseconds = 0;
    private bool running = false;

    /// <summary>
    /// Instantiate a SoftPwm object that can perform PWM using digital pins
    /// </summary>
    /// <param name="outputPin"></param>
    /// <param name="dutyCycle"></param>
    /// <param name="frequency"></param>
    public SoftPwmPort(IPin outputPin, float dutyCycle = 0.5f, float frequency = 1.0f) :
        this(outputPin.CreateDigitalOutputPort(false), dutyCycle, frequency)
    {
    }

    /// <summary>
    /// Instantiate a SoftPwm object that can perform PWM using digital pins
    /// </summary>
    /// <param name="outputPort"></param>
    /// <param name="dutyCycle"></param>
    /// <param name="frequencyInHertz"></param>
    public SoftPwmPort(IDigitalOutputPort outputPort, float dutyCycle = 0.0f, float frequencyInHertz = 1000)
    {
        Port = outputPort;
        DutyCycle = dutyCycle;
        frequency = new Frequency(frequencyInHertz, Units.Frequency.UnitType.Hertz);

        Channel = new PwmChannelInfo("SoftPwmChannel", 0, 0, 1000, 1000, false, false);
    }

    /// <summary>
    /// Start the pulse width modulation
    /// </summary>
    public void Start()
    {
        running = true;

        // create a new thread that actually writes the pwm to the output port
        thread = new Thread(() =>
        {
            while (running)
            {
                Port.State = !Inverted;
                Thread.Sleep(onTimeMilliseconds);
                Port.State = Inverted;
                Thread.Sleep(offTimeMilliseconds);
            }
        });
        thread.Start();
    }

    /// <summary>
    /// Stop the pulse width modulation
    /// </summary>
    public void Stop()
    {
        // setting this will wrap up the thread
        running = false;

        // need to make sure the port is off, otherwise it can get
        // stuck in an ON state.
        Port.State = false;
    }

    /// <summary>
    /// Calculates the pulse on time in milliseconds
    /// </summary>
    protected int CalculateOnTimeMillis()
    {
        var dc = DutyCycle;
        // clamp
        if (dc < 0) dc = 0;
        if (dc > 1) dc = 1;
        // on time  = 
        return (int)(dc / frequency.Kilohertz);
    }

    /// <summary>
    /// Calculates the off time of pulse in milliseconds
    /// </summary>
    /// <returns></returns>
    protected int CalculateOffTimeMillis()
    {
        var dc = DutyCycle;
        // clamp
        if (dc < 0) dc = 0;
        if (dc > 1) dc = 1;
        // off time = 
        return (int)((1 - dc) / frequency.Kilohertz);
    }

    private bool disposedValue = false; // To detect redundant calls

    /// <summary>
    /// Dispose of the object
    /// </summary>
    /// <param name="disposing">Is disposing</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
            }

            disposedValue = true;
        }
    }

    ///<inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
    }
}