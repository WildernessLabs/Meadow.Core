using System.Collections.Generic;

namespace Meadow;

/// <summary>
/// Interface for GPIO chip collections across libgpiod versions
/// </summary>
internal interface IChipCollection : IEnumerable<ChipInfo>
{
    int Count { get; }
    ChipInfo this[int index] { get; }
    ChipInfo? this[string name] { get; }
    bool Contains(string name);
    void Add(ChipInfo chip);
}
