using System.Collections;
using System.Collections.Generic;

namespace Meadow;

internal class LineCollection : IEnumerable<LineInfo>
{
    private LineInfo[] Lines { get; }
    private ChipInfo Chip { get; }

    internal LineCollection(ChipInfo chip, int count)
    {
        Chip = chip;
        Lines = new LineInfo[count];
    }

    public int Count => Lines.Length;

    public LineInfo this[int index]
    {
        get
        {
            if (Lines[index] == null)
            {
                Lines[index] = new LineInfo(Chip, index);
            }
            return Lines[index];
        }
    }

    public IEnumerator<LineInfo> GetEnumerator()
    {
        int position = 0; // state
        while (position < Count)
        {
            position++;
            yield return this[position - 1];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
