using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Diagnostics;
using System.Threading;

namespace Profiler
{
    class ProfilerApp : App<F7Micro, ProfilerApp>
    {
        const uint GPIOB_STATE_ADDR = 0x40020414;

        private IDigitalOutputPort _d04;

        public void Run()
        {
            // create the port to have core initialize all of the non-state stuff
            _d04 = Device.CreateDigitalOutputPort(Device.Pins.D04);

            //DirectRegisterHighLow();
            //WriteUseCoreObject();
            ReadUseCoreObject();
            //MethodCall();
            //ObjectCreation();
        }

        public unsafe void DirectRegisterHighLow()
        {
            // RESULTS - time between transitions (or time spent in one state)
            // beta 3.5     1.12us [Debug]

            Console.WriteLine($"Test 1");

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

            Console.WriteLine($"Test 2w");

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

            Console.WriteLine($"Test 2r");
            var iterations = 10000;

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
            // beta 3.5     0.020ms [Debug]
            // beta 3.5     0.021ms [Release]

            Console.WriteLine($"Test 3");

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

            Console.WriteLine($"Test 4");

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
    }

    class TestObject
    {
        public int IntProp { get; set; } = 1;

        public TestObject()
        { 
        }
    }
}
