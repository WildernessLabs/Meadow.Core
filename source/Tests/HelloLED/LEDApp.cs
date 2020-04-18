using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace HelloLED
{
    class LEDApp : App<F7Micro, LEDApp>
    {
        IDigitalOutputPort _redLED;
        IDigitalOutputPort _blueLED;
        IDigitalOutputPort _greenLED;

        public LEDApp()
        {
            Console.WriteLine("Hello!");
            CreateOutputs();
            ShowLights();
        }

        public void CreateOutputs()
        {
            Console.WriteLine("Creating Outputs");
            _redLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedRed);
            _blueLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedBlue);
            _greenLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedGreen);
        }

        public void ShowLights()
        {
            var state = false;
            var stateCount = 0;

            while(true)
            {
                state = !state;

                Console.WriteLine($" Count: {++stateCount}, State: {state}");

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