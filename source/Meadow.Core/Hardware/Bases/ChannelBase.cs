using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides base functionality for channel types.
    /// </summary>
    public abstract class ChannelBase
    {
        public string Name { get; protected set; }

        protected ChannelBase(string name)
        {
            this.Name = name;
        }
    }
}
