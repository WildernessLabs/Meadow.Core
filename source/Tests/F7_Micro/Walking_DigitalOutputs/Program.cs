using System;
using System.Threading;
using Meadow;

namespace Walking_DigitalOutputs
{
    class MainClass
    {
        static IApp app;

        public static void Main(string[] args)
        {
            app = new OutputApp();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
