using Meadow;
using System;
using Xunit;

namespace Core.Unit.Tests
{
    public class CircularBufferTests
    {
        [Fact]
        public void TestMoveItemsToHeadAfterTail()
        {
            var rnd = new Random();
            var size = rnd.Next(20, 50);
            var buffer = new CircularBuffer<byte>(size);

            // add some data
            var less = rnd.Next(0, 5);
            for (byte i = 0; i < size - less; i++)
            {
                buffer.Append(i);
            }
            var totalBytes = size - less;
            Assert.Equal(totalBytes, buffer.Count);

            // pull a couple so we're not copying right from 0
            var remove = rnd.Next(5);
            for (int i = 0; i < remove; i++)
            {
                var r = buffer.Remove();
                totalBytes--;
                Assert.Equal(i, r);
                Assert.Equal(totalBytes, buffer.Count);
            }

            // now do a bulk move
            var bytes = new byte[10];
            var moved = buffer.MoveItemsTo(bytes, 0, 10);
            Assert.Equal(10, moved);
            totalBytes -= moved;
            Assert.Equal(totalBytes, buffer.Count);

            // the next item in the buffer should be one past the 10 we just removed
            var b = buffer.Remove();
            Assert.Equal(10 + remove, b);
            totalBytes--;
            Assert.Equal(totalBytes, buffer.Count);

        }

        [Fact]
        public void TestMoveItemsToHeadBeforeTail()
        {
            var rnd = new Random();
            var buffer = new CircularBuffer<byte>(20);

            // add some data and remove it to get the buffer pointer advanced;
            for (byte i = 0; i < 30; i++)
            {
                buffer.Append(i);
                if (i > 0 && i < 15)
                {
                    var r = buffer.Remove();
                    Assert.Equal(i - 1, r);
                }
            }

            // now do a bulk move
            var bytes = new byte[10];
            var moved = buffer.MoveItemsTo(bytes, 0, 10);
            Assert.Equal(10, moved);
            var b = buffer.Remove();
            Assert.Equal(24, moved);
        }
    }
}
