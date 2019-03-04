using System;
namespace Meadow.Hardware
{
    public abstract class PortBase<C> : IPort<C> where C : IChannelInfo
    {
        protected bool disposed = false;

        //public abstract PortDirectionType Direction { get; }
        public abstract SignalType SignalType { get; }

        public C Channel { get; }

        public IPin Pin { get; protected set; }

        protected PortBase(IPin pin, C channel)
        {
            this.Pin = pin;
            this.Channel = channel;
        }

        public abstract void Dispose();
    }
}
