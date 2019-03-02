using System;
namespace Meadow.Hardware
{
    public abstract class PwmPortBase : IPwmPort
    {
        public IPwmChannel ChannelInfo {
            get => _channelinfo;
        } protected IPwmChannel _channelinfo;

        public PortDirectionType Direction => PortDirectionType.Output;
        public SignalType SignalType => SignalType.Digital;

        protected PwmPortBase(IPwmChannel channelInfo)
        {
            _channelinfo = channelInfo;
        }

        // TODO: review all these
        public abstract float Duration { get; set; }
        public abstract float Period { get; set; }
        public abstract float DutyCycle { get; set; }
        public abstract float Frequency { get; set; }
        public abstract bool Inverted { get; set; }
        public abstract TimeScaleFactor Scale { get; set; }

        public abstract bool State { get; }

        public abstract void Start();
        public abstract void Stop();
    }
}
