using System;
using System.Linq;
using System.Threading;
using Meadow;
using Meadow.Devices;

namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        string foo1 = "this is test.\r\n";
        string foo2 = "this is another test. no matching token.";
        string foo3 = "this is a test.\r\n with two matching tokens.\r\n";

        byte[] goo1 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        CircularBuffer<byte> buffer = new CircularBuffer<byte>(100);


        public MeadowApp()
        {
            Console.WriteLine("up");

            // fill the buffer
            foreach (var b in goo1) {
                buffer.Append(b);
                //buffer.Enqueue(b);
            }

            Console.WriteLine($"Buffer.Count():{buffer.Count()}");

            byte[] searchPattern = new byte[] { 3, 4, 5 };

            if (buffer.Contains(searchPattern)) {
                Console.WriteLine("found pattern");
            } else {
                Console.WriteLine("did not find pattern");
            }

            var firstIndexOf = buffer.FirstIndexOf(searchPattern);
            Console.WriteLine($"firstIndexOf: {firstIndexOf}");
           
        }

        
    }
}