using System;
using System.Collections.Generic;

namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for BiDirectional Ports; digital ports 
    /// that can be both input and output.
    /// </summary>
    public abstract class BiDirectionalPortBase : DigitalPortBase, IBiDirectionalPort, IDigitalInterruptPort, IDisposable
    {
        public event EventHandler<DigitalInputPortEventArgs> Changed;

        // internals
        protected bool _currentState;
        protected bool _isDisposed;
        private uint _debounceDuration;
        private uint _glitchDuration;

        public bool InitialState { get; }
        public OutputType InitialOutputType { get; }
        public ResistorMode Resistor { get; }
        protected List<IObserver<DigitalInputPortEventArgs>> _observers { get; set; } = new List<IObserver<DigitalInputPortEventArgs>>();

        public abstract bool State { get; set; }
        public abstract PortDirectionType Direction { get; set; }

        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Gets or sets a value indicating the type of interrupt monitoring this input.
        /// </summary>
        /// <value><c>true</c> if interrupt enabled; otherwise, <c>false</c>.</value>
        public InterruptMode InterruptMode { get; protected set; }

        protected BiDirectionalPortBase(
            IPin pin,
            IDigitalChannelInfo channel,
            bool initialState,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input,
            uint debounceDuration = 0,
            uint glitchDuration = 0,
            OutputType initialOutputType = OutputType.PushPull)
            : base(pin, channel)
        {
            this.InterruptMode = interruptMode;
            InitialState = initialState;
            Resistor = resistorMode;
            Direction = initialDirection;
            DebounceDuration = debounceDuration;
            GlitchDuration = glitchDuration;
            InitialOutputType = initialOutputType;
        }

        public override void Dispose()
        {
            Dispose(true);
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        public uint DebounceDuration 
        {
            get => _debounceDuration; 
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException();
                _debounceDuration = value;
            } 
        }

        public uint GlitchDuration
        {
            get => _glitchDuration; 
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException();
                _glitchDuration = value;
            } 
        }

        protected void RaiseChangedAndNotify(DigitalInputPortEventArgs changeResult)
        {
            if (_isDisposed) return;
            
            Changed?.Invoke(this, changeResult);
            // TODO: implement Subscribe patter (see DigitalInputPortBase)
            // _observers.ForEach(x => x.OnNext(changeResult));
        }
    }
}
