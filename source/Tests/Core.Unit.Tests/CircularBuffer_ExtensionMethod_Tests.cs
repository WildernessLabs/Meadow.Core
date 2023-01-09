using System;
using System.Linq;
using Meadow;
using Xunit;

namespace Core.Unit.Tests
{
    public class ExtensionMethod_FirstIndexOf_Tests
    {
        [Fact]
        public void TestContains()
        {
            // TODO: Move this stuff into a shared setup method

            byte[] goo1 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            CircularBuffer<byte> buffer = new CircularBuffer<byte>(100);

            // fill the buffer
            foreach (var b in goo1) {
                buffer.Append(b);
            }

            //Resolver.Log.Info($"Buffer.Count():{buffer.Count()}");

            byte[] searchPattern = new byte[] { 3, 4, 5 };

            Assert.True(buffer.Contains(searchPattern));
        }

        [Fact]
        public void TestFirstIndexOf()
        {
            byte[] goo1 = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            CircularBuffer<byte> buffer = new CircularBuffer<byte>(100);
            // fill the buffer
            foreach (var b in goo1) {
                buffer.Append(b);
            }


            byte[] searchPattern = new byte[] { 3, 4, 5 };


            Assert.True(buffer.FirstIndexOf(searchPattern) > 0);

        }
    }
}
