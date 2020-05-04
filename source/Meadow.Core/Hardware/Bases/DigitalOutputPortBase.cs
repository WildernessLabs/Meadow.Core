using Meadow.Devices;

namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for digital output ports.
    /// </summary>
    public abstract class DigitalOutputPortBase : DigitalPortBase, IDigitalOutputPort
    {
        /// <summary>
        /// The initial state of the port.
        /// </summary>
        public bool InitialState { get; protected set; }
        public OutputType InitialOutputType { get; protected set; }

        public abstract bool State { get; set; }

        protected DigitalOutputPortBase(IPin pin, IDigitalChannelInfo channel, bool initialState, OutputType initialOutputType) 
            : base(pin, channel)
        {
            this.InitialState = initialState;
            this.InitialOutputType = initialOutputType;
        }
    }
}
