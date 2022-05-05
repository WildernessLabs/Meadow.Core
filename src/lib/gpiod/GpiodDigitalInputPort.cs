using Meadow.Hardware;
using System;

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
            if (resistorMode != ResistorMode.Disabled)
            {
                throw new NotSupportedException("Resistor Mode not supported on this platform");
            }
            if (debounceDuration > 0 || glitchDuration > 0)
            {
                throw new NotSupportedException("Glitch filtering and debounce are not currently supported on this platform.");
            }

            Driver = driver;
            Pin = pin;

            if (pin is GpiodPin { } gp)
            {
                Line = Driver.Request(gp);
                Line.Request(Gpiod.Interop.line_direction.GPIOD_LINE_DIRECTION_INPUT);
            }
            else
            {
                throw new NativeException($"Pin {pin.Name} does not support GPIOD operations");
            }


            InterruptMode = interruptMode;
        }

        public override void Dispose()
        {
            
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
