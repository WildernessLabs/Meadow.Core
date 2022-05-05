using Meadow.Hardware;
using System;

namespace Meadow
{

    public class GpiodDigitalOutputPort : IDigitalOutputPort
    {
        public bool InitialState { get; private set; }
        public IPin Pin { get; private set; }
        private bool LastState { get; set; }

        private Gpiod Driver { get; }
        private LineInfo Line { get; }


        public IDigitalChannelInfo Channel => throw new NotImplementedException(); // TODO

        internal GpiodDigitalOutputPort(Gpiod driver, IPin pin, bool initialState)
        {
            Driver = driver;
            Pin = pin;
            InitialState = initialState;

            if (pin is GpiodPin { } gp)
            {
                Line = Driver.Request(gp);
                Line.Request(Gpiod.Interop.line_direction.GPIOD_LINE_DIRECTION_OUTPUT);
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
                Line.SetValue(value);

                LastState = value;
            }
        }

        public void Dispose()
        {
        }
    }
}
