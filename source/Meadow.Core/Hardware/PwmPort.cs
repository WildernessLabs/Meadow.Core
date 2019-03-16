using Meadow.Hardware;
using System;
using System.Linq;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of generating a Pulse-Width-Modulation
    /// signal; which approximates an analog output via digital pulses.
    /// 
    /// NOTE: This class has not been implemented.
    /// </summary>
    public class PwmPort : PwmPortBase
    {
        protected PwmPort(
            IPin pin,
            IIOController ioController,
            IPwmChannelInfo channel,
            float frequency = 100, 
            float dutyCycle = 0
            /*bool inverted = false*/) 
            : base (pin, channel)
        {
        }

        internal static PwmPort From(
            IPin pin,
            IIOController ioController,
            float frequency = 100,
            float dutyCycle = 0
            /*bool inverted = false*/)
        {
            var channel = pin.SupportedChannels.OfType<IPwmChannelInfo>().First();
            if (channel != null) {
                //TODO: need other checks here.
                return new PwmPort(pin, ioController, channel, frequency, dutyCycle);
            } else {
                throw new Exception("Unable to create an output port on the pin, because it doesn't have a PWM channel");
            }

        }

        ~PwmPort() { throw new NotImplementedException(); }

        public override float Duration { get; set; }
        public override float DutyCycle { get; set; }
        public override float Frequency { get; set; }
        public override float Period { get; set; }
        //public IDigitalPin Pin { get; }
        public override TimeScaleFactor Scale { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override bool State => throw new NotImplementedException();

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        protected void Dispose(bool disposing) { throw new NotImplementedException(); }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}