using System;
using System.Threading;
using Meadow;

namespace BLETest
{
    class MainClass
    {
        static BLEApp app;

        public static void Main(string[] args)
        {
            app = new BLEApp();
            BLEApp.Device.
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
