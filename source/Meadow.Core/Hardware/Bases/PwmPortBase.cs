using Meadow.Units;
using System;
namespace Meadow.Hardware
{
    public abstract class PwmPortBase : DigitalPortBase, IPwmPort
    {
        public new IPwmChannelInfo Channel {
            get => (IPwmChannelInfo)base.Channel;
            protected set { base.Channel = value; }
        }

        protected PwmPortBase(
            IPin pin,
            IPwmChannelInfo channelInfo,
            Frequency frequency,
            float dutyCycle = 0,
            bool inverted = false
            ) : base (pin, channelInfo)
        {
            this.Inverted = inverted;
            this.Frequency = frequency;
            this.DutyCycle = dutyCycle;
        }

        /// <summary>
        /// The units in which time-based properties (Period and Duration) are expressed
        /// </summary>
        public TimeScale TimeScale { get; set; }

        public abstract bool Inverted { get; set; }
        public abstract float DutyCycle { get; set; }
        public abstract Frequency Frequency { get; set; }
        public abstract float Duration { get; set; }
        public abstract float Period { get; set; }
        public abstract bool State { get; }
        public abstract void Start();
        public abstract void Stop();
    }
}
