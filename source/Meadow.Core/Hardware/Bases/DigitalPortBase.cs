namespace Meadow.Hardware
{
    /// <summary>
    /// DigitalPortBase provides a base implementation for much of the 
    /// common tasks of classes implementing IDigitalPort.
    /// </summary>
    public abstract class DigitalPortBase : PortBase, IDigitalPort
    {
        public new IDigitalChannel Channel { get; protected set; }

        /// <summary>
        /// The PortDirectionType property is backed by the readonly _direction member. 
        /// This member must be set during the constructor and describes whether the 
        /// port in an input or output port.
        /// </summary>
        public override PortDirectionType Direction { get; protected set; }

        /// <summary>
        /// The PortSignalType property returns PortSignalType.Digital.
        /// </summary>
        public override SignalType SignalType => SignalType.Digital;

        /// <summary>
        /// Gets or sets the port state, either high (true), or low (false).
        /// </summary>
        public virtual bool State
        {
            get { return _state; }
            set { _state = value; }
        }
        protected bool _state = false;

        protected DigitalPortBase(IPin pin, IDigitalChannel channel, PortDirectionType direction)
            : base(pin)
        {
            this.Channel = channel;
            this.Direction = direction;
        }
    }
}
