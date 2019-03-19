using System;
using System.Diagnostics;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace HelloLED
{
    class LEDApp : AppBase<F7Micro, LEDApp>
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
            Debug.WriteLine("Creating Outpus");
            _redLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLEDRed);
            _blueLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLEDBlue);
            _greenLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLEDGreen);
        }

        public void ShowLights()
        {
            var state = false;

            while(true)
            {
                state = !state;

                Console.WriteLine($"State: {state}");

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
