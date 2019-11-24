using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for a pin on the Meadow board.
    /// </summary>
    public interface IPin
    {
        IList<IChannelInfo> SupportedChannels { get; }
        string Name { get; }
        object Key { get; }

        //IChannelInfo ActiveChannel { get; }

        //void ReserveChannel<C>(); // TODO: should this return Task<bool>? (true if reserved)
        //void ReleaseChannel();

        // TODO: upgrade to C# 8 and do this:
        //public override string ToString() {
        //    return Name;
        //}
    }
}
