using System;
using System.Threading;
using Meadow.Core;

namespace SPITest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            SPITestApplication application = new SPITestApplication();
            application.Run();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
