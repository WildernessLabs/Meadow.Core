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
        public event EventHandler<DigitalPortResult> Changed = delegate { };

        // internals
        protected bool _currentState;
        protected bool _isDisposed;

        public bool InitialState { get; }
        public OutputType InitialOutputType { get; }
        public ResistorMode Resistor { get; }
        protected List<IObserver<DigitalPortResult>> _observers { get; set; } = new List<IObserver<DigitalPortResult>>();

        public abstract bool State { get; set; }
        public abstract PortDirectionType Direction { get; set; }

        protected abstract void Dispose(bool disposing);

        protected TimeSpan _debounceDuration;
        protected TimeSpan _glitchDuration;

        public abstract TimeSpan DebounceDuration { get; set; }
        public abstract TimeSpan GlitchDuration { get; set; }

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
            PortDirectionType initialDirection = PortDirectionType.Input)
            : this(pin, channel, initialState, interruptMode, resistorMode, initialDirection, debounceDuration: TimeSpan.Zero, glitchDuration: TimeSpan.Zero, initialOutputType: OutputType.PushPull)
        {
        }

        protected BiDirectionalPortBase(
            IPin pin,
            IDigitalChannelInfo channel,
            bool initialState,
            InterruptMode interruptMode,
            ResistorMode resistorMode,
            PortDirectionType initialDirection,
            TimeSpan debounceDuration,
            TimeSpan glitchDuration,
            OutputType initialOutputType)
            : base(pin, channel)
        {
            this.InterruptMode = interruptMode;
            InitialState = initialState;
            Resistor = resistorMode;
            Direction = initialDirection;
            _debounceDuration = debounceDuration;   // Don't trigger WireInterrupt call via property
            _glitchDuration = glitchDuration;       // Don't trigger WireInterrupt call via property
            InitialOutputType = initialOutputType;
        }

        public override void Dispose()
        {
            Dispose(true);
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        protected void RaiseChangedAndNotify(DigitalPortResult changeResult)
        {
            if (_isDisposed) return;

            Changed?.Invoke(this, changeResult);
            // TODO: implement Subscribe patter (see DigitalInputPortBase)
            // _observers.ForEach(x => x.OnNext(changeResult));
        }
    }
}
