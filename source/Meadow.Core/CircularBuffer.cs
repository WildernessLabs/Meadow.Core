using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularBuffer<T> : IEnumerable<T>
    {
        public event EventHandler ItemAdded;

        // TODO: this should probably be Span<T>
        private T[] _list;
        private object _syncRoot = new object();
        private int _head = 0;
        private int _tail = 0;
        private bool _highwaterExceeded = false;
        private bool _lowwaterExceeded = true;
        private int _highWater;
        private int _lowWater;
        private AutoResetEvent _addedResetEvent;

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
        /// Fires when the number of elements reaches a non-zero LowWaterLevel value on a Remove call.  This event fires only once when passing downward across the boundary.
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
            _addedResetEvent = new AutoResetEvent(false);
            MaxElements = maxElements;
            _list = new T[MaxElements];
        }

        /// <summary>
        /// Empties all elements from the buffer
        /// </summary>
        public void Clear()
        {
            lock (_syncRoot) {
                _head = 0;
                _tail = 0;
                _highwaterExceeded = false;
                _lowwaterExceeded = true;
                IsFull = false;
                HasOverrun = false;
                HasUnderrun = false;
            }
        }

        /// <summary>
        /// Gets the current count of elements in the buffer
        /// </summary>
        public int Count {
            get {
                lock (_syncRoot) {
                    if (IsFull) return MaxElements;

                    if (_head == _tail) return 0;

                    if (_head > _tail) {
                        return _head - _tail;
                    }

                    return MaxElements - _tail + _head;
                }
            }
        }

        /// <summary>
        /// The HighWater event will fire when the buffer contains this many (or more) elements.
        /// </summary>
        /// <remarks>
        /// Set the value to zero (default) to disable high-water notifications
        /// </remarks>
        public int HighWaterLevel {
            get { return _highWater; }
            set {
                _highWater = value;
            }
        }

        /// <summary>
        /// The LowWater event will fire when the buffer contains this many (or less) elements.
        /// </summary>
        /// <remarks>
        /// Set the value to zero (default) to disable low-water notifications
        /// </remarks>
        public int LowWaterLevel {
            get { return _lowWater; }
            set {
                _lowWater = value;
            }
        }

        private void IncrementTail()
        {
            _tail++;
            if (_tail >= MaxElements) {
                _tail = 0;
            }
        }

        private void IncrementHead()
        {
            _head++;
            if (_head >= MaxElements) {
                _head = 0;
            }

            if (_head == _tail) {
                IsFull = true;
            }
        }

        public void Append(IEnumerable<T> items)
        {
            foreach (var i in items) {
                Append(i);
            }
        }

        public void Append(T[] items, int offset, int count)
        {
            for (int i = offset; i < offset + count; i++) {
                Append(items[i]);
            }
        }

        // TODO: not sure why i can't do this. but LINQ adds Append<T> methods.
        //public void Append<T>(T element)
        //{
        //    this.Enqueue(element);
        //}

        //public void Append(T element)
        //{
        //    this.Enqueue(element);
        //}

        //public void Append(IEnumerable<T> items)
        //{
        //    this.Enqueue(items);
        //}

        /// <summary>
        /// Adds an element to the head of the buffer
        /// </summary>
        /// <param name="item"></param>
        /// <remarks>
        /// If the buffer is full and Enqueue is called, the new item will be successfully added to the buffer and the tail (oldest) item will be automatically removed
        /// </remarks>
        public void Append(T item)
        {
            lock (_syncRoot) {
                if (IsFull) {
                    // drop the tail item
                    IncrementTail();

                    // notify the consumer
                    OnOverrun();
                }

                // put the new item in the list
                _list[_head] = item;

                IncrementHead();

                if ((HighWaterLevel > 0) && (Count >= HighWaterLevel)) {
                    if (!_highwaterExceeded) {
                        _highwaterExceeded = true;
                        HighWater?.Invoke(this, EventArgs.Empty);
                    }
                }

                if ((LowWaterLevel > 0) && (Count > LowWaterLevel)) {
                    _lowwaterExceeded = false;
                }

                // do notifications
                _addedResetEvent.Set();
                ItemAdded?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool AppendWaitOne(int millisecondsTimeout)
        {
            return _addedResetEvent.WaitOne(millisecondsTimeout);
        }

        /// <summary>
        /// Removes the element from the tail of the buffer, if one exists
        /// </summary>
        /// <returns></returns>
        public T Remove()
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
            lock (_syncRoot) {
                if ((Count == 0) && !(IsFull)) {
                    OnUnderrun();
                    return default(T);
                }

                T item = _list[_tail];

                if (remove) {
                    IncrementTail();
                    IsFull = false;

                    if ((HighWaterLevel > 0) && (Count < HighWaterLevel)) {
                        _highwaterExceeded = false;
                    }

                    if ((LowWaterLevel > 0) && (Count <= LowWaterLevel)) {
                        if (!_lowwaterExceeded) {
                            _lowwaterExceeded = true;
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

            if (ExceptOnOverrun) {
                throw new BufferException("Overrun");
            }
            Overrun?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnUnderrun()
        {
            HasUnderrun = true;

            if (ExceptOnUnderrun) {
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
            lock (_syncRoot) {
                int index = 0;
                if (_head > 0) {
                    index = _head - 1;
                }

                for (int i = 0; i < Count; i++) {
                    T item = _list[index];
                    if (findFunction(item)) {
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
            lock (_syncRoot) {
                int index = _tail;

                for (int i = 0; i < Count; i++) {
                    T item = _list[index];
                    if (findFunction(item)) {
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
            lock (_syncRoot) {
                // we don't want to enumerate values outside of our "valid" range
                for (int i = 0; i < Count; i++) {
                    int index = _tail + i;

                    if ((_head <= _tail) && (index >= MaxElements)) {
                        index -= MaxElements;
                    }

                    if (_list[index].Equals(searchFor)) return true;
                }

                return false;
            }
        }

        //public bool Contains(T[] pattern)
        //{
        //    int patternLength = pattern.Length;
        //    //int totalLength = Count;
        //    T firstMatch = pattern[0];

        //    for (int i = 0; i < Count; i++) {
        //        // calculate the index from the head
        //        int index = _tail = i;
        //        if ((_head <= _tail) && (index >= MaxElements)) {
        //            index -= MaxElements;
        //        }

        //        if (firstMatch == source[i] && Count - i >= patternLength) {

        //        }
        //    }

        //    return false;
        //}

        /// <summary>
        /// Removes the requested number of elements from the buffer
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        /// <remarks>
        /// Similar to the Take() Linq method, if the buffer contains less items than requested, and empty array of items is returned and no items are Removed
        /// </remarks>
        public T[] Remove(int count)
        {
            if (Count < count) return new T[] { };

            var result = new T[count];

            lock (_syncRoot) {
                for (int i = 0; i < count; i++) {
                    result[i] = Remove();
                }
            }

            return result;
        }

        public int MoveItemsTo(T[] destination, int index, int count)
        {
            if (count <= 0) return 0;

            try
            {
                lock (_syncRoot)
                {
                    // how many are we moving?
                    // move from current toward the tail
                    var actual = (count > this.Count) ? this.Count : count;

                    if (_tail < _head || (_tail == 0 && IsFull))
                    {
                        // the data is linear, just copy
                        Array.Copy(_list, destination, actual);
                        // move the tail pointer
                        _tail += actual;
                    }
                    else
                    {
                        // there's a data wrap
                        // copy from here to the end
                        var remaining = actual;
                        var c = _list.Length - _tail;
                        Array.Copy(_list, _tail, destination, 0 + index, c);
                        // now copy from the start (tail == 0) the remaining data
                        _tail = 0;
                        remaining -= c;
                        Array.Copy(_list, _tail, destination, c + index, remaining);

                        // move the tail pointer
                        _tail = remaining;
                    }

                    return actual;
                }
            }
            finally
            {
                if ((LowWaterLevel > 0) && (Count <= LowWaterLevel))
                {
                    if (!_lowwaterExceeded)
                    {
                        _lowwaterExceeded = true;
                        LowWater?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
        }

        public T this[int index] {
            get {
                lock (_syncRoot) {
                    int i = _tail + index;

                    if ((_head <= _tail) && (i >= MaxElements)) {
                        i -= MaxElements;
                    }

                    return _list[i];
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            // we don't want to enumerate values outside of our "valid" range
            for (int i = 0; i < Count; i++) {
                int index = _tail + i;

                if ((_head <= _tail) && (index >= MaxElements)) {
                    index -= MaxElements;
                }

                yield return _list[index];
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
