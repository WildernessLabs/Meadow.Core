using System;
namespace Meadow.Hardware
{
    public abstract class PortBase<C> : IPort<C> where C : class, IChannelInfo
    {
        protected bool disposed = false;

        public C Channel { get; }

        public IPin Pin { get; protected set; }

        protected PortBase(IPin pin, C channel)
        {
            this.Pin = pin ?? throw new ArgumentNullException(nameof(pin));
            this.Channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        protected virtual void Dispose(bool disposing) { }

        public void Dispose()
        {
            Dispose(true);
            disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
