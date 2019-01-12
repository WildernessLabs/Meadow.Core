using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Tristate port.
    /// </summary>
    /// <remarks>A tristate port is initially an input port</remarks>
    public class BiDirectionalPort : IBiDirectionalPort, IDisposable
    {
        public event EventHandler<PortEventArgs> Changed;

        protected IDigitalPin _pin;
        protected bool _disposed;
        private bool _currentState;
        private bool _interruptEnabled;

        private PortDirectionType _currentDirection = PortDirectionType.Input;

        public bool GlitchFilter { get; set; }
        public bool InitialState { get; }
        public ResistorMode Resistor { get; }

        public BiDirectionalPort(IDigitalPin pin, bool initialState = false, bool glitchFilter = false, ResistorMode resistorMode = ResistorMode.Disabled)
        {
            InitialState = initialState;
            Resistor = resistorMode;

            // attempt to reserve the pin
            var result = DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.DigitalInput);

            if (result.Item1)
            {
                this._pin = pin;

                // make sure the pin is configured as a digital input
                _pin.GPIOManager.ConfigureInput(_pin, glitchFilter, resistorMode, false);
            }
            else
            {
                throw new PortInUseException();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // TODO: we should consider moving this logic to the finalizer
            // but the problem with that is that we don't know when it'll be called
            // but if we do it in here, we may need to check the _disposed field
            // elsewhere

            if (!_disposed)
            {
                if (disposing)
                {
                    bool success = DeviceChannelManager.ReleasePin(_pin);
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// True if the port is currently an output; otherwise false
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        public bool Active
        {
            get => _currentDirection == PortDirectionType.Output ? true : false;
            set
            {
                if (value == Active) return;

                if(value)
                {
                    _pin.GPIOManager.ConfigureOutput(_pin, _currentState);
                }
                else
                {
                    _pin.GPIOManager.ConfigureInput(_pin, GlitchFilter, Resistor, _interruptEnabled);
                }

                _currentDirection = value ? PortDirectionType.Output : PortDirectionType.Input;
            }
        }

        public bool InterrupEnabled
        {
            get => _interruptEnabled;
            set
            {
                if (value == InterrupEnabled) return;

                _interruptEnabled = value;
            }
        }

        public PortDirectionType DirectionType
        {
            get => _currentDirection; 
        }

        public SignalType SignalType
        {
            get => SignalType.Digital;
        }

        public bool State
        {
            get
            {
                return _pin.GPIOManager.GetDiscrete(_pin);
            }
            set
            {
                _pin.GPIOManager.SetDiscrete(_pin, value);
                _currentState = value;
            }
        }
    }
}
