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
        public event EventHandler<PortEventArgs> Changed = delegate { };

        /// <summary>
        /// Gets or sets a value indicating whether or not this port will raise
        /// events on state change.
        /// </summary>
        /// <value><c>true</c> if interrupt enabled; otherwise, <c>false</c>.</value>
        public bool InterruptEnabled { get; protected set; }

        public abstract bool State { get; }

        protected DigitalInputPortBase(IPin pin, IDigitalChannelInfo channel, bool interruptEnabled = false) 
            : base(pin, channel)
        {
            this.InterruptEnabled = interruptEnabled;
        }

        protected void RaiseChanged(bool value)
        {
            Changed(this, new PortEventArgs() { Value = value });
        }
    }
}