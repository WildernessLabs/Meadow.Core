using System;
using Meadow;

namespace Basic_Threading
{
    class MainClass
    {
        static IApp app;

        public static void Main(string[] args)
        {
            app = new ThreadingApp();
        }
    }
}
