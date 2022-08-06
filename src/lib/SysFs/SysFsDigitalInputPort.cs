using Meadow.Hardware;
using System;
using System.Threading;

namespace Meadow
{
    public class SysFsDigitalInputPort : DigitalInputPortBase, IDigitalInputPort
    {
        private int Gpio { get; set; } = -1;
        private SysFsGpioDriver Driver { get; }

        public override bool State => Driver.GetValue(Gpio);

        internal SysFsDigitalInputPort(
            SysFsGpioDriver driver,
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
            if (pin is SysFsPin { } sp)
            {
                Gpio = sp.Gpio;
            }
            else
            {
                throw new NativeException($"Pin {pin.Name} does not support SYS FS GPIO operations");
            }

            Driver.Export(Gpio);
            Thread.Sleep(100); // this seems to be required to prevent an error 13
            Driver.SetDirection(Gpio, SysFsGpioDriver.GpioDirection.Input);
            switch (interruptMode)
            {
                case InterruptMode.None:
                    // nothing to do
                    break;
                default:
                    Driver.HookInterrupt(Gpio, interruptMode, InterruptCallback);
                    break;
            }

            InterruptMode = interruptMode;
        }

        private void InterruptCallback()
        {
            // TODO: implement old/new
            RaiseChangedAndNotify(new DigitalPortResult());
        }

        protected override void Dispose(bool disposing)
        {
            if (Gpio >= 0)
            {
                Driver.UnhookInterrupt(Gpio);
                Driver.Unexport(Gpio);
            }

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
