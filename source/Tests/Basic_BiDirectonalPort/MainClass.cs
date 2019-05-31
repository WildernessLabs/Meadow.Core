using System;
using System.Threading;
using Meadow;

namespace Basic_BiDirectonalPort
{

    class MainClass
    {
        static IApp app;

        public static void Main(string[] args)
        {
            app = new BiDirectionalPortApp();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
