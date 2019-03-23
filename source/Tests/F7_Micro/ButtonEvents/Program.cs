using System;
using System.Threading;
using Meadow;

namespace ButtonEvents
{
    class MainClass
    {
        static IApp app;

        public static void Main(string[] args)
        {
            app = new ButtonEventsApp();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
