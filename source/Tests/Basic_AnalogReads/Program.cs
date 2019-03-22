using System;
using Meadow;

namespace Basic_AnalogReads
{
    class MainClass
    {
        static IApp app;

        public static void Main(string[] args)
        {
            app = new AnalogReadApp();
        }
    }
}
