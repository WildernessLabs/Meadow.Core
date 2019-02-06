using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace ESP32ConnectionTest
{
    public class ESP32ConnectionApp : AppBase<F7Micro, ESP32ConnectionApp>
    {
        private DigitalOutputPort _red;
        private DigitalOutputPort _blue;
        private DigitalOutputPort _green;
        private DigitalOutputPort _esp32Pin;

        public ESP32ConnectionApp()
        {
            _red = new DigitalOutputPort(Device.Pins.OnboardLEDRed, false);
            _blue = new DigitalOutputPort(Device.Pins.OnboardLEDBlue, false);
            _green = new DigitalOutputPort(Device.Pins.OnboardLEDGreen, false);
            _esp32Pin = new DigitalOutputPort(Device.Pins.ESP_CS, false);
        }

        public void Run()
        {
            while (true)
            {
                _red.State = !_red.State;
                _esp32Pin.State = !_esp32Pin.State;
                Thread.Sleep(500);
            }
        }
    }
}
