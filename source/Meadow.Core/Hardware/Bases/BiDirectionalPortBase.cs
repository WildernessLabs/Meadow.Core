using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for BiDirectional Ports; digital ports 
    /// that can be both input and output.
    /// </summary>
    public abstract class BiDirectionalPortBase : DigitalPortBase, IBiDirectionalPort, IDisposable
    {
        public event EventHandler<PortEventArgs> Changed;

        // internals
        protected bool _disposed;
        protected bool _currentState;
        protected bool _interruptEnabled;
        //protected PortDirectionType _currentDirection = PortDirectionType.Input;


        public bool GlitchFilter { get; set; }
        public bool InitialState { get; }
        public ResistorMode Resistor { get; }

        protected BiDirectionalPortBase(
            IPin pin,
            bool initialState = false,
            bool glitchFilter = false,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input)
            : base(pin, initialDirection)
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
    }
}
