using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for digital input ports.
    /// </summary>
    public abstract class DigitalInputPortBase : DigitalPortBase, IDigitalInputPort
    {
        public event EventHandler<PortEventArgs> Changed = delegate { };

        public bool InterruptEnabled { get; protected set; }
        public PortDirectionType Direction => PortDirectionType.Output;

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