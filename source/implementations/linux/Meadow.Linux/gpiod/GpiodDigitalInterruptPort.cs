using Meadow.Hardware;
using System;
using static Meadow.Gpiod.Interop;

namespace Meadow
{
    public class GpiodDigitalInterruptPort : DigitalInterruptPortBase
    {
        private Gpiod Driver { get; }
        private LineInfo Line { get; }

        public override bool State => Line.GetValue();

        internal GpiodDigitalInterruptPort(
            Gpiod driver,
            IPin pin,
            GpiodDigitalChannelInfo channel,
            InterruptMode interruptMode,
            ResistorMode resistorMode,
            TimeSpan debounceDuration,
            TimeSpan glitchDuration)
            : base(pin, channel, interruptMode)
        {
            if (debounceDuration != TimeSpan.Zero || glitchDuration != TimeSpan.Zero)
            {
                throw new NotSupportedException("Glitch filtering and debounce are not currently supported on this platform.");
            }

            Driver = driver;
            Pin = pin;

            line_request_flags flags = line_request_flags.None;

            LineInfo? li = null;

            if (pin is GpiodPin { } gp)
            {
                li = Driver.GetLine(gp);
            }
            else if (pin is LinuxFlexiPin { } lp)
            {
                li = Driver.GetLine(lp);
            }

            if (li != null)
            {
                Line = li;
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

                InterruptMode = interruptMode;

                switch (InterruptMode)
                {
                    case InterruptMode.EdgeRising:
                    case InterruptMode.EdgeFalling:
                    case InterruptMode.EdgeBoth:
                        Line.InterruptOccurred += OnInterruptOccurred;
                        Line.RequestInterrupts(InterruptMode, flags);
                        break;
                    default:
                        Line.RequestInput(flags);
                        break;
                }

            }
            else
            {
                throw new NativeException($"Pin {pin.Name} does not support GPIOD operations");
            }
        }

        private void OnInterruptOccurred(LineInfo sender, gpiod_line_event e)
        {
            var state = e.event_type == gpiod_event_type.GPIOD_LINE_EVENT_RISING_EDGE ? true : false;

            this.RaiseChangedAndNotify(new DigitalPortResult { New = new DigitalState(state, DateTime.UtcNow) }); // TODO: convert event time?
        }

        protected override void Dispose(bool disposing)
        {
            Line.Release();
            base.Dispose(disposing);
        }

        public override ResistorMode Resistor
        {
            get => ResistorMode.Disabled;
            set => throw new NotSupportedException("Resistor Mode not supported on this platform");
        }

        public override TimeSpan DebounceDuration
        {
            get => TimeSpan.Zero;
            set => throw new NotSupportedException("Glitch filtering and debounce are not currently supported on this platform.");
        }

        public override TimeSpan GlitchDuration
        {
            get => TimeSpan.Zero;
            set => throw new NotSupportedException("Glitch filtering and debounce are not currently supported on this platform.");
        }
    }
}
