using System;
using System.Threading;
using Meadow;

namespace Benchmarks
{
    class MainClass
    {
        static IApp app;

        public static void Main(string[] args)
        {
            app = new I2CDiscoverApp();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
