using System;
using System.Threading;
using Meadow;

namespace BasicI2CTest
{

    class MainClass
    {
        static IApp app;

        public static void Main(string[] args)
        {
            app = new I2CApp();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
