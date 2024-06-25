using System;
using System.Linq;

namespace Meadow.Hardware;

/// <summary>
/// Represents a bi-directional port.
/// </summary>
public class BiDirectionalPort : BiDirectionalPortBase
{
    private PortDirectionType _currentDirection;

    /// <summary>
    /// Gets or sets the port's IOController
    /// </summary>
    protected IMeadowIOController IOController { get; }
    /// <summary>
    /// Gets or sets the last event time for the port.
    /// </summary>
    protected DateTime LastEventTime { get; set; } = DateTime.MinValue;

    /// <inheritdoc/>
    public override PortDirectionType Direction
    {
        get => _currentDirection;
        set
        {
            // since we're overriding a virtual, which actually gets called in the base ctor, we need to ignore that ctor call (the IO Controller will be null)
            if ((IOController == null) || (value == Direction)) return;

            // InterruptMode.None disables interrupts within Nuttx via WireInterrupt
            this.IOController.ConfigureInput(this.Pin, this.Resistor, InterruptMode.None, TimeSpan.Zero, TimeSpan.Zero);

            if (value == PortDirectionType.Output)
            {
                this.IOController.ConfigureOutput(this.Pin, this.InitialState, InitialOutputType);
            }
            _currentDirection = value;
        }
    }

    /// <summary>
    /// Protected constructor for creating a <see cref="BiDirectionalPort"/>.
    /// </summary>
    /// <param name="pin">The pin associated with the bi-directional port.</param>
    /// <param name="gpioController">The Meadow I/O controller.</param>
    /// <param name="channel">The digital channel information.</param>
    /// <param name="initialState">The initial state of the port.</param>
    /// <param name="resistorMode">The resistor mode for the port (default is <see cref="ResistorMode.Disabled"/>).</param>
    /// <param name="initialDirection">The initial direction of the port (default is <see cref="PortDirectionType.Input"/>).</param>
    /// <param name="outputType">The output type for the port (default is <see cref="OutputType.PushPull"/>).</param>
    protected BiDirectionalPort(
        IPin pin,
        IMeadowIOController gpioController,
        IDigitalChannelInfo channel,
        bool initialState,
        ResistorMode resistorMode = ResistorMode.Disabled,
        PortDirectionType initialDirection = PortDirectionType.Input,
        OutputType outputType = OutputType.PushPull
        )
        : base(pin, channel, initialState, resistorMode, initialDirection, outputType)
    {
        this.IOController = gpioController ?? throw new ArgumentNullException(nameof(gpioController));

        // attempt to reserve the pin - we'll reserve it as an input even though we use it for bi-directional
        var result = this.IOController.DeviceChannelManager.ReservePin(
            this.Pin,
            ChannelConfigurationType.DigitalInput);

        if (result.Item1)
        {
            Direction = initialDirection;
        }
        else
        {
            throw new PortInUseException($"{this.GetType().Name}: Pin {pin.Name} is already in use");
        }
    }

    /// <summary>
    /// Creates a new instance of <see cref="BiDirectionalPort"/> with the specified parameters.
    /// </summary>
    /// <param name="pin">The pin associated with the bi-directional port.</param>
    /// <param name="ioController">The Meadow I/O controller.</param>
    /// <param name="initialState">The initial state of the port.</param>
    /// <param name="resistorMode">The resistor mode for the port.</param>
    /// <param name="initialDirection">The initial direction of the port.</param>
    /// <returns>A new instance of <see cref="BiDirectionalPort"/>.</returns>
    public static BiDirectionalPort From(
        IPin pin,
        IMeadowIOController ioController,
        bool initialState = false,
        ResistorMode resistorMode = ResistorMode.Disabled,
        PortDirectionType initialDirection = PortDirectionType.Input
        )
    {
        return From(pin, ioController, initialState, resistorMode, initialDirection);
    }

    /// <summary>
    /// Creates a new instance of <see cref="BiDirectionalPort"/> with the specified parameters.
    /// </summary>
    /// <param name="pin">The pin associated with the bi-directional port.</param>
    /// <param name="ioController">The Meadow I/O controller.</param>
    /// <param name="initialState">The initial state of the port.</param>
    /// <param name="resistorMode">The resistor mode for the port.</param>
    /// <param name="initialDirection">The initial direction of the port.</param>
    /// <param name="outputType">The output type for the port.</param>
    /// <returns>A new instance of <see cref="BiDirectionalPort"/>.</returns>
    public static BiDirectionalPort From(
        IPin pin,
        IMeadowIOController ioController,
        bool initialState,
        ResistorMode resistorMode,
        PortDirectionType initialDirection,
        OutputType outputType
        )
    {
        var chan = pin.SupportedChannels.OfType<IDigitalChannelInfo>().FirstOrDefault();
        if (chan == null)
        {
            throw new Exception("Unable to create an output port on the pin, because it doesn't have a digital channel");
        }
        return new BiDirectionalPort(pin, ioController, chan, initialState, resistorMode, initialDirection, outputType);
    }

    /// <summary>
    /// Finalizes the Port instance
    /// </summary>
    ~BiDirectionalPort()
    {
        Dispose(false);
    }

    /// <inheritdoc/>
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
                this.IOController.UnconfigureGpio(this.Pin);
                bool success = this.IOController.DeviceChannelManager.ReleasePin(this.Pin);
            }
            disposed = true;
        }
    }

    /// <inheritdoc/>
    public override bool State
    {
        get
        {
            Direction = PortDirectionType.Input;
            var value = IOController.GetDiscrete(this.Pin);
            return InverseLogic ? !value : value;
        }
        set
        {
            Direction = PortDirectionType.Output;
            IOController.SetDiscrete(this.Pin, InverseLogic ? !value : value);
        }
    }
}
