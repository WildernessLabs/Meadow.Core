using System;
using System.Linq;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of reading digital input.
    /// </summary>
    public class DigitalInputPort : DigitalInputPortBase
    {
        private ResistorMode _resistorMode;

        protected IMeadowIOController IOController { get; set; }

        private DateTime LastEventTime { get; set; } = DateTime.MinValue;

        protected DigitalInputPort(
            IPin pin,
            IMeadowIOController ioController,
            IDigitalChannelInfo channel,
            ResistorMode resistorMode
            ) : base(pin, channel)
        {
            this.IOController = ioController;
            this._resistorMode = resistorMode;

            // attempt to reserve
            var success = this.IOController.DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.DigitalInput);
            if (success.Item1)
            {
                // make sure the pin is configured as a digital input with the proper state
                ioController.ConfigureInput(pin, resistorMode, InterruptMode.None, TimeSpan.Zero, TimeSpan.Zero);
            }
            else
            {
                throw new PortInUseException($"{this.GetType().Name}: Pin {pin.Name} is already in use");
            }
        }

        public static DigitalInputPort From(
            IPin pin,
            IMeadowIOController ioController,
            ResistorMode resistorMode
            )
        {
            var chan = pin.SupportedChannels.OfType<IDigitalChannelInfo>().FirstOrDefault();

            if (chan == null)
            {
                throw new Exception("Unable to create an input port on the pin, because it doesn't have a digital channel");
            }

            var port = new DigitalInputPort(pin, ioController, chan, resistorMode);
            return port;
        }

        /// <summary>
        /// Gets or Sets the internal resistor mode for the input
        /// </summary>
        public override ResistorMode Resistor
        {
            get => _resistorMode;
            set
            {
                IOController.SetResistorMode(this.Pin, value);
                _resistorMode = value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            // TODO: we should consider moving this logic to the finalizer
            // but the problem with that is that we don't know when it'll be called
            // but if we do it in here, we may need to check the _disposed field
            // elsewhere
            if (!disposed)
            {
                if (disposing)
                {
                    this.IOController.DeviceChannelManager.ReleasePin(Pin);
                    IOController.UnconfigureGpio(Pin);
                }
                disposed = true;
            }
        }

        // Finalizer
        ~DigitalInputPort()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the current State of the input (True == high, False == low)
        /// </summary>
        public override bool State
        {
            get
            {
                var state = this.IOController.GetDiscrete(this.Pin);
                return InverseLogic ? !state : state;
            }
        }
    }
}