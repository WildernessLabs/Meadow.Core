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

        private IntPtr DriverHandle => (IOController as F7GPIOManager).DriverHandle;

        protected PwmPort(
            IPin pin,
            IIOController ioController,
            IPwmChannelInfo channel,
            float frequency = 100, 
            float dutyCycle = 0
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
            var data = new Nuttx.UpdPwmCmd()
            {
                TimerId = PwmChannelInfo.Timer,
                Frequency = (uint)Frequency,
                Duty = (uint)DutyCycle
            };

            Nuttx.PwmCmd(DriverHandle, Nuttx.UpdIoctlFn.PwmStart, data);
        }

        public override void Stop()
        {
            var data = new Nuttx.UpdPwmCmd()
            {
                TimerId = PwmChannelInfo.Timer,
            };

            Nuttx.PwmCmd(DriverHandle, Nuttx.UpdIoctlFn.PwmStop, data);
        }

        public void Setup()
        {
            var data = new Nuttx.UpdPwmCmd()
            {
                TimerId = PwmChannelInfo.Timer,
            };

            Nuttx.PwmCmd(DriverHandle, Nuttx.UpdIoctlFn.PwmSetup, data);
        }

        public void Shutdown()
        {
            var data = new Nuttx.UpdPwmCmd()
            {
                TimerId = PwmChannelInfo.Timer,
            };

            Nuttx.PwmCmd(DriverHandle, Nuttx.UpdIoctlFn.PwmShutdown, data);
        }

        protected void Dispose(bool disposing) { throw new NotImplementedException(); }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}