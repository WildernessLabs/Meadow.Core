using System;
namespace Meadow.Hardware
{
    public class PwmChannelInfo : DigitalChannelIInfoBase, IPwmChannelInfo
    {
        public float MinimumFrequency { get; protected set; }
        public float MaximumFrequency { get; protected set; }
        public uint Timer { get; protected set; }
        public uint TimerChannel { get; protected set; }

        public PwmChannelInfo(string name,
            uint timer,
            uint timerChannel,
            float minimumFrequency = 0,
            float maximumFrequency = 100000,
            bool pullDownCapable = false, // does this mean anything in PWM?
            bool pullUpCapable = false) // ibid
            : base(
                name,
                inputCapable: true,
                outputCapable: true,
                interruptCapable: false, // ?? i mean, technically, yes, but will we have events?
                pullDownCapable: pullDownCapable,
                pullUpCapable: pullUpCapable) //TODO: switch to C# 7.2+ to get rid of trailing names
        {
            this.Timer = timer;
            this.TimerChannel = timerChannel;
            this.MinimumFrequency = minimumFrequency;
            this.MaximumFrequency = maximumFrequency;
        }
    }
}
