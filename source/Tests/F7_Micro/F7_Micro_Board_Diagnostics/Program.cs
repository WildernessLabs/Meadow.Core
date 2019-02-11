using System;
using System.Threading;
using Meadow;

namespace F7_Micro_Board_Diagnostics
{
    class MainClass
    {
        static IApp _app;

        public static void Main(string[] args)
        {
            // launch the app
            _app = new F7MicroDiagApp();

            // stay alive forever.
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
