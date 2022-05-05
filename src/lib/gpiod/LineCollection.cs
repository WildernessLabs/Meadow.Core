using System.Collections;
using System.Collections.Generic;

namespace Meadow
{
    internal class LineCollection : IEnumerable<LineInfo>
    {
        private LineInfo[] Lines { get; }
        private ChipInfo Chip { get; }

        private LineCollectionEnumerator _enumerator;

        internal LineCollection(ChipInfo chip, int count)
        {
            Chip = chip;
            Lines = new LineInfo[count];
            _enumerator = new LineCollectionEnumerator(this);
        }

        public int Count => Lines.Length;

        public LineInfo this[int index]
        {
            get 
            {
                if(Lines[index] == null)
                {
                    Lines[index] = new LineInfo(Chip, index);
                }
                return Lines[index];
            }
        }

        public IEnumerator<LineInfo> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
