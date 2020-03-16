using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow
{
    public class CircularBuffer<T> : IEnumerable<T>
    {
        public event EventHandler ItemAdded;

        private T[] m_list;
        private object m_syncRoot = new object();
        private int m_head = 0;
        private int m_tail = 0;
        private bool m_highwaterExceeded = false;
        private bool m_lowwaterExceeded = true;
        private int m_highWater;
        private int m_lowWater;
        private AutoResetEvent m_addedResetEvent;

        /// <summary>
        /// Fires when an element is added to the buffer when it is already full
        /// </summary>
        public event EventHandler Overrun;
        /// <summary>
        /// Fires when an attempt is made to remove an item from an empty buffer
        /// </summary>
        public event EventHandler Underrun;
        /// <summary>
        /// Fires when the number of elements reaches a non-zero HighWaterLevel value on an Enqueue call.  This event fires only once when passing upward across the boundary.
        /// </summary>
        public event EventHandler HighWater;
        /// <summary>
        /// Fires when the number of elements reaches a non-zero LowWaterLevel value on a Dequeue call.  This event fires only once when passing downward across the boundary.
        /// </summary>
        public event EventHandler LowWater;
        /// <summary>
        /// Gets the maximum number of elements the buffer can hold.
        /// </summary>
        public int MaxElements { get; private set; }
        /// <summary>
        /// When set to <b>true</b>, overrun conditions will throw an exception.  Default is <b>false</b>.
        /// </summary>
        public bool ExceptOnOverrun { get; set; }
        /// <summary>
        /// When set to <b>true</b>, underrun conditions will throw an exception.  Default is <b>false</b>.
        /// </summary>
        public bool ExceptOnUnderrun { get; set; }
        /// <summary>
        /// Returns true when an overrun condition has occurred.
        /// </summary>
        /// <remarks>
        /// The buffer will never reset this value except when Clear is called.  It is up to the consumer to set this back to false if desired.
        /// </remarks>
        public bool HasOverrun { get; set; }
        /// Returns true when an underrun condition has occurred.
        /// </summary>
        /// <remarks>
        /// The buffer will never reset this value except when Clear is called.  It is up to the consumer to set this back to false if desired.
        /// </remarks>
        public bool HasUnderrun { get; set; }
        /// <summary>
        /// Returns <b>true</b> if the buffer's Count equals its MaxEleemnts.
        /// </summary>
        public bool IsFull { get; private set; }

        public CircularBuffer(int maxElements)
        {
            m_addedResetEvent = new AutoResetEvent(false);
            MaxElements = maxElements;
            m_list = new T[MaxElements];
        }

        /// <summary>
        /// Empties all elements from the buffer
        /// </summary>
        public void Clear()
        {
            lock (m_syncRoot)
            {
                m_head = 0;
                m_tail = 0;
                m_highwaterExceeded = false;
                m_lowwaterExceeded = true;
                IsFull = false;
                HasOverrun = false;
                HasUnderrun = false;
            }
        }

        /// <summary>
        /// Gets the current count of elements in the buffer
        /// </summary>
        public int Count
        {
            get
            {
                lock (m_syncRoot)
                {
                    if (IsFull) return MaxElements;

                    if (m_head == m_tail) return 0;

                    if (m_head > m_tail)
                    {
                        return m_head - m_tail;
                    }

                    return MaxElements - m_tail + m_head;
                }
            }
        }

        /// <summary>
        /// The HighWater event will fire when the buffer contains this many (or more) elements.
        /// </summary>
        /// <remarks>
        /// Set the value to zero (default) to disable high-water notifications
        /// </remarks>
        public int HighWaterLevel
        {
            get { return m_highWater; }
            set
            {
                m_highWater = value;
            }
        }

        /// <summary>
        /// The LowWater event will fire when the buffer contains this many (or less) elements.
        /// </summary>
        /// <remarks>
        /// Set the value to zero (default) to disable low-water notifications
        /// </remarks>
        public int LowWaterLevel
        {
            get { return m_lowWater; }
            set
            {
                m_lowWater = value;
            }
        }

        private void IncrementTail()
        {
            m_tail++;
            if (m_tail >= MaxElements)
            {
                m_tail = 0;
            }
        }

        private void IncrementHead()
        {
            m_head++;
            if (m_head >= MaxElements)
            {
                m_head = 0;
            }

            if (m_head == m_tail)
            {
                IsFull = true;
            }
        }

        public void Enqueue(IEnumerable<T> items)
        {
            foreach (var i in items)
            {
                Enqueue(i);
            }
        }

        public void Enqueue(T[] items, int offset, int count)
        {
            for (int i = offset; i < offset + count; i++)
            {
                Enqueue(items[i]);
            }
        }

        /// <summary>
        /// Adds an element to the head of the buffer
        /// </summary>
        /// <param name="item"></param>
        /// <remarks>
        /// If the buffer is full and Enqueue is called, the new item will be successfully added to the buffer and the tail (oldest) item will be automatically removed
        /// </remarks>
        public void Enqueue(T item)
        {
            lock (m_syncRoot)
            {
                if (IsFull)
                {
                    // drop the tail item
                    IncrementTail();

                    // notify the consumer
                    OnOverrun();
                }

                // put the new item in the list
                m_list[m_head] = item;

                IncrementHead();

                if ((HighWaterLevel > 0) && (Count >= HighWaterLevel))
                {
                    if (!m_highwaterExceeded)
                    {
                        m_highwaterExceeded = true;
                        HighWater?.Invoke(this, EventArgs.Empty);
                    }
                }

                if ((LowWaterLevel > 0) && (Count > LowWaterLevel))
                {
                    m_lowwaterExceeded = false;
                }

                // do notifications
                m_addedResetEvent.Set();
                ItemAdded?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool EnqueueWaitOne(int millisecondsTimeout)
        {
            return m_addedResetEvent.WaitOne(millisecondsTimeout);
        }

        /// <summary>
        /// Removes the element from the tail of the buffer, if one exists
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            return GetOldest(true);
        }

        /// <summary>
        /// Returns the element currently at the head of the buffer, if one exists, without removing it
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return GetOldest(false);
        }

        private T GetOldest(bool remove)
        {
            lock (m_syncRoot)
            {
                if ((Count == 0) && !(IsFull))
                {
                    OnUnderrun();
                    return default(T);
                }

                T item = m_list[m_tail];

                if (remove)
                {
                    IncrementTail();
                    IsFull = false;

                    if ((HighWaterLevel > 0) && (Count < HighWaterLevel))
                    {
                        m_highwaterExceeded = false;
                    }

                    if ((LowWaterLevel > 0) && (Count <= LowWaterLevel))
                    {
                        if (!m_lowwaterExceeded)
                        {
                            m_lowwaterExceeded = true;
                            LowWater?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }

                return item;
            }
        }

        public virtual void OnOverrun()
        {
            HasOverrun = true;

            if (ExceptOnOverrun)
            {
                throw new BufferException("Overrun");
            }
            Overrun?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnUnderrun()
        {
            HasUnderrun = true;

            if (ExceptOnUnderrun)
            {
                throw new BufferException("Underrun");
            }
            Underrun?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Find the next element that matches the provided function criteria starting with the head item.
        /// </summary>
        /// <param name="findFunction"></param>
        /// <param name="defaultValue">The value to return if find function finds nothing</param>
        /// <returns></returns>
        public T Last(Func<T, bool> findFunction, T defaultValue = default(T))
        {
            lock (m_syncRoot)
            {
                int index = 0;
                if (m_head > 0)
                {
                    index = m_head - 1;
                }

                for (int i = 0; i < Count; i++)
                {
                    T item = m_list[index];
                    if (findFunction(item))
                    {
                        return item;
                    }
                    if (--index < 0) index = MaxElements - 1;
                }

                return defaultValue;
            }
        }

        /// <summary>
        /// Find the next element that matches the provided function criteria starting with the tail item.
        /// </summary>
        /// <param name="findFunction"></param>
        /// <param name="defaultValue">The value to return if find function finds nothing</param>
        /// <returns></returns>
        public T First(Func<T, bool> findFunction, T defaultValue = default(T))
        {
            lock (m_syncRoot)
            {
                int index = m_tail;

                for (int i = 0; i < Count; i++)
                {
                    T item = m_list[index];
                    if (findFunction(item))
                    {
                        return item;
                    }
                    if (++index >= MaxElements - 1) index = 0;
                }

                return defaultValue;
            }
        }

        /// <summary>
        /// Determine if the buffer contains a specified value
        /// </summary>
        /// <returns></returns>
        public bool Contains(T searchFor)
        {
            lock (m_syncRoot)
            {
                // we don't want to enumerate values outside of our "valid" range
                for (int i = 0; i < Count; i++)
                {
                    int index = m_tail + i;

                    if ((m_head <= m_tail) && (index >= MaxElements))
                    {
                        index -= MaxElements;
                    }

                    if (m_list[index].Equals(searchFor)) return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Dequeues the requested number of elements from the buffer
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <remarks>
        /// Similar to the Take() Linq method, if the buffer contains less items than requested, and empty array of items is returned and no items are dequeued
        /// </remarks>
        public T[] Dequeue(int count)
        {
            if (Count < count) return new T[] { };

            var result = new T[count];

            lock (m_syncRoot)
            {
                for (int i = 0; i < count; i++)
                {
                    result[i] = Dequeue();
                }
            }

            return result;
        }

        public T this[int index]
        {
            get
            {
                lock (m_syncRoot)
                {
                    int i = m_tail + index;

                    if ((m_head <= m_tail) && (i >= MaxElements))
                    {
                        i -= MaxElements;
                    }

                    return m_list[i];
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            // we don't want to enumerate values outside of our "valid" range
            for (int i = 0; i < Count; i++)
            {
                int index = m_tail + i;

                if ((m_head <= m_tail) && (index >= MaxElements))
                {
                    index -= MaxElements;
                }

                yield return m_list[index];
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class BufferException : Exception
    {
        public BufferException(string message)
            : base(message)
        {
        }
    }
}
