namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for digital output ports.
    /// </summary>
    public abstract class DigitalOutputPortBase : DigitalPortBase, IDigitalOutputPort
    {
        /// <summary>
        /// The InitialState property is backed by the readonly bool 
        /// _initialState member and should be during the constructor.
        /// </summary>
        public abstract bool InitialState { get; }
        protected readonly bool _initialState;

        protected DigitalOutputPortBase(IDigitalChannelInfo channelInfo, bool initialState) 
            : base(channelInfo, PortDirectionType.Output)
        {
            _initialState = initialState;
            _state = initialState;
        }
    }
}
