using System;
using System.Collections.Generic;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace Benchmarks
{
    public class BenchmarkApp : App<F7Micro, BenchmarkApp>
    {
        public BenchmarkApp()
        {
            Console.WriteLine("App Up");
            RunIntegerListTests();
            RunDigitalOutputTests();
        }

        protected void RunIntegerListTests()
        {
            int total = 0;
            int listCount = 1000;

            DateTime start = DateTime.Now;

            // initialize our list
            List<int> intList = new List<int>();
            DateTime listInit = DateTime.Now;

            // add some numbers to it
            for (int i = 0; i < listCount; i++) {
                intList.Add(i);
            }
            DateTime listPopulated = DateTime.Now;

            // add them up
            foreach (int n in intList) {
                total += n;
            }
            DateTime listSummed = DateTime.Now;

            // remove them
            for (int i = intList.Count; i >= 0; i--) {
                intList.Remove(i);
            }
            DateTime listCleared = DateTime.Now;

            // calculate times.
            TimeSpan timeToInit = listInit - start;
            TimeSpan timeToPopulate = listPopulated - listInit;
            TimeSpan timeToSum = listSummed - listPopulated;
            TimeSpan timeToClear = listCleared - listSummed;

            // output
            Console.WriteLine("=======================================");
            Console.WriteLine($"Integer List Test Results, int count: {listCount}:");
            Console.WriteLine($"Time to initialize: {timeToInit.TotalMilliseconds}ms");
            Console.WriteLine($"Time to populate: {timeToPopulate.TotalMilliseconds}ms");
            Console.WriteLine($"Time to sum: {timeToSum.TotalMilliseconds}ms");
            Console.WriteLine($"Time to clear: {timeToClear.TotalMilliseconds}ms");
            Console.WriteLine("=======================================");
        }

        protected void RunDigitalOutputTests()
        {
            DateTime start = DateTime.Now;
            bool state = false;
            int writeLoopCount = 100;

            // init some ports
            IDigitalOutputPort red = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedRed);
            IDigitalOutputPort green = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedGreen);
            IDigitalOutputPort blue = Device.CreateDigitalOutputPort(Device.Pins.OnboardLedBlue);
            DateTime portsCreated = DateTime.Now;

            // write to the ports
            for (int i = 0; i < writeLoopCount; i++) {
                state = !state;
                red.State = state;
                green.State = state;
                blue.State = state;
            }
            DateTime portsWritten = DateTime.Now;

            // calculate times.
            TimeSpan timeToInit = portsCreated - start;
            TimeSpan timeToWrite = portsWritten - portsCreated;
            float averageWriteTime = (float)timeToWrite.TotalMilliseconds / (float)(writeLoopCount * 3);

            // output
            Console.WriteLine("=======================================");
            Console.WriteLine($"Port Test Results:");
            Console.WriteLine($"Time to initialize: {timeToInit.TotalMilliseconds}ms");
            Console.WriteLine($"Time to write: {timeToWrite.TotalMilliseconds}ms");
            Console.WriteLine($"Average time per write: {averageWriteTime}ms");
            Console.WriteLine("=======================================");

        }
    }
}
