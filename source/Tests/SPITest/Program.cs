using System;
using Meadow.Core;

namespace SPITest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Testing Software SPI Communications");
            Console.WriteLine();

            SPITestApplication application = new SPITestApplication();
            application.Run();
        }
    }
}
