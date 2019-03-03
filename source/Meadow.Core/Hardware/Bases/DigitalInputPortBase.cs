using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for digital input ports.
    /// </summary>
    public abstract class DigitalInputPortBase : DigitalPortBase
    {
        public event EventHandler<PortEventArgs> Changed = delegate { };

        public new IDigitalChannelInfo Channel { get; protected set; }

        public bool InterruptEnabled { get; protected set; }

        protected DigitalInputPortBase(IPin pin, bool interruptEnabled = false) 
            : base(pin, PortDirectionType.Input)
        {
            this.InterruptEnabled = interruptEnabled;

            //TODO: set the channel
            //if(pin.SupportedChannels.Contains())

        }

        protected void RaiseChanged(bool value)
        {
            Changed(this, new PortEventArgs() { Value = value });
        }
    }
}