using System.Collections;
using System.Collections.Generic;

namespace Meadow;

internal class LineCollection2 : IEnumerable<LineInfo2>
{
    private LineInfo2[] Lines { get; }
    private ChipInfo2 Chip { get; }

    internal LineCollection2(ChipInfo2 chip, int count)
    {
        Chip = chip;
        Lines = new LineInfo2[count];
    }

    public int Count => Lines.Length;

    public LineInfo2 this[int index]
    {
        get
        {
            if (Lines[index] == null)
            {
                Lines[index] = new LineInfo2(Chip, index);
            }
            return Lines[index];
        }
    }

    public IEnumerator<LineInfo2> GetEnumerator()
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
