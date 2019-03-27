using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for digital input ports.
    /// </summary>
    public abstract class DigitalInputPortBase : DigitalPortBase, IDigitalInputPort
    {
        /// <summary>
        /// Occurs when the state is changed. To enable this, the `interruptEnabled`
        /// parameter must be `true`.
        /// </summary>
        public event EventHandler<DigitalInputPortEventArgs> Changed = delegate { };

        /// <summary>
        /// Gets or sets a value indicating the type of interrupt monitoring this input.
        /// </summary>
        /// <value><c>true</c> if interrupt enabled; otherwise, <c>false</c>.</value>
        public InterruptMode InterruptMode { get; protected set; }

        public abstract bool State { get; }
        public abstract int DebounceDuration { get; set; }
        public abstract int GlitchFilterCycleCount { get; set; }

        protected DigitalInputPortBase(
            IPin pin,
            IDigitalChannelInfo channel,
            InterruptMode interruptMode = InterruptMode.None
            )
            : base(pin, channel)
        {
            this.InterruptMode = interruptMode;
        }

        protected void RaiseChanged(bool value, DateTime time)
        {
            Changed(this, new DigitalInputPortEventArgs() { Value = value, Time = time });
        }
    }
}