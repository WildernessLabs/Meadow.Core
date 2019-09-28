using Meadow;
using System.Threading;

namespace SerialEcho
{
    class Program
    {
        static IApp app;

        public static void Main(string[] args)
        {
            app = new SerialEcho();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
