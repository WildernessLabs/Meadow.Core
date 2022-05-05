using System.Collections;
using System.Collections.Generic;

namespace Meadow
{
    internal class LineCollectionEnumerator : IEnumerator<LineInfo>
    {
        private LineCollection _collection;
        private int index = 0;

        public LineCollectionEnumerator(LineCollection collection)
        {
            _collection = collection;
        }

        public LineInfo Current => _collection[index];
        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (index < _collection.Count - 2)
            {
                index++;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            index = 0;
        }
    }
}
