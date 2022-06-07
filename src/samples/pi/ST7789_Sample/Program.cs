using Meadow;
using System.Threading;

namespace PushButton_Sample
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
