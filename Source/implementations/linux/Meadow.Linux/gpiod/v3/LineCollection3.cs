using System.Collections;
using System.Collections.Generic;

namespace Meadow;

/// <summary>
/// Collection of LineInfo3 objects for a GPIO chip
/// </summary>
internal class LineCollection3 : IEnumerable<LineInfo3>
{
    private readonly ChipInfo3 _chip;
    private readonly LineInfo3?[] _lines;

    public int Count => _lines.Length;

    public LineCollection3(ChipInfo3 chip, int count)
    {
        _chip = chip;
        _lines = new LineInfo3?[count];
    }

    public LineInfo3 this[int offset]
    {
        get
        {
            if (offset < 0 || offset >= _lines.Length)
            {
                throw new System.ArgumentOutOfRangeException(nameof(offset));
            }

            // Lazy initialization
            if (_lines[offset] == null)
            {
                _lines[offset] = new LineInfo3(_chip, offset);
            }

            return _lines[offset]!;
        }
    }

    public IEnumerator<LineInfo3> GetEnumerator()
    {
        for (int i = 0; i < _lines.Length; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
