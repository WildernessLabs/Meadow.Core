using System;
namespace Meadow.Hardware
{
    public abstract class PortBase<C> : IPort<C>, IDisposable where C : IChannelInfo
    {
        protected bool disposed = false;

        public abstract PortDirectionType Direction { get; protected set; }
        public abstract SignalType SignalType { get; protected set; }

        public C Channel { 
            get {
                return (C)this.Pin.ActiveChannel;
            }
        }
        public IPin Pin { get; protected set; }

        protected PortBase(IPin pin)
        {
            this.Pin = pin;
            this.Pin.ReserveChannel<C>();
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed) {
                if (disposing) {
                    // dispose managed state (managed objects).
                    //Pin.ReleaseChannel(); here? underneath it's an unmanaged resource?
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                Pin.ReleaseChannel();

                disposed = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
         ~PortBase()
         {
           // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
           Dispose(false);
         }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
    }
}
