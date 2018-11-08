using Meadow.Hardware;
using System;

namespace Meadow.Hardware
{
    public class PWM : IDisposable
    {
        public PWM(Cpu.PWMChannel channel, double frequency_Hz, double dutyCycle, bool invert) { throw new NotImplementedException(); }
        public PWM(Cpu.PWMChannel channel, uint period, uint duration, ScaleFactor scale, bool invert) { throw new NotImplementedException(); }

        ~PWM() { throw new NotImplementedException(); }

        public uint Duration { get; set; }
        public double DutyCycle { get; set; }
        public double Frequency { get; set; }
        public uint Period { get; set; }
        public Cpu.Pin Pin { get; }
        public ScaleFactor Scale { get; set; }

        public static void Start(PWM[] ports) { throw new NotImplementedException(); }
        public static void Stop(PWM[] ports) { throw new NotImplementedException(); }
        public void Dispose() { throw new NotImplementedException(); }
        public void Start() { throw new NotImplementedException(); }
        public void Stop() { throw new NotImplementedException(); }
        protected void Commit() { throw new NotImplementedException(); }
        protected void Dispose(bool disposing) { throw new NotImplementedException(); }
        protected void Init() { throw new NotImplementedException(); }
        protected void Uninit() { throw new NotImplementedException(); }

        public enum ScaleFactor : uint
        {
            Milliseconds = 1000,
            Microseconds = 1000000,
            Nanoseconds = 1000000000
        }
    }
}