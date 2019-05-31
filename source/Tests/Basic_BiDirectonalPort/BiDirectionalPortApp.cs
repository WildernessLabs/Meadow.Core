using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace Basic_BiDirectonalPort
{
    class BiDirectionalPortApp : App<F7Micro, BiDirectionalPortApp>
    {
        private IBiDirectionalPort _d04;
        private IBiDirectionalPort _d05;

        public BiDirectionalPortApp()
        {
            var name = this.GetType().Name;
            Console.WriteLine(name);
            Console.WriteLine(new string('-', name.Length));

            SetupIO();
            RunApp();
        }

        public void SetupIO()
        {
            Console.Write("Creating ports...");
            _d04 = Device.CreateBiDirectionalPort(Device.Pins.D04);
            _d05 = Device.CreateBiDirectionalPort(Device.Pins.D05);
            Console.WriteLine("ok");
        }

        public void RunApp()
        {
            var state = true;

            while (true)
            {
                Console.WriteLine($"D04 --> D05 {(state ? "high" : "low")}");
                // set output
                _d04.State = state;
                // read input
                var check = _d05.State;
                Console.WriteLine($"  D05 is {(check ? "high" : "low")}");

                state = !state;

                // now reverse
                Console.Write($"D04 <-- D05 {(state ? "high" : "low")}");
                // set output
                _d05.State = state;
                // read input
                check = _d04.State;
                Console.Write($"  D04 is {(check ? "high" : "low")}");

                state = !state;

                Thread.Sleep(2000);
            }

        }
    }
}
