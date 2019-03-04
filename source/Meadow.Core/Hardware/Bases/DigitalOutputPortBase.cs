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
        public bool InitialState { get; protected set; }

        public PortDirectionType Direction => PortDirectionType.Output;

        public abstract bool State { get; set; }

        protected DigitalOutputPortBase(IPin pin, IDigitalChannelInfo channel, bool initialState) 
            : base(pin, channel)
        {
            this.InitialState = initialState;
            //_state = initialState;
        }
    }
}
