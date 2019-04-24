using System.Threading;
using Meadow;
using Meadow.Devices;

namespace Basic_PWM
{
    class PWMApp : AppBase<F7Micro, PWMApp>
    {
        public PWMApp()
        {
            var pwm = Device.CreatePwmPort(Device.Pins.D02);
            var heartbeat = Device.CreateDigitalOutputPort(Device.Pins.D04);

            while(true)
            {
                heartbeat.State = !heartbeat.State;
                Thread.Sleep(1000);
            }
        }
    }
}
