using Meadow.Hardware;
using System;

namespace Meadow.Hardware
{
    public class PWMPort : IPwmPort, IDisposable
    {
        public PWMPort(IPWMChannel pin, double frequency = 100, double dutyCycle = 0, bool invert = false) {}

        //TODO: fix these
        public PWMPort(Cpu.PWMChannel channel, double frequency_Hz = 100, double dutyCycle = 0, bool invert = false) { throw new NotImplementedException(); }
        public PWMPort(Cpu.PWMChannel channel, uint period, uint duration, TimeScaleFactor scale, bool invert) { throw new NotImplementedException(); }

        ~PWMPort() { throw new NotImplementedException(); }

        public double Duration { get; set; }
        public double DutyCycle { get; set; }
        public double Frequency { get; set; }
        public double Period { get; set; }
        public bool Inverted { get; set; }
        //public Cpu.Pin Pin { get; }
        public TimeScaleFactor Scale { get; set; }

        public PortDirectionType DirectionType => PortDirectionType.Output;
        public SignalType SignalType => SignalType.Digital;

        public static void Start(PWMPort[] ports) { throw new NotImplementedException(); }
        public static void Stop(PWMPort[] ports) { throw new NotImplementedException(); }
        public void Dispose() { throw new NotImplementedException(); }
        public void Start() { throw new NotImplementedException(); }
        public void Stop() { throw new NotImplementedException(); }
        protected void Commit() { throw new NotImplementedException(); }
        protected void Dispose(bool disposing) { throw new NotImplementedException(); }
        protected void Init() { throw new NotImplementedException(); }
        protected void Uninit() { throw new NotImplementedException(); }

    }
}