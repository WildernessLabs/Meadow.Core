using System;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace BasicMeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        IDigitalOutputPort redLED;
        IDigitalOutputPort blueLED;
        IDigitalOutputPort greenLED;

        public MeadowApp()
        {
            ConfigurePorts();
            BlinkLed();
        }

        protected void ConfigurePorts()
        {
            // create ports for the onboard LED
            redLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLEDRed);
            blueLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLEDBlue);
            greenLED = Device.CreateDigitalOutputPort(Device.Pins.OnboardLEDGreen);
        }

        protected Task BlinkLed()
        {
            // create a task to walk through some colors on the LED
            Task blinky = new Task(() => {
                var state = false;
                while (true) {
                    state = !state;

                    redLED.State = state;
                    Task.Delay(200);

                    greenLED.State = state;
                    Task.Delay(200);

                    blueLED.State = state;
                    Task.Delay(200);
                }
            });
            blinky.Start();
            return blinky;
        }
    }
}
