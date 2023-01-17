using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow
{
    public class GpiodPin : Pin
    {
        public string Chip { get; }
        public int Offset { get; }

        public GpiodPin(string name, object key, string chip, int offset, IList<IChannelInfo>? supportedChannels = null)
            : base(name, key, supportedChannels)
        {
            Chip = chip;
            Offset = offset;
        }
    }
}
