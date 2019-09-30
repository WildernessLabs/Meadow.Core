using System;
using System.Threading;
using Meadow;
using Meadow.Devices;

namespace Basic_PWM
{
    class PWMApp : App<F7Micro, PWMApp>
    {
        public PWMApp()
        {
            Console.WriteLine("+PWMApp");

            var pwm = Device.CreatePwmPort(Device.Pins.OnboardLedBlue, 100, 0.5f);
            pwm.Start();
            var heartbeat = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedGreen);

            while(true)
            {
                heartbeat.State = !heartbeat.State;
                Thread.Sleep(1000);
                Console.WriteLine("tick");
            }
        }
    }
}
