using System;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace Basic_BiDirectonalPort
{
    class BiDirectionalPortApp : App<F7Micro, BiDirectionalPortApp>
    {
        private IBiDirectionalPort _d04 = null;
        private IBiDirectionalPort _d05 = null;
        private IBiDirectionalPort _d06 = null;

        public BiDirectionalPortApp()
        {
            var name = this.GetType().Name;
            Console.WriteLine(name);
            Console.WriteLine(new string('-', name.Length));

            RunApp();
        }

        private void SetupIO()
        {
            Console.Write("Creating ports...");
            _d04 = Device.CreateBiDirectionalPort(Device.Pins.D04);
            _d05 = Device.CreateBiDirectionalPort(Device.Pins.D05);

            _d06 = Device.CreateBiDirectionalPort(
                Device.Pins.D06,
                resistorMode: ResistorMode.Disabled,
                initialDirection: PortDirectionType.Input, 
                interruptMode: InterruptMode.EdgeRising,
                glitchDuration: 10);

            _d06.Changed += OnD06Changed;

            Console.WriteLine("ok");
        }

        private async void OnD06Changed(object sender, DigitalInputPortEventArgs args)
        {
            Console.WriteLine("D06 Interrupt");
            Console.WriteLine("D06 -> high");
            _d06.State = true;
            await Task.Delay(1000);
            Console.WriteLine("D06 -> low");
            _d06.State = false;
            Console.WriteLine("D06 -> input");
            _d06.Direction = PortDirectionType.Input;
        }

        private void TeardownIO()
        {
            Console.Write("Disposing ports...");
            _d04.Dispose();
            _d04 = null;
            _d05.Dispose();
            _d05 = null;
            _d06.Dispose();
            _d06 = null;
            Console.WriteLine("ok");
        }

        public void RunApp()
        {
            var state = true;
            var count = 0;

            while (true)
            {
                if(_d04 == null)
                {
                    SetupIO();
                }

                Console.WriteLine($"D04 --> D05 {(state ? "high" : "low")}");
                // set output
                _d04.State = state;
                // read input
                var check = _d05.State;
                Console.WriteLine($"  D05 is {(check ? "high" : "low")}");

                state = !state;

                // now reverse
                Console.WriteLine($"D04 <-- D05 {(state ? "high" : "low")}");
                // set output
                _d05.State = state;
                // read input
                check = _d04.State;
                Console.WriteLine($"  D04 is {(check ? "high" : "low")}");

                state = !state;

                Thread.Sleep(2000);

                if(++count %10 == 0)
                {
                    // verifies Dispose is working
                    TeardownIO();
                }
            }

        }
    }
}
