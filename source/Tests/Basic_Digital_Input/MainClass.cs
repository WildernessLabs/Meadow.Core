using System;
using Meadow;

namespace Basic_Digital_Input
{
    class MainClass
    {
        static IApp _app;

        public static void Main(string[] args)
        {
            // instantiate and run new meadow app
            _app = new InputApp();
        }
    }
}
