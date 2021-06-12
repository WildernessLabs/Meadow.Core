using Meadow.Hardware;
using System;

namespace Meadow
{
    public class SysFsDigitalOutputPort : IDigitalOutputPort
    {
        public bool InitialState { get; private set; }
        public IPin Pin { get; private set; }
        private bool LastState { get; set; }
        private int Gpio { get; set; } = -1;
        private SysFsGpioDriver Driver { get; }

        public IDigitalChannelInfo Channel => throw new NotImplementedException(); // TODO

        internal SysFsDigitalOutputPort(SysFsGpioDriver driver, IPin pin, bool initialState)
        {
            Driver = driver;
            Pin = pin;
            LastState = InitialState = initialState;
            if (pin is SysFsPin { } sp)
            {
                Gpio = sp.Gpio;
            }
            else
            {
                throw new NativeException($"Pin {pin.Name} does not support SYS FS GPIO operations");
            }

            Driver.Export(Gpio);
            Driver.SetDirection(Gpio, SysFsGpioDriver.GpioDirection.Output);
        }

        public bool State 
        {
            get => LastState;
            set
            {
                Driver.SetValue(Gpio, value);
                LastState = value;
            }
        }

        public void Dispose()
        {
            if (Gpio >= 0)
            {
                Driver.Unexport(Gpio);
            }
        }
    }
}
