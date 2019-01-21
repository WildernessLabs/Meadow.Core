namespace Meadow.Hardware
{
    /// <summary>
    /// DigitalPortBase provides a base implementation for much of the 
    /// common tasks of classes implementing IDigitalPort.
    /// </summary>
    public abstract class DigitalPortBase : IDigitalPort
    {
        public IDigitalChannelInfo ChannelInfo
        {
            get => _channelinfo;
        }
        protected IDigitalChannelInfo _channelinfo;

        /// <summary>
        /// The PortDirectionType property is backed by the readonly _direction member. 
        /// This member must be set during the constructor and describes whether the 
        /// port in an input or output port.
        /// </summary>
        public PortDirectionType Direction
        {
            get { return _direction; }
        }
        protected PortDirectionType _direction;

        /// <summary>
        /// The PortSignalType property returns PortSignalType.Digital.
        /// </summary>
        public SignalType SignalType { get { return SignalType.Digital; } }

        /// <summary>
        /// Gets or sets the port state, either high (true), or low (false).
        /// </summary>
        public virtual bool State
        {
            get { return _state; }
            set { _state = value; }
        }
        protected bool _state = false;

        protected DigitalPortBase(IDigitalChannelInfo channelInfo, PortDirectionType direction)
        {
            _channelinfo = channelInfo;
            _direction = direction;
        }
    }
}
