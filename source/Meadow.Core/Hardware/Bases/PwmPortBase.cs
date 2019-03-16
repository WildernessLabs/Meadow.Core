using System;
namespace Meadow.Hardware
{
    public abstract class PwmPortBase : DigitalPortBase, IPwmPort
    {
        public new IPwmChannelInfo Channel { get; protected set; }

        public bool Inverted { get; set; } = false;

        protected PwmPortBase(
            IPin pin,
            IPwmChannelInfo channelInfo
            //bool inverted = false
            ) : base (pin, channelInfo)
        {
            //this.Inverted = inverted;
        }

        // TODO: review all these
        public abstract float Duration { get; set; }
        public abstract float Period { get; set; }
        public abstract float DutyCycle { get; set; }
        public abstract float Frequency { get; set; }
        public abstract TimeScaleFactor Scale { get; set; }

        public abstract bool State { get; }

        public abstract void Start();
        public abstract void Stop();
    }
}
