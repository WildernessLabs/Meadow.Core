using Meadow.Hardware;
using System;
using System.Threading.Tasks;
using static Meadow.Gpiod.Interop;

namespace Meadow
{
    public class GpiodDigitalInputPort : DigitalInputPortBase
    {
        private Gpiod Driver { get; }
        private LineInfo Line { get; }

        public override bool State => Line.GetValue();

        internal GpiodDigitalInputPort(
            Gpiod driver,
            IPin pin,
            SysFsDigitalChannelInfo channel,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            double debounceDuration = 0,
            double glitchDuration = 0)
            : base(pin, channel, interruptMode)
        {
            if (debounceDuration > 0 || glitchDuration > 0)
            {
                throw new NotSupportedException("Glitch filtering and debounce are not currently supported on this platform.");
            }

            Driver = driver;
            Pin = pin;

            line_request_flags flags = line_request_flags.None;

            if (pin is GpiodPin { } gp)
            {
                Line = Driver.GetLine(gp);
                switch (resistorMode)
                {
                    case ResistorMode.InternalPullUp:
                        flags = line_request_flags.GPIOD_LINE_REQUEST_FLAG_BIAS_PULL_UP;
                        break;
                    case ResistorMode.InternalPullDown:
                        flags = line_request_flags.GPIOD_LINE_REQUEST_FLAG_BIAS_PULL_DOWN;
                        break;
                    default:
                        flags = line_request_flags.GPIOD_LINE_REQUEST_FLAG_BIAS_DISABLE;
                        break;
                }

                Console.WriteLine($"Flags: {flags}");
                Line.RequestInput(flags);
            }
            else
            {
                throw new NativeException($"Pin {pin.Name} does not support GPIOD operations");
            }

            InterruptMode = interruptMode;

            switch(InterruptMode)
            {
                case InterruptMode.EdgeRising:
                case InterruptMode.EdgeFalling:
                case InterruptMode.EdgeBoth:
                    Line.SpawnIST(InterruptMode, flags);
                    Line.InterruptOccurred += OnInterruptOccurred;
                    break;
            }
        }

        private void OnInterruptOccurred(object sender, EventArgs e)
        {
            this.RaiseChangedAndNotify(new DigitalPortResult { New = new DigitalState(State, DateTime.UtcNow) });
        }

        public override void Dispose()
        {
            Line.Release();
        }

        public override ResistorMode Resistor
        {
            get => ResistorMode.Disabled;
            set => throw new NotSupportedException("Resistor Mode not supported on this platform");
        }

        public override double DebounceDuration
        {
            get => 0;
            set => throw new NotSupportedException("Glitch filtering and debounce are not currently supported on this platform.");
        }

        public override double GlitchDuration
        {
            get => 0;
            set => throw new NotSupportedException("Glitch filtering and debounce are not currently supported on this platform.");
        }
    }
}
