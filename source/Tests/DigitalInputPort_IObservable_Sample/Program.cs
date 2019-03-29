using System;
using System.Threading;
using Meadow;

namespace DigitalInputPort_IObservable_Sample
{
    class MainClass
    {
        static IApp app;

        public static void Main(string[] args)
        {
            app = new InputObservableApp();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
