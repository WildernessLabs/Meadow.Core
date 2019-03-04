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
        protected bool _currentState;
        protected bool _interruptEnabled;
        //protected PortDirectionType _currentDirection = PortDirectionType.Input;


        public bool GlitchFilter { get; set; }
        public bool InitialState { get; }
        public ResistorMode Resistor { get; }

        protected BiDirectionalPortBase(
            IPin pin,
            IDigitalChannelInfo channel,
            bool initialState,
            bool glitchFilter,
            ResistorMode resistorMode,
            PortDirectionType initialDirection)
            : base(pin, channel)
        {
            InitialState = initialState;
            Resistor = resistorMode;
        }

        public override void Dispose()
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

        public abstract bool State { get; set; }
        public abstract PortDirectionType Direction { get; set; }

        // TODO: WAT??
        PortDirectionType IDigitalInputPort.Direction => throw new NotImplementedException();

        PortDirectionType IDigitalOutputPort.Direction => throw new NotImplementedException();
    }
}
