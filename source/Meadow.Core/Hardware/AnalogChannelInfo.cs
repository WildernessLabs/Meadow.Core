using System;
namespace Meadow.Hardware
{
    public class AnalogChannelInfo : ChannelInfoBase, IAnalogChannelInfo
    {
        public byte Precision { get; protected set; }

        public AnalogChannelInfo(string name, byte precision) : base (name)
        {
            this.Precision = precision;
        }

    }
}
