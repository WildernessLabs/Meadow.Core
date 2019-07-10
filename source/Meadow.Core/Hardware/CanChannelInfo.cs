using System;
namespace Meadow.Hardware
{
    public class CanChannelInfo : DigitalChannelIInfoBase, ICanChannelInfo
    {
        public SerialDirectionType SerialDirection { get; protected set; }

        public CanChannelInfo(string name, SerialDirectionType serialDirection) 
            : base(name, true, true, true, true, true, false)
        {
            this.SerialDirection = serialDirection;
        }

    }
}
