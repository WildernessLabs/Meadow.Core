using System;
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
            Console.WriteLine("+PWMApp");

            var pwm = Device.CreatePwmPort(Device.Pins.D04, 100, 0.5f);

            DurationChecks(pwm);
        }

        private void FrequencyChecks(IPwmPort pwm)
        {
            var delta = 100;

            pwm.Start();
            while (true)
            {                
                Console.WriteLine($"Freq: {pwm.Frequency}  Period: {pwm.Period} {pwm.TimeScale}");
                Thread.Sleep(5000);

                pwm.Frequency += delta;
                if (pwm.Frequency <= 100 || pwm.Frequency >= 1000)
                {
                    delta *= -1;
                }
            }
        }

        private void DutyCycleChecks(IPwmPort pwm)
        {
            var delta = 0.10000f;

            pwm.Start();
            while (true)
            {
                Console.WriteLine($"Duty: {pwm.DutyCycle}  Duration: {pwm.Duration} {pwm.TimeScale}");
                Thread.Sleep(2000);

                var temp = Math.Round(pwm.DutyCycle + delta, 1);
                pwm.DutyCycle = (float)temp;

                if (pwm.DutyCycle <= .00 || pwm.DutyCycle >= 1.0)
                {
                    delta *= -1;
                }
            }
        }

        private void DurationChecks(IPwmPort pwm)
        {
            var delta = 1f;
            pwm.TimeScale = TimeScale.Milliseconds;

            pwm.Start();
            while (true)
            {
                Console.WriteLine($"Duty: {pwm.DutyCycle}  Duration: {pwm.Duration} {pwm.TimeScale}");
                Thread.Sleep(2000);

                var temp = Math.Round(pwm.Duration + delta, 0);
                pwm.Duration = (float)temp;

                if (pwm.Duration <= 000 || pwm.Duration >= 10.0)
                {
                    delta *= -1;
                }
            }
        }
    }
}
