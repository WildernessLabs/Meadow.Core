namespace Meadow.Hardware
{
    public class DigitalOutputPort : DigitalOutputPortBase
    {
        //protected H.OutputPort _digitalOutPort = null;

        public override bool InitialState => base._initialState;

        public override bool State 
        {
            get => _state;
            set
            {
                // TODO: Write to port
                //_digitalOutPort.Write(value);
                _state = value;
            }
        }

        // hidden constructors
        protected DigitalOutputPort() : base(false)
        {
            //nothing goes here
        }

        /// <summary>
        /// Creates a new DigitalOutputPort from a pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="initialState"></param>
        public DigitalOutputPort(IDigitalPin pin, bool initialState = false) : base(initialState)
        {
            //this._digitalOutPort = new H.OutputPort(pin, initialState);

            // attempt to reserve
            var success = DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.Digital);
            if(success.Item1)
            {
                //TODO: do interop
            } else {
                throw new PortInUseException();
            }
        }
    }
}
