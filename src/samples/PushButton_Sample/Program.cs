using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using System.Threading;

namespace DigitalIOSample
{
    class Program
    {
        static IApp app;

        public static void Main(string[] args)
        {
            app = new MeadowApp();

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
