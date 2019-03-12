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
        private readonly IDigitalOutputPort _redLED;
        private readonly IDigitalOutputPort _blueLED;
        private readonly IDigitalOutputPort _greenLED;
        private readonly IDigitalOutputPort _reset;
        private readonly IDigitalOutputPort _boot;

        public SPITestApplication()
        {
            _redLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLEDRed);
            _blueLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLEDBlue);
            _greenLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLEDGreen);
            _reset = Device.CreateDigitalOutputPort(Device.Pins.ESP_RST);
            _boot = Device.CreateDigitalOutputPort(Device.Pins.ESP_BOOT);
        }

        public void Run()
        {
            _boot.State = true;
            _reset.State = false;
            _reset.State = true;
            var esp32 = new SoftwareSPIBus(Device, Device.Pins.ESP_MOSI, Device.Pins.ESP_MISO, Device.Pins.ESP_CLK, Device.Pins.ESP_CS);
            int counter = 0;
            while (true)
            {
                _blueLED.State = !_blueLED.State;
                string message = $".NET Message number {counter}";
                counter++;
                Console.WriteLine($"Sending message: '{message}'");
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                int esp32BufferLength = buffer.Length + 1;
                //
                //  The buffer should be a multiple of four bytes long.
                //
                if ((esp32BufferLength & 3) != 0)
                {
                    esp32BufferLength &= 0xfffc;
                    esp32BufferLength += 4;
                }
                byte[] writeBuffer = new byte[esp32BufferLength];
                Array.Clear(writeBuffer, 0, esp32BufferLength);
                Array.Copy(buffer, writeBuffer, buffer.Length);
                Console.WriteLine(DebugHelpers.PrintableBuffer(writeBuffer));
                byte[] dataFromESP32 = esp32.WriteRead(writeBuffer, (ushort) writeBuffer.Length);
                //
                Console.WriteLine("Message from ESP32:");
                Console.WriteLine(Encoding.UTF8.GetString(dataFromESP32));
                Console.WriteLine(DebugHelpers.PrintableBuffer(dataFromESP32));
                Thread.Sleep(500);
            }
        }
    }
}
