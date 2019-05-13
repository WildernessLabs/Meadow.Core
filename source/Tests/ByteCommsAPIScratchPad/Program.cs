using System;
using System.Threading;
using Meadow;

namespace ByteCommsAPIScratchPad
{
    class MainClass
    {
        static IApp app; 

        public static void Main(string[] args)
        {
            app = new ByteCommsApp();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
