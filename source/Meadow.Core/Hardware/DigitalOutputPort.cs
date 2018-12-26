using System;

namespace Meadow.Hardware
{
    public class DigitalOutputPort : DigitalOutputPortBase
    {
        //protected H.OutputPort _digitalOutPort = null
        protected IPin _pin;
        protected bool _disposed;

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
                this._pin = pin;
                //TODO: do interop
            }
            else {
                throw new PortInUseException();
            }
        }

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // TODO: we should consider moving this logic to the finalizer
            // but the problem with that is that we don't know when it'll be called
            // but if we do it in here, we may need to check the _disposed field
            // elsewhere

            if (!_disposed)
            {
                if (disposing)
                {
                    bool success = DeviceChannelManager.ReleasePin(_pin);
                }
                _disposed = true;
            }
        }

        // Finalizer
        ~DigitalOutputPort()
        {
            Dispose(false);
        }
    }
}
