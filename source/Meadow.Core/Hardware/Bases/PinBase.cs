using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meadow.Hardware
{
    /// <summary>
    /// Provides base implementation for IO pins.
    /// </summary>
    public abstract class PinBase : IPin
    {
        public IList<IChannelInfo> SupportedChannels { get; protected set; }

        public string Name { get; protected set; }
        /// <summary>
        /// Identifier that the parent Device can use to identify the I/O (address, port, pin, etc)
        /// </summary>
        /// <value>The key.</value>
        public object Key { get; protected set; }

        //public abstract IChannelInfo ActiveChannel { get; protected set; }

        protected PinBase(string name, object key, IList<IChannelInfo> supportedChannels)
        {
            this.Name = name;
            this.Key = key;
            this.SupportedChannels = supportedChannels;
        }

        private PinBase()
        {
        }

        public override string ToString()
        {
            return this.Name;
        }

        public void ReserveChannel<C>() { 
        
        }
        public void ReleaseChannel() { 
        
        }
    }
}