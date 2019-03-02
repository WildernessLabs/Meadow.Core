using System;
namespace Meadow.Hardware
{
    public abstract class PortBase : IPort   
    {
        public abstract PortDirectionType Direction { get; protected set; }
        public abstract SignalType SignalType { get; protected set; }

        public IChannel Channel { get; protected set; }
        public IPin Pin { get; protected set; }

        protected PortBase(IPin pin)
        {
            this.Pin = pin;
        }
    }
}
