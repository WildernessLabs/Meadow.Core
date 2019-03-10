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
        private DigitalInputPort _esp32Pin;
        private DigitalOutputPort _esp32Reset;
        private DigitalOutputPort _esp32Boot;

        public ESP32ConnectionApp()
        {
            _red = new DigitalOutputPort(Device.Pins.OnboardLEDRed, false);
            _blue = new DigitalOutputPort(Device.Pins.OnboardLEDBlue, false);
            _green = new DigitalOutputPort(Device.Pins.OnboardLEDGreen, false);
            _esp32Reset = new DigitalOutputPort(Device.Pins.ESP_RST, true);
            _esp32Boot = new DigitalOutputPort(Device.Pins.ESP_BOOT, true);
        }

        public void Run()
        {
            _esp32Boot.State = true;
            _esp32Reset.State = false;
            Thread.Sleep(10);
            _esp32Reset.State = true;
            _esp32Pin = new DigitalInputPort(Device.Pins.ESP_UART_TX);
            while (true)
            {
                _red.State = _esp32Pin.State;
            }
        }
    }
}
