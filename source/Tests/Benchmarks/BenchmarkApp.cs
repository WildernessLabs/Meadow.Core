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
        }

        protected void RunIntegerListTests()
        {
            int total = 0;

            DateTime start = DateTime.Now;

            // initialize our list
            List<int> intList = new List<int>();
            DateTime listInit = DateTime.Now;

            // add some numbers to it
            for (int i = 0; i < 2000; i++) {
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
            Console.WriteLine($"Integer List Test Results");
            Console.WriteLine($"Time to initialize: {timeToInit.Milliseconds}ms");
            Console.WriteLine($"Time to populate: {timeToPopulate.Milliseconds}ms");
            Console.WriteLine($"Time to sum: {timeToSum.Milliseconds}ms");
            Console.WriteLine($"Time to clear: {timeToClear.Milliseconds}ms");
            Console.WriteLine("=======================================");
        }
    }
}
