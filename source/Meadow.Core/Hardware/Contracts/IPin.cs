using System;
using System.Collections.Generic;

namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for a pin on the Meadow board.
    /// </summary>
    public interface IPin
    {
        IList<IChannel> SupportedChannels { get; }
        string Name { get; }
        object Key { get; }
    }
}
