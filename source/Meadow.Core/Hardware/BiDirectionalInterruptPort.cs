using System;
using System.Linq;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of reading and writing digital input
    /// and output.
    /// </summary>
    public class BiDirectionalInterruptPort : BiDirectionalInterruptPortBase
    {
        private PortDirectionType _currentDirection;
        private TimeSpan _debounceDuration;
        private TimeSpan _glitchDuration;

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
                if (value == PortDirectionType.Input)
                {
                    this.IOController.ConfigureInput(this.Pin, this.Resistor, this.InterruptMode, this._debounceDuration, this._glitchDuration);
                }
                else
                {
                    // InterruptMode.None disables interrupts within Nuttx via WireInterrupt
                    this.IOController.ConfigureInput(this.Pin, this.Resistor, InterruptMode.None, TimeSpan.Zero, TimeSpan.Zero);
                    this.IOController.ConfigureOutput(this.Pin, this.InitialState, InitialOutputType);
                }
                _currentDirection = value;
            }
        }

        /// <summary>
        /// Protected constructor for creating a <see cref="BiDirectionalInterruptPort"/> with default debounce and glitch durations.
        /// </summary>
        /// <param name="pin">The pin associated with the bi-directional interrupt port.</param>
        /// <param name="channel">The digital channel information.</param>
        /// <param name="gpioController">The Meadow I/O controller.</param>
        /// <param name="initialState">The initial state of the port.</param>
        /// <param name="interruptMode">The interrupt mode for the port.</param>
        /// <param name="resistorMode">The resistor mode for the port.</param>
        /// <param name="initialDirection">The initial direction of the port.</param>
        protected BiDirectionalInterruptPort(
            IPin pin,
            IDigitalChannelInfo channel,
            IMeadowIOController gpioController,
            bool initialState,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input)
            : this(pin, gpioController, channel, initialState, interruptMode, resistorMode, initialDirection, debounceDuration: TimeSpan.Zero, glitchDuration: TimeSpan.Zero, outputType: OutputType.PushPull)
        {
        }

        /// <summary>
        /// Protected constructor for creating a <see cref="BiDirectionalInterruptPort"/>.
        /// </summary>
        /// <param name="pin">The pin associated with the bi-directional interrupt port.</param>
        /// <param name="gpioController">The Meadow I/O controller.</param>
        /// <param name="channel">The digital channel information.</param>
        /// <param name="initialState">The initial state of the port.</param>
        /// <param name="interruptMode">The interrupt mode for the port.</param>
        /// <param name="resistorMode">The resistor mode for the port.</param>
        /// <param name="initialDirection">The initial direction of the port.</param>
        /// <param name="debounceDuration">The debounce duration for the port.</param>
        /// <param name="glitchDuration">The glitch duration for the port.</param>
        /// <param name="outputType">The output type for the port.</param>
        protected BiDirectionalInterruptPort(
            IPin pin,
            IMeadowIOController gpioController,
            IDigitalChannelInfo channel,
            bool initialState,
            InterruptMode interruptMode,
            ResistorMode resistorMode,
            PortDirectionType initialDirection,
            TimeSpan debounceDuration,
            TimeSpan glitchDuration,
            OutputType outputType
            )
            : base(pin, channel, initialState, interruptMode, resistorMode, initialDirection, debounceDuration, glitchDuration, outputType)
        {
            if (interruptMode != InterruptMode.None && (!channel.InterruptCapable))
            {
                throw new Exception("Unable to create port; channel is not capable of interrupts");
            }
            if (interruptMode != InterruptMode.None && (!channel.InputCapable))
            {
                throw new Exception("Unable to create port; channel is not capable of inputs");
            }
            if (interruptMode != InterruptMode.None && (!channel.OutputCapable))
            {
                throw new Exception("Unable to create port; channel is not capable of outputs");
            }
            if (debounceDuration.TotalMilliseconds < 0.0 || debounceDuration.TotalMilliseconds > 1000.0)
            {
                throw new ArgumentOutOfRangeException(nameof(debounceDuration), debounceDuration, "Unable to create an input port, because debounceDuration is out of range (0.1-1000.0)");
            }
            if (glitchDuration.TotalMilliseconds < 0.0 || glitchDuration.TotalMilliseconds > 1000.0)
            {
                throw new ArgumentOutOfRangeException(nameof(glitchDuration), "Unable to create an input port, because glitchDuration is out of range (0.1-1000.0)");
            }

            this.IOController = gpioController ?? throw new ArgumentNullException(nameof(gpioController));
            this.IOController.Interrupt += OnInterrupt;

            // attempt to reserve the pin - we'll reserve it as an input even though we use it for bi-directional
            var result = this.IOController.DeviceChannelManager.ReservePin(
                this.Pin,
                ChannelConfigurationType.DigitalInput);

            if (result.Item1)
            {
                _currentDirection = initialDirection;

                // make sure the pin direction (and state for outputs) is configured as desired
                if (_currentDirection == PortDirectionType.Input)
                {
                    // This call will ultimately result in Nuttx being called
                    this.IOController.ConfigureInput(this.Pin, this.Resistor, interruptMode, debounceDuration, glitchDuration);
                }
                else
                {
                    // InterruptMode.None disables interrupts within Nuttx via WireInterrupt
                    this.IOController.ConfigureInput(this.Pin, this.Resistor, InterruptMode.None, TimeSpan.Zero, TimeSpan.Zero);
                    this.IOController.ConfigureOutput(this.Pin, InverseLogic ? !this.InitialState : this.InitialState, outputType);
                }
            }
            else
            {
                throw new PortInUseException($"{this.GetType().Name}: Pin {pin.Name} is already in use");
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="BiDirectionalInterruptPort"/> with default debounce and glitch durations.
        /// </summary>
        /// <param name="pin">The pin associated with the bi-directional interrupt port.</param>
        /// <param name="ioController">The Meadow I/O controller.</param>
        /// <param name="initialState">The initial state of the port.</param>
        /// <param name="interruptMode">The interrupt mode for the port.</param>
        /// <param name="resistorMode">The resistor mode for the port.</param>
        /// <param name="initialDirection">The initial direction of the port.</param>
        /// <returns>A new instance of <see cref="BiDirectionalInterruptPort"/>.</returns>
        public static BiDirectionalInterruptPort From(
            IPin pin,
            IMeadowIOController ioController,
            bool initialState = false,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input
            )
        {
            return From(pin, ioController, initialState, interruptMode, resistorMode, initialDirection, TimeSpan.Zero, TimeSpan.Zero, OutputType.PushPull);
        }

        /// <summary>
        /// Creates a new instance of <see cref="BiDirectionalInterruptPort"/>.
        /// </summary>
        /// <param name="pin">The pin associated with the bi-directional interrupt port.</param>
        /// <param name="ioController">The Meadow I/O controller.</param>
        /// <param name="initialState">The initial state of the port.</param>
        /// <param name="interruptMode">The interrupt mode for the port.</param>
        /// <param name="resistorMode">The resistor mode for the port.</param>
        /// <param name="initialDirection">The initial direction of the port.</param>
        /// <param name="debounceDuration">The debounce duration for the port.</param>
        /// <param name="glitchDuration">The glitch duration for the port.</param>
        /// <param name="outputType">The output type for the port.</param>
        /// <returns>A new instance of <see cref="BiDirectionalInterruptPort"/>.</returns>
        public static BiDirectionalInterruptPort From(
            IPin pin,
            IMeadowIOController ioController,
            bool initialState,
            InterruptMode interruptMode,
            ResistorMode resistorMode,
            PortDirectionType initialDirection,
            TimeSpan debounceDuration,
            TimeSpan glitchDuration,
            OutputType outputType
            )
        {
            var chan = pin.SupportedChannels.OfType<IDigitalChannelInfo>().FirstOrDefault();
            if (chan == null)
            {
                throw new Exception("Unable to create an output port on the pin, because it doesn't have a digital channel");
            }
            return new BiDirectionalInterruptPort(pin, ioController, chan, initialState, interruptMode, resistorMode, initialDirection, debounceDuration, glitchDuration, outputType);
        }

        /// <summary>
        /// Finalizes the Port instance
        /// </summary>
        ~BiDirectionalInterruptPort()
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
                    this.IOController.Interrupt -= OnInterrupt;
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

        /// <inheritdoc/>
        public override TimeSpan DebounceDuration
        {
            get => _debounceDuration;
            set
            {
                if (value.TotalMilliseconds < 0.0 || value.TotalMilliseconds > 1000.0) throw new ArgumentOutOfRangeException("DebounceDuration");
                _debounceDuration = value;
                // Update in MCU
                this.IOController.WireInterrupt(Pin, InterruptMode, this.Resistor, _debounceDuration, _glitchDuration);
            }
        }

        /// <inheritdoc/>
        public override TimeSpan GlitchDuration
        {
            get => _glitchDuration;
            set
            {
                if (value.TotalMilliseconds < 0.0 || value.TotalMilliseconds > 1000.0) throw new ArgumentOutOfRangeException("GlitchDuration");
                _glitchDuration = value;
                // Update in MCU
                this.IOController.WireInterrupt(Pin, InterruptMode, this.Resistor, _debounceDuration, _glitchDuration);
            }
        }

        private void OnInterrupt(IPin pin, bool state)
        {
            if (pin == this.Pin)
            {
                var capturedLastTime = LastEventTime; // note: doing this for latency reasons. kind of. sort of. bad time good time. all time.
                this.LastEventTime = DateTime.Now;
                // BC 2021.05.21 b5.0: Changed this to the new result type.
                // assuming that old state is just an inversion of the new state, yeah?
                RaiseChangedAndNotify(new DigitalPortResult(new DigitalState(state, this.LastEventTime), new DigitalState(!state, capturedLastTime)));
                //RaiseChangedAndNotify(new DigitalInputPortChangeResult(state, this.LastEventTime, capturedLastTime));
            }
        }
    }
}
