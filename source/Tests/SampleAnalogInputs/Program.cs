using System;
using System.Threading;
using Meadow;

namespace SampleAnalogInputs
{
    class MainClass
    {
        static IApp app;

        public static void Main(string[] args)
        {
            app = new ADCApp();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
