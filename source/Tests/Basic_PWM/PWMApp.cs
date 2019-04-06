using Meadow;
using Meadow.Devices;

namespace Basic_PWM
{
    class PWMApp : AppBase<F7Micro, PWMApp>
    {
        public PWMApp()
        {
            var pwm = Device.CreatePwmPort(Device.Pins.D02);
        }
    }
}
