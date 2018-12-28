using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace HelloLED
{
    class LEDApp : AppBase<F7Micro, LEDApp>
    {
        private DigitalOutputPort _redLED;
        private DigitalOutputPort _blueLED;
        private DigitalOutputPort _greenLED;

        public override void Run()
        {
            CreateOutputs();
            ShowLights();
        }

        public void CreateOutputs()
        {
            _redLED = new DigitalOutputPort(Device.Pins.OnboardLEDRed, false);
            _blueLED = new DigitalOutputPort(Device.Pins.OnboardLEDBlue, false);
            _greenLED = new DigitalOutputPort(Device.Pins.OnboardLEDGreen, false);

        }

        public void ShowLights()
        {
            var state = false;

            while(true)
            {
                state = !state;

                _redLED.State = state;
                Thread.Sleep(200);
                _greenLED.State = state;
                Thread.Sleep(200);
                _blueLED.State = state;
                Thread.Sleep(200);
            }
        }
    }
}
