using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace Basic_PWM
{
    class PWMApp : App<F7Micro, PWMApp>
    {
        public PWMApp()
        {
            var pwm = Device.CreatePwmPort(Device.Pins.D05, 50, 0x8000);
            var heartbeat = Device.CreateDigitalOutputPort(Device.Pins.D04);

            ((PwmPort)pwm).Setup();
            pwm.Start();

            while(true)
            {
                heartbeat.State = !heartbeat.State;
                Thread.Sleep(1000);
            }
        }
    }
}
