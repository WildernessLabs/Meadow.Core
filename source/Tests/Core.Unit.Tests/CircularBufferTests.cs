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
        public void TestMoveItemsToFullBufferStart0()
        {
            var bufferSize = 30;
            var moveSize = 10;

            var buffer = new CircularBuffer<byte>(bufferSize);

            // add some data
            for (byte i = 0; i < bufferSize; i++)
            {
                buffer.Append(i);
            }
            Assert.Equal(bufferSize, buffer.Count);

            // now do a bulk move
            var bytes = new byte[moveSize];
            var moved = buffer.MoveItemsTo(bytes, 0, bytes.Length);
            Assert.Equal(bytes.Length, moved);

            // make sure the buffer shrunk
            Assert.Equal(bufferSize - moveSize, buffer.Count);

            // check the contents of the destination
            for (byte i = 0; i < moveSize; i++)
            {
                Assert.Equal(i, bytes[i]);
            }

            // check the contents of the source
            for (byte i = (byte)moveSize; i < bufferSize; i++)
            {
                Assert.Equal(i, buffer.Remove());
            }
        }

        [Fact]
        public void TestMoveItemsToFullBufferStartNon0NoWrap()
        {
            var bufferSize = 30;
            var moveSize = 10;

            var buffer = new CircularBuffer<byte>(bufferSize);

            // add and remove to push the head and tail off of zero
            buffer.Append(255);
            buffer.Remove();

            // add some data
            for (byte i = 0; i < bufferSize; i++)
            {
                buffer.Append(i);
            }
            Assert.Equal(bufferSize, buffer.Count);

            // now do a bulk move
            var bytes = new byte[moveSize];
            var moved = buffer.MoveItemsTo(bytes, 0, bytes.Length);
            Assert.Equal(bytes.Length, moved);

            // make sure the buffer shrunk
            Assert.Equal(bufferSize - moveSize, buffer.Count);

            // check the contents of the destination
            for (byte i = 0; i < moveSize; i++)
            {
                Assert.Equal(i, bytes[i]);
            }

            // check the contents of the source
            for (byte i = (byte)moveSize; i < bufferSize; i++)
            {
                Assert.Equal(i, buffer.Remove());
            }
        }

        [Fact]
        public void TestMoveItemsToFullBufferStartNon0WithWrap()
        {
            var bufferSize = 30;
            var moveSize = 10;

            var buffer = new CircularBuffer<byte>(bufferSize);

            // add and remove to push the head and tail off of zero and far enough for the data pull to wrap
            for (int i = 0; i < 25; i++)
            {
                buffer.Append(255);
                buffer.Remove();
            }

            // add some data
            for (byte i = 0; i < bufferSize; i++)
            {
                buffer.Append(i);
            }
            Assert.Equal(bufferSize, buffer.Count);

            // now do a bulk move
            var bytes = new byte[moveSize];
            var moved = buffer.MoveItemsTo(bytes, 0, bytes.Length);
            Assert.Equal(bytes.Length, moved);

            // make sure the buffer shrunk
            Assert.Equal(bufferSize - moveSize, buffer.Count);

            // check the contents of the destination
            for (byte i = 0; i < moveSize; i++)
            {
                Assert.Equal(i, bytes[i]);
            }

            // check the contents of the source
            for (byte i = (byte)moveSize; i < bufferSize; i++)
            {
                Assert.Equal(i, buffer.Remove());
            }
        }

        [Fact]
        public void TestMoveItemsToPartialBuffer()
        {
            var bufferSize = 30;
            var fillSize = 25;
            var moveSize = 10;

            var buffer = new CircularBuffer<byte>(bufferSize);

            // add some data - not full
            for (byte i = 0; i < fillSize; i++)
            {
                buffer.Append(i);
            }
            Assert.Equal(fillSize, buffer.Count);

            // now do a bulk move
            var bytes = new byte[moveSize];
            var moved = buffer.MoveItemsTo(bytes, 0, bytes.Length);
            Assert.Equal(bytes.Length, moved);

            // make sure the buffer shrunk
            Assert.Equal(fillSize - moveSize, buffer.Count);

            // check the contents of the destination
            for (byte i = 0; i < moveSize; i++)
            {
                Assert.Equal(i, bytes[i]);
            }

            // check the contents of the source
            for (byte i = (byte)moveSize; i < fillSize; i++)
            {
                Assert.Equal(i, buffer.Remove());
            }
        }

        [Fact]
        public void TestMoveItemsToHeadBeforeTail()
        {
            var bufferSize = 20;

            var buffer = new CircularBuffer<byte>(bufferSize);

            // add some data and remove it to get the buffer pointer advanced;
            // we're adding more (3) than the buffer size (20), so this forces a wrap
            for (byte i = 0; i < 30; i++)
            {
                buffer.Append(i);
                if (i > 0 && i < 15)
                {
                    // remove some to prevent overflow and to move the tail
                    var r = buffer.Remove();
                    Assert.Equal(i - 1, r);
                }
            }

            Assert.Equal(16, buffer.Count);

            // now do a bulk move
            var bytes = new byte[10];
            var moved = buffer.MoveItemsTo(bytes, 0, bytes.Length);
            Assert.Equal(bytes.Length, moved);
            Assert.Equal(6, buffer.Count);

            var b = buffer.Remove();
            Assert.Equal(5, buffer.Count);
        }
    }
}
