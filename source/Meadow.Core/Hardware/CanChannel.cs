using System;
namespace Meadow.Hardware
{
    public class CanChannel : DigitalChannelBase, ICanChannel
    {
        public SerialDirectionType SerialDirection { get; protected set; }

        public CanChannel(string name, SerialDirectionType serialDirection) 
            : base(name, true, true, true, true, true)
        {
            this.SerialDirection = serialDirection;
        }

    }
}
