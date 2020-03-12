using System;
using System.Threading;

namespace Basic_PWM
{
class MainClass
    {
        static PWMApp _app;

        static void Main(string[] args)
        {
            // instantiate and run new meadow app
            _app = new PWMApp();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
