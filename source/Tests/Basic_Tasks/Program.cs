using System;
using System.Threading;
using Meadow;

namespace Basic_Tasks
{
    class MainClass
    {
        static IApp app;

        public static void Main(string[] args)
        {
            app = new TasksApp();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
