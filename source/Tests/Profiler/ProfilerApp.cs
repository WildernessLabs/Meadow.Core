using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Profiler
{
    class ProfilerApp : App<F7Micro, ProfilerApp>
    {
        const uint GPIOB_STATE_ADDR = 0x40020414;

        private IDigitalOutputPort _d04;

        public void Run()
        {
            //DirectRegisterHighLow();
            //WriteUseCoreObject();
            //ReadUseCoreObject();
            MethodCall();
            //ObjectCreation();
            //InterruptToOutput();
        }

        public unsafe void DirectRegisterHighLow()
        {
            // RESULTS - time between transitions (or time spent in one state)
            // beta 3.5     1.12us [Debug]
            // beta 3.7     0.27us

            Console.WriteLine($"Test 1");

            // create the port to have core initialize all of the non-state stuff
            _d04 = Device.CreateDigitalOutputPort(Device.Pins.D04);

            while (true)
            {
                *(uint*)GPIOB_STATE_ADDR = 0xffffffff;
                *(uint*)GPIOB_STATE_ADDR = 0;
            }
        }

        public void WriteUseCoreObject()
        {
            // RESULTS - time between transitions (or time spent in one state)
            // beta 3.5     15.6ms [Debug]
            // beta 3.5     15.5ms [Release]
            // beta 3.6      4.1ms [Debug]
            // beta 3.7      0.43ms

            Console.WriteLine($"Test 2w");

            // create the port to have core initialize all of the non-state stuff
            _d04 = Device.CreateDigitalOutputPort(Device.Pins.D04);

            while (true)
            {
                _d04.State = true;
                _d04.State = false;
            }
        }

        public unsafe void ReadUseCoreObject()
        {
            // RESULTS - time between transitions (or time spent in one state)
            // beta 3.5     10000 took 930ms [Debug]
            // beta 3.6     10000 took 900ms [Debug]
            // beta 3.7     10000 took 31ms [Debug]
            Console.WriteLine($"Test 2r");
            var iterations = 10000;

            // create the port to have core initialize all of the non-state stuff
            _d04 = Device.CreateDigitalOutputPort(Device.Pins.D04);

            var state = false;
            while (true)
            {
                *(uint*)GPIOB_STATE_ADDR = 0xffffffff;
                
                for (var i = 0; i < iterations; i++)
                {
                    state |= _d04.State;
                }

                *(uint*)GPIOB_STATE_ADDR = 0;
                Thread.Sleep(1000);
                Console.WriteLine($"Using state to prevent optimization...{state}");
            }
        }

        public void MethodCall()
        {
            // RESULTS - time between transitions (or time spent in one state)
            // beta 3.5     20us [Debug]
            // beta 3.5     21us [Release]
            // beta 3.7     0.27us
            Console.WriteLine($"Test 3");

            // create the port to have core initialize all of the non-state stuff
            _d04 = Device.CreateDigitalOutputPort(Device.Pins.D04);

            while (true)
            {
                High();
                Low();
            }
        }

        public unsafe void High()
        {
            *(uint*)0x40020414 = 0xffffffff;
        }

        public unsafe void Low()
        {
            *(uint*)0x40020414 = 0;
        }

        public unsafe void ObjectCreation()
        {
            // RESULTS - time between transitions (or time spent in one state)
            // beta 3.5     0.066ms [Debug]
            // beta 3.7     11.5us
            Console.WriteLine($"Test 4");

            // create the port to have core initialize all of the non-state stuff
            _d04 = Device.CreateDigitalOutputPort(Device.Pins.D04);

            while (true)
            {
                // RESULTS - time between transitions (or time spent in one state)
                // beta 3.5     ??

                *(uint*)GPIOB_STATE_ADDR = 0xffffffff;
                var a = new TestObject();
                *(uint*)GPIOB_STATE_ADDR = 0;
                var b = new TestObject();
            }
        }

        public void InterruptToOutput()
        {
            // this test uses two pins.
            // D03 is an input from a button
            // D05 is an output
            // On a rising interrupt on D03, D05 is set high for a period.
            // This allows using a scope to see the time from the input going high to the output going high
            // so measuring the delta from the rising edge on D03 to the rising edge on D05

            // RESULTS - time between transitions (or time spent in one state)
            // beta 3.6     ~50ms
            // beta 3.7     ~3.5ms

            var input = Device.CreateDigitalInputPort(
                Device.Pins.D03, 
                InterruptMode.EdgeRising);

            var output = Device.CreateDigitalOutputPort(
                Device.Pins.D05, false);

            Console.WriteLine($"Test 5");

            var count = 0;

            input.Changed += async (s, o) =>
            {
                output.State = true;
                count++;
                await Task.Delay(1000);
                output.State = false;
                Console.WriteLine($"click {count}");
            };


        }
    }

    class TestObject
    {
        public int IntProp { get; set; } = 1;

        public TestObject()
        { 
        }
    }
}
