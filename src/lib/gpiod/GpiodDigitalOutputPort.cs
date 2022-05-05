using Meadow.Hardware;
using System;

namespace Meadow
{

    public class GpiodDigitalOutputPort : IDigitalOutputPort
    {
        public bool InitialState { get; private set; }
        public IPin Pin { get; private set; }
        private bool LastState { get; set; }
        private int Gpio { get; set; } = -1;
        private Gpiod Driver { get; }

        private GpioHandleRequest _request;

        public IDigitalChannelInfo Channel => throw new NotImplementedException(); // TODO

        internal GpiodDigitalOutputPort(Gpiod driver, IPin pin, bool initialState)
        {
            Driver = driver;
            Pin = pin;
            InitialState = initialState;

            if (pin is GpiodPin { } gp)
            {
                Gpio = gp.Gpio;
            }
            else
            {
                throw new NativeException($"Pin {pin.Name} does not support GPIOD operations");
            }

        }

        public bool State
        {
            get => LastState;
            set
            {
                Driver.SetValue(_request, value);

                LastState = value;
            }
        }

        public void Dispose()
        {
            Interop.close(_request.FileDescriptor);
        }
    }
}
