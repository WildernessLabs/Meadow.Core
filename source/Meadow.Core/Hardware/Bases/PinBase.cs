using System.Collections.Generic;

namespace Meadow.Hardware
{

    /// <summary>
    /// Provides base implementation for IO pins.
    /// </summary>
    public abstract class PinBase : IPin
    {
        public IList<IChannelInfo>? SupportedChannels { get; protected set; }

        public string Name { get; protected set; }
        /// <summary>
        /// Identifier that the parent Device can use to identify the I/O (address, port, pin, etc)
        /// </summary>
        /// <value>The key.</value>
        public object Key { get; protected set; }

        //public abstract IChannelInfo ActiveChannel { get; protected set; }

        protected PinBase(string name, object key, IList<IChannelInfo>? supportedChannels)
        {
            this.Name = name;
            this.Key = key;
            this.SupportedChannels = supportedChannels;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected PinBase()
        {
            // make default non-callable
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public override string ToString()
        {
            return this.Name;
        }

        public void ReserveChannel<C>()
        {

        }
        public void ReleaseChannel()
        {

        }

        public virtual bool Equals(IPin other)
        {
            if (other == null) return false;

            return this.Key.Equals(other.Key);
        }

        public override bool Equals(object obj)
        {
            if (obj is IPin { } p)
            {
                return p.Equals(this);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}