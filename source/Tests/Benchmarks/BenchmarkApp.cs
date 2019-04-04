using System;
using System.Collections.Generic;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace Benchmarks
{
    public class BenchmarkApp : AppBase<F7Micro, BenchmarkApp>
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

            DateTime start = DateTime.Now;

            // initialize our list
            List<int> intList = new List<int>();
            DateTime listInit = DateTime.Now;

            // add some numbers to it
            for (int i = 0; i < 1000; i++) {
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
            Console.WriteLine($"Integer List Test Results, int count: {intList.Count}:");
            Console.WriteLine($"Time to initialize: {timeToInit.Milliseconds}ms");
            Console.WriteLine($"Time to populate: {timeToPopulate.Milliseconds}ms");
            Console.WriteLine($"Time to sum: {timeToSum.Milliseconds}ms");
            Console.WriteLine($"Time to clear: {timeToClear.Milliseconds}ms");
            Console.WriteLine("=======================================");
        }

        protected void RunDigitalOutputTests()
        {
            DateTime start = DateTime.Now;
            bool state = false;

            // init some ports
            IDigitalOutputPort red = Device.CreateDigitalOutputPort(Device.Pins.OnboardLEDRed);
            IDigitalOutputPort green = Device.CreateDigitalOutputPort(Device.Pins.OnboardLEDGreen);
            IDigitalOutputPort blue = Device.CreateDigitalOutputPort(Device.Pins.OnboardLEDBlue);
            DateTime portsCreated = DateTime.Now;

            // write to the ports
            for (int i = 0; i < 100; i++) {
                state = !state;
                red.State = state;
                green.State = state;
                blue.State = state;
            }
            DateTime portsWritten = DateTime.Now;

            // calculate times.
            TimeSpan timeToInit = portsCreated - start;
            TimeSpan timeToWrite = portsWritten - portsCreated;

            // output
            Console.WriteLine("=======================================");
            Console.WriteLine($"Port Test Results:");
            Console.WriteLine($"Time to initialize: {timeToInit.Milliseconds}ms");
            Console.WriteLine($"Time to write: {timeToWrite.Milliseconds}ms");
            Console.WriteLine("=======================================");

        }
    }
}
