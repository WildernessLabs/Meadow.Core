using System;
using System.Linq;

namespace Meadow.Hardware;

/// <summary>
/// Represents a port that is capable of reading digital input.
/// </summary>
public class DigitalInterruptPort : DigitalInterruptPortBase
{
    private ResistorMode _resistorMode;
    private TimeSpan _debounceDuration;
    private TimeSpan _glitchDuration;
    private DigitalPortResult _interruptResult = new DigitalPortResult();
    private DigitalState _newState = new DigitalState(false, -1);
    private DigitalState _oldState = new DigitalState(false, -1);

    /// <inheritdoc/>
    protected IMeadowIOController IOController { get; set; }

    private int LastEventTime { get; set; } = -1;

    /// <summary>
    /// Protected constructor for creating a <see cref="DigitalInterruptPort"/>.
    /// </summary>
    /// <param name="pin">The pin associated with the digital interrupt port.</param>
    /// <param name="ioController">The Meadow I/O controller.</param>
    /// <param name="channel">The digital channel information.</param>
    /// <param name="interruptMode">The interrupt mode for the port.</param>
    /// <param name="resistorMode">The resistor mode for the port.</param>
    /// <param name="debounceDuration">The debounce duration for the port.</param>
    /// <param name="glitchDuration">The glitch duration for the port.</param>
    protected DigitalInterruptPort(
        IPin pin,
        IMeadowIOController ioController,
        IDigitalChannelInfo channel,
        InterruptMode interruptMode,
        ResistorMode resistorMode,
        TimeSpan debounceDuration,
        TimeSpan glitchDuration
        ) : base(pin, channel, interruptMode)
    {
        // DEVELOPER NOTE:
        // Debounce recognizes the first state transition and then ignores anything after that for a period of time.
        // Glitch filtering ignores the first state transition and waits a period of time and then looks at state to make sure the result is stable

        if (interruptMode != InterruptMode.None && (!channel.InterruptCapable))
        {
            throw new Exception("Unable to create port; channel is not capable of interrupts");
        }

        this.IOController = ioController;
        this.IOController.Interrupt += OnInterrupt;
        this._resistorMode = resistorMode;
        _debounceDuration = debounceDuration;
        _glitchDuration = glitchDuration;

        // attempt to reserve
        var success = this.IOController.DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.DigitalInput);
        if (success.Item1)
        {
            // make sure the pin is configured as a digital input with the proper state
            ioController.ConfigureInput(pin, resistorMode, interruptMode, debounceDuration, glitchDuration);
            if (interruptMode != InterruptMode.None)
            {
                IOController.WireInterrupt(pin, interruptMode, resistorMode, debounceDuration, glitchDuration);
            }
        }
        else
        {
            throw new PortInUseException($"{this.GetType().Name}: Pin {pin.Name} is already in use");
        }
    }

    /// <summary>
    /// Creates a DigitalInterruptPort with the specified parameters
    /// </summary>
    /// <param name="pin">The IPin the port is on</param>
    /// <param name="ioController">The IIoController used to control the IPin</param>
    /// <param name="interruptMode">The interrupt mode for the port</param>
    /// <param name="resistorMode">The resistor mode for the port</param>
    /// <param name="debounceDuration">The debounce duration used for interrupts</param>
    /// <param name="glitchDuration">The glitch filter duration used for interrupts</param>
    public static DigitalInterruptPort From(
        IPin pin,
        IMeadowIOController ioController,
        InterruptMode interruptMode,
        ResistorMode resistorMode,
        TimeSpan debounceDuration,
        TimeSpan glitchDuration
        )
    {
        var chan = pin.SupportedChannels.OfType<IDigitalChannelInfo>().FirstOrDefault();
        //TODO: may need other checks here.
        if (chan == null)
        {
            throw new Exception($"Unable to create an input port on pin '{pin.Name}', because it doesn't have a digital channel");
        }
        if (interruptMode != InterruptMode.None && (!chan.InterruptCapable))
        {
            throw new Exception($"Unable to create interrupt port on pin '{pin.Name}': the channel is not capable of interrupts");
        }
        if (debounceDuration.TotalMilliseconds > 1000.0)
        {
            throw new ArgumentOutOfRangeException(nameof(debounceDuration), "Unable to create an input port, because debounceDuration is out of range (0.1-1000.0)");
        }
        if (glitchDuration.TotalMilliseconds > 1000.0)
        {
            throw new ArgumentOutOfRangeException(nameof(glitchDuration), "Unable to create an input port, because glitchDuration is out of range (0.1-1000.0)");
        }

        var port = new DigitalInterruptPort(pin, ioController, chan, interruptMode, resistorMode,
                        debounceDuration, glitchDuration);
        return port;
    }

    /// <summary>
    /// Gets or Sets the internal resistor mode for the input
    /// </summary>
    public override ResistorMode Resistor
    {
        get => _resistorMode;
        set
        {
            IOController.SetResistorMode(this.Pin, value);
            _resistorMode = value;
        }
    }

    private void OnInterrupt(IPin pin, bool state)
    {
        if (pin == this.Pin)
        {
            lock (this)
            {
                // this is all to prevent new-ing up (and thereby preventing GC stuff)
                _oldState.Time = LastEventTime; // note: doing this for latency reasons. kind of. sort of. bad time good time. all time.
                _newState.Time = this.LastEventTime = Environment.TickCount;
                _oldState.State = !state;
                _newState.State = state;
                _interruptResult.Old = (LastEventTime == -1) ? null : _oldState;
                _interruptResult.New = _newState;
            }
            RaiseChangedAndNotify(_interruptResult);
        }
    }

    /// <summary>
    /// Releases Port resources
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        // TODO: we should consider moving this logic to the finalizer
        // but the problem with that is that we don't know when it'll be called
        // but if we do it in here, we may need to check the _disposed field
        // elsewhere
        if (!disposed)
        {
            if (disposing)
            {
                this.IOController.Interrupt -= OnInterrupt;
                this.IOController.DeviceChannelManager.ReleasePin(Pin);
                IOController.UnconfigureGpio(Pin);
            }
            disposed = true;
        }
    }

    /// <summary>
    /// Finalizes the Port instance
    /// </summary>
    ~DigitalInterruptPort()
    {
        Dispose(false);
    }

    /// <summary>
    /// Gets the current State of the input (True == high, False == low)
    /// </summary>
    public override bool State
    {
        get
        {
            var state = this.IOController.GetDiscrete(this.Pin);
            return InverseLogic ? !state : state;
        }
    }

    /// <summary>
    /// Gets or Sets the interrupt debounce duration
    /// </summary>
    public override TimeSpan DebounceDuration
    {
        get => _debounceDuration;
        set
        {
            if (value.TotalMilliseconds < 0.0 || value.TotalMilliseconds > 1000.0) throw new ArgumentOutOfRangeException("DebounceDuration");
            if (value == _debounceDuration) return;

            _debounceDuration = value;

            // Update in F7
            // we have to disconnect the interrupt and reconnect, otherwise we'll get an error for an already-wired interrupt
            this.IOController.WireInterrupt(Pin, InterruptMode.None, _resistorMode, TimeSpan.Zero, TimeSpan.Zero);
            this.IOController.WireInterrupt(Pin, InterruptMode, _resistorMode, _debounceDuration, _glitchDuration);
        }
    }

    /// <summary>
    /// Gets or Sets the interrupt glitch filter duration
    /// </summary>
    public override TimeSpan GlitchDuration
    {
        get => _glitchDuration;
        set
        {
            if (value.TotalMilliseconds < 0.0 || value.TotalMilliseconds > 1000.0) throw new ArgumentOutOfRangeException("GlitchDuration");
            if (value == _glitchDuration) return;

            _glitchDuration = value;

            // Update in F7
            // we have to disconnect the interrupt and reconnect, otherwise we'll get an error for an already-wired interrupt
            this.IOController.WireInterrupt(Pin, InterruptMode.None, _resistorMode, TimeSpan.Zero, TimeSpan.Zero);
            this.IOController.WireInterrupt(Pin, InterruptMode, _resistorMode, _debounceDuration, _glitchDuration);
        }
    }
}