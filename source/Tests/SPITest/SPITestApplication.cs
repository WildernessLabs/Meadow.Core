using System;
using System.Text;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Hardware.Communications;

namespace SPITest
{
    public class SPITestApplication : AppBase<F7Micro, SPITestApplication>
    {
        private DigitalOutputPort _redLED;
        private DigitalOutputPort _blueLED;
        private DigitalOutputPort _greenLED;
        private DigitalOutputPort _reset;
        private DigitalOutputPort _boot;

        public SPITestApplication()
        {
            _redLED = new DigitalOutputPort(Device.Pins.OnboardLEDRed, false);
            _blueLED = new DigitalOutputPort(Device.Pins.OnboardLEDBlue, false);
            _greenLED = new DigitalOutputPort(Device.Pins.OnboardLEDGreen, false);
            _reset = new DigitalOutputPort(Device.Pins.ESP_RST, true);
            _boot = new DigitalOutputPort(Device.Pins.ESP_BOOT, true);
        }

        public void Run()
        {
            _boot.State = true;
            _reset.State = false;
            _reset.State = true;
            SoftwareSPIBus esp32 = new SoftwareSPIBus(Device.Pins.ESP_MOSI, Device.Pins.ESP_MISO, Device.Pins.ESP_CLK, Device.Pins.ESP_CS);
            int counter = 0;
            while (true)
            {
                _blueLED.State = !_blueLED.State;
                string message = $".NET Message number {counter}          ";
                counter++;
                Console.WriteLine($"Sending message: '{message}'");
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                byte[] writeBuffer = new byte[buffer.Length + 1];
                Array.Copy(buffer, writeBuffer, buffer.Length);
                writeBuffer[buffer.Length] = 0;
                byte[] dataFromESP32 = esp32.WriteRead(writeBuffer, (ushort) writeBuffer.Length);
                Console.WriteLine("Message from ESP32: " + Encoding.UTF8.GetString(dataFromESP32));
                Thread.Sleep(500);
            }
        }
    }
}
