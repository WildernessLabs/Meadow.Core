using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for digital input ports.
    /// </summary>
    public abstract class DigitalInputPortBase : DigitalPortBase
    {
        public event EventHandler<PortEventArgs> Changed = delegate { };

        public bool InterruptEnabled
        {
            get { return _interruptEnabled; }
        }
        protected bool _interruptEnabled;

        protected DigitalInputPortBase(IDigitalChannelInfo channelInfo, bool interruptEnabled = false) 
            : base(channelInfo, PortDirectionType.Input)
        {
            _interruptEnabled = interruptEnabled;
        }

        protected void RaiseChanged(bool value)
        {
            Changed(this, new PortEventArgs() { Value = value });
        }
    }
}