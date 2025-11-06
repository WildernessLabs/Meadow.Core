using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Meadow;

/// <summary>
/// Abstract base class for GPIO chip collections across libgpiod versions
/// </summary>
internal abstract class ChipCollection<TChipInfo> : IChipCollection
    where TChipInfo : ChipInfo
{
    protected readonly List<TChipInfo> _chips = new List<TChipInfo>();

    public int Count => _chips.Count;

    public ChipInfo this[int index]
    {
        get => _chips[index];
    }

    public ChipInfo? this[string name]
    {
        get => _chips.FirstOrDefault(c => c.Name == name);
    }

    // Explicit interface implementations for IChipCollection
    ChipInfo IChipCollection.this[int index] => this[index];
    ChipInfo? IChipCollection.this[string name] => this[name];

    public bool Contains(string name)
    {
        return _chips.Any(c => c.Name == name);
    }

    public void Add(ChipInfo chip)
    {
        _chips.Add((TChipInfo)chip);
    }

    public IEnumerator<TChipInfo> GetEnumerator()
    {
        return _chips.GetEnumerator();
    }

    IEnumerator<ChipInfo> IEnumerable<ChipInfo>.GetEnumerator()
    {
        return _chips.Cast<ChipInfo>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
