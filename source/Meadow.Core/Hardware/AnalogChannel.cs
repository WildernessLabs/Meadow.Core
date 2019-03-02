using System;
namespace Meadow.Hardware
{
    public class AnalogChannel : ChannelBase, IAnalogChannel
    {
        public byte Precision { get; protected set; }

        public AnalogChannel(string name, byte precision) : base (name)
        {
            this.Precision = precision;
        }

    }
}
