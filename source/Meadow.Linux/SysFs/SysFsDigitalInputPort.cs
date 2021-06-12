using Meadow.Hardware;
using System;

namespace Meadow
{
    public class SysFsDigitalInputPort : IDigitalInputPort
    {
        public IPin Pin { get; private set; }
        private int Gpio { get; set; } = -1;
        private SysFsGpioDriver Driver { get; }

        public bool State => Driver.GetValue(Gpio);

        internal SysFsDigitalInputPort(SysFsGpioDriver driver, IPin pin)
        {
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
            Driver.SetDirection(Gpio, SysFsGpioDriver.GpioDirection.Input);
        }

        public void Dispose()
        {
            if (Gpio >= 0)
            {
                Driver.Unexport(Gpio);
            }
        }

        // TODO: ----- implement stuff below here -----

        public event EventHandler<DigitalPortResult> Changed = delegate { };

        public ResistorMode Resistor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public InterruptMode InterruptMode => throw new NotImplementedException();

        public double DebounceDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double GlitchDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IDigitalChannelInfo Channel => throw new NotImplementedException();

        public IDisposable Subscribe(IObserver<IChangeResult<DigitalState>> observer)
        {
            throw new NotImplementedException();
        }
    }
}
