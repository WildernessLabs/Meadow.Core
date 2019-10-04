using System;
namespace Meadow.Hardware
{
    public abstract class PwmPortBase : DigitalPortBase, IPwmPort
    {
        public new IPwmChannelInfo Channel { get; protected set; }

        public bool Inverted { get; set; } = false;

        protected PwmPortBase(
            IPin pin,
            IPwmChannelInfo channelInfo,
            float frequency = 100,
            float dutyCycle = 0,
            bool inverted = false
            ) : base (pin, channelInfo)
        {
            this.Inverted = inverted;
            this.Frequency = frequency;
            this.DutyCycle = dutyCycle;
        }

        // TODO: review all these
        public float DutyCycle { get; set; }
        public float Frequency { get; set; }


        public abstract float Duration { get; set; }
        public abstract float Period { get; set; }
        public abstract TimeScaleFactor Scale { get; set; }

        public abstract bool State { get; }

        public abstract void Start();
        public abstract void Stop();
    }
}
