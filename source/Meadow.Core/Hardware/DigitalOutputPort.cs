using System;
using System.Linq;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of writing analog output.
    /// </summary>
    public class DigitalOutputPort : DigitalOutputPortBase
    {
        //public override bool InitialState => base._initialState;

        protected IGpioController GpioController { get; set; }

        public override bool State 
        {
            get => _state;
            set
            {
                //_pin.GPIOManager.SetDiscrete(_pin, value);
                _state = value;
            }
        } protected bool _state;

        //// hidden constructors
        //protected DigitalOutputPort() : base(false)
        //{
        //    //nothing goes here
        //}

        /// <summary>
        /// Creates a new DigitalOutputPort from a pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="initialState"></param>
        protected DigitalOutputPort(
            IPin pin,
            IGpioController gpioController,
            IDigitalChannelInfo channel, 
            bool initialState) 
            : base(pin, channel, initialState)
        {
            this.GpioController = gpioController;

            // attempt to reserve
            var success = DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.DigitalOutput);
            if (success.Item1)
            {
                //// make sure the pin is configured as a digital output with the proper state
                //pin.GPIOManager.ConfigureOutput(pin, initialState);

                //// initialize the output state
                //pin.GPIOManager.SetDiscrete(pin, initialState);
            }
            else
            {
                throw new PortInUseException();
            }
        }

        /// <summary>
        /// From the specified pin and initialState.
        /// </summary>
        /// <returns>The from.</returns>
        /// <param name="pin">Pin.</param>
        /// <param name="initialState">If set to <c>true</c> initial state.</param>
        internal static DigitalOutputPort From(
            IPin pin,
            IGpioController gpioController,
            bool initialState = false
            )
        {
            var chan = pin.SupportedChannels.OfType<IDigitalChannelInfo>().First();
            if(chan != null) {
                //TODO: need other checks here.
                return new DigitalOutputPort(pin, gpioController, chan, initialState);
            } else {
                throw new Exception("Unable to create an output port on the pin, because it doesn't have a digital channel");
            }
        }

        //Implement IDisposable.
        public override void Dispose()
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

            if (!disposed)
            {
                if (disposing)
                {
                    //bool success = DeviceChannelManager.ReleasePin(_pin);
                }
                disposed = true;
            }
        }

        // Finalizer
        ~DigitalOutputPort()
        {
            Dispose(false);
        }
    }
}
