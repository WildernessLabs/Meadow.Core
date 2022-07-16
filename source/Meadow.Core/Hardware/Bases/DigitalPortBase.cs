using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// DigitalPortBase provides a base implementation for much of the 
    /// common tasks of classes implementing IDigitalPort.
    /// </summary>
    public abstract class DigitalPortBase : PortBase<IDigitalChannelInfo>, IDigitalPort
    {
        public new IDigitalChannelInfo Channel { get; protected set; }

        protected bool InverseLogic { get; }

        protected DigitalPortBase(IPin pin, IDigitalChannelInfo channel)
            : base(pin, channel)
        {
            this.InverseLogic = channel.InverseLogic;
            this.Channel = channel;
        }

        protected virtual void Dispose(bool disposing) { }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
