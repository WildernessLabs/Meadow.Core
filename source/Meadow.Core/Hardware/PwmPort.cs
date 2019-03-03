using Meadow.Hardware;
using System;

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
        public PwmPort(IPin pin, float frequency = 100, float dutyCycle = 0, bool invert = false) : base (pin)
        {
            this.Frequency = frequency;
            this.DutyCycle = dutyCycle;
            this.Inverted = invert;
        }

        ~PwmPort() { throw new NotImplementedException(); }

        public override float Duration { get; set; }
        public override float DutyCycle { get; set; }
        public override float Frequency { get; set; }
        public override float Period { get; set; }
        public override bool Inverted { get; set; }
        //public IDigitalPin Pin { get; }
        public override TimeScaleFactor Scale { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override bool State => throw new NotImplementedException();

        public void Dispose() { throw new NotImplementedException(); }

        public override void Start()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        protected void Dispose(bool disposing) { throw new NotImplementedException(); }

    }
}