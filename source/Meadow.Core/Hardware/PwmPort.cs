using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Linq;
using static Meadow.Core.Interop;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of generating a Pulse-Width-Modulation
    /// signal; which approximates an analog output via digital pulses.
    /// </summary>
    public class PwmPort : PwmPortBase
    {
        protected IIOController IOController { get; set; }
        protected IPwmChannelInfo PwmChannelInfo { get; set; }

        protected PwmPort(
            IPin pin,
            IIOController ioController,
            IPwmChannelInfo channel
            /*bool inverted = false*/) 
            : base (pin, channel)
        {
            this.IOController = ioController;
            this.PwmChannelInfo = channel;
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
                var port = new PwmPort(pin, ioController, channel);
                port.Frequency = frequency;
                port.DutyCycle = dutyCycle;

                return port;

            }
            else {
                throw new Exception("Unable to create an output port on the pin, because it doesn't have a PWM channel");
            }

        }

        ~PwmPort() { throw new NotImplementedException(); }

        public override float Duration { get; set; }
        //public override float DutyCycle { get; set; }
        //public override float Frequency { get; set; }
        public override float Period
        {
            get => 1.0f / Frequency; 
            set
            {
                Frequency = 1.0f / value;
            }
        }
        //public IDigitalPin Pin { get; }
        public override TimeScaleFactor Scale { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override bool State => throw new NotImplementedException();

        public override void Start()
        {
            UPD.PWM.Start(PwmChannelInfo, (uint)Frequency, DutyCycle);
        }

        public override void Stop()
        {
            UPD.PWM.Stop(PwmChannelInfo.Timer);
        }

        protected void Dispose(bool disposing)
        {
            Stop();
            UPD.PWM.Shutdown(PwmChannelInfo.Timer);
        }

        public override void Dispose()
        {
            Dispose(true);
        }
    }
}