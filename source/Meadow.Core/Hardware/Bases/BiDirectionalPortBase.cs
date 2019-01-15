using System;
namespace Meadow.Hardware
{
    public abstract class BiDirectionalPortBase : IBiDirectionalPort, IDisposable
    {
        public event EventHandler<PortEventArgs> Changed;

        // internals
        protected bool _disposed;
        protected bool _currentState;
        protected bool _interruptEnabled;
        protected PortDirectionType _currentDirection = PortDirectionType.Input;

        public SignalType SignalType
        {
            get => SignalType.Digital;
        }

        public bool GlitchFilter { get; set; }
        public bool InitialState { get; }
        public ResistorMode Resistor { get; }

        protected BiDirectionalPortBase(
            IDigitalPin pin,
            bool initialState = false,
            bool glitchFilter = false,
            ResistorMode resistorMode = ResistorMode.Disabled)
        {
            InitialState = initialState;
            Resistor = resistorMode;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract void Dispose(bool disposing);

        public bool InterrupEnabled
        {
            get => _interruptEnabled;
            set
            {
                if (value == InterrupEnabled) return;
                _interruptEnabled = value;
            }
        }

        public PortDirectionType Direction
        {
            get => _currentDirection;
        }

        public abstract bool State { get; set; }
    }
}
