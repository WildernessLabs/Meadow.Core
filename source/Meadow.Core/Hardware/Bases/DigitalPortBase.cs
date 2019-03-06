namespace Meadow.Hardware
{
    /// <summary>
    /// DigitalPortBase provides a base implementation for much of the 
    /// common tasks of classes implementing IDigitalPort.
    /// </summary>
    public abstract class DigitalPortBase : PortBase<IDigitalChannelInfo>, IDigitalPort
    {
        public new IDigitalChannelInfo Channel { get; protected set; }

        /// <summary>
        /// The PortSignalType property returns PortSignalType.Digital.
        /// </summary>
        public override SignalType SignalType => SignalType.Digital;


        protected DigitalPortBase(IPin pin, IDigitalChannelInfo channel)
            : base(pin, channel)
        {
            this.Channel = channel;
        }
    }
}
