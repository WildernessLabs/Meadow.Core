using System;
using System.Threading;
using System.Threading.Tasks;
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

            try
            {
                PwmWithGpio();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }


        private void PwmWithGpio()
        {
            var pwm04 = Device.CreatePwmPort(Device.Pins.D04, 2, 0.5f);

            var d03 = Device.CreateDigitalOutputPort(Device.Pins.D03);

            Task.Run(async () =>
            {
                var c = 0;

                while (true)
                {
                    d03.State = !d03.State;
                    await Task.Delay(1000);
                }
            });

            Thread.Sleep(5000);

            pwm04.Start();
        }

        private void MultiplePwms()
        {
            var f = 100;
            Console.WriteLine($"DeviceCreatePwmPort {f} Hz");

            var pwmA = Device.CreatePwmPort(Device.Pins.D11, f, 0.5f);
            var pwmB = Device.CreatePwmPort(Device.Pins.D12, 200, 0.5f);
            var pwmC = Device.CreatePwmPort(Device.Pins.D13, 400, 0.25f);

            pwmA.Start();
            pwmB.Start();
            pwmC.Start();
        }

        private void TimeScaleChecks(IPwmPort pwm)
        {
            var delta = 100;

            pwm.Frequency = 50f;

            pwm.Start();
            while (true)
            {
                pwm.TimeScale = TimeScale.Seconds;
                pwm.Period = 0.02f;
                Console.WriteLine($"Freq: {(int)pwm.Frequency}  Period: {(int)pwm.Period} {pwm.TimeScale}");
                Thread.Sleep(2000);

                pwm.TimeScale = TimeScale.Milliseconds;
                Console.WriteLine($"Freq: {(int)pwm.Frequency}  Period: {(int)pwm.Period} {pwm.TimeScale}");
                Thread.Sleep(2000);
                pwm.Period = 50f;
                Console.WriteLine($"Freq: {(int)pwm.Frequency}  Period: {(int)pwm.Period} {pwm.TimeScale}");
                Thread.Sleep(2000);

                pwm.TimeScale = TimeScale.Microseconds;
                Console.WriteLine($"Freq: {(int)pwm.Frequency}  Period: {(int)pwm.Period} {pwm.TimeScale}");
                pwm.Period = 80f;
                Console.WriteLine($"Freq: {(int)pwm.Frequency}  Period: {(int)pwm.Period} {pwm.TimeScale}");
                Thread.Sleep(2000);
            }
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
