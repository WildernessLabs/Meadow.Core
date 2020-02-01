using System;
using System.Linq;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of writing analog output.
    /// </summary>
    public class DigitalOutputPort : DigitalOutputPortBase
    {
        protected IIOController IOController { get; }

        /// <summary>
        /// Gets or sets the state of the port.
        /// </summary>
        /// <value><c>true</c> for `HIGH`; otherwise, <c>false</c>, for `LOW`.</value>
        public override bool State 
        {
            get => this.InverseLogic ? !_state : _state;
            set
            {
                _state = this.InverseLogic ? !value : value;
                IOController.SetDiscrete(base.Pin, _state);
            }
        } protected bool _state;

        /// <summary>
        /// Creates a new DigitalOutputPort from a pin.
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="initialState"></param>
        protected DigitalOutputPort(
            IPin pin,
            IIOController ioController,
            IDigitalChannelInfo channel, 
            bool initialState) 
            : base(pin, channel, initialState)
        {
            this.IOController = ioController;

            // attempt to reserve
            var success = DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.DigitalOutput);
            if (success.Item1)
            {
                _state = this.InverseLogic ? !initialState : initialState;

                // make sure the pin is configured as a digital output with the proper state
                ioController.ConfigureOutput(pin, _state);
            }
            else
            {
                throw new PortInUseException(success.Item2);
            }
        }

        /// <summary>
        /// From the specified pin and initialState.
        /// </summary>
        /// <returns>The from.</returns>
        /// <param name="pin">Pin.</param>
        /// <param name="initialState">If set to <c>true</c> initial state.</param>
        internal static DigitalOutputPort From(IPin pin, 
            IIOController ioController,
            bool initialState = false)
        {
            var channel = pin.SupportedChannels.OfType<IDigitalChannelInfo>().FirstOrDefault();
            if(channel != null) {
                //TODO: need other checks here.
                return new DigitalOutputPort(pin, ioController, channel, initialState);
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
                    DeviceChannelManager.ReleasePin(Pin);
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