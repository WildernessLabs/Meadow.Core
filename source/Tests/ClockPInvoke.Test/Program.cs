using System;
using Meadow.Core.Interop;

namespace ClockPInvoke.Test
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Program Started (console)!");

            while (true)
            {
                Console.WriteLine("Before P/Invoke");

                Interop.Nuttx.timespec ts = new Interop.Nuttx.timespec();
                var time = Interop.Nuttx.clock_gettime(Interop.Nuttx.clockid_t.CLOCK_REALTIME, ref ts);

                Console.WriteLine("Result: " + ts.tv_sec.ToString());

                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
