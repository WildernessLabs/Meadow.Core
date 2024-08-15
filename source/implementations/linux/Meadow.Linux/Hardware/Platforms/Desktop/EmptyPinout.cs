using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Pinouts;

/// <summary>
/// Defines the an empty pinout configuration
/// </summary>
public class EmptyPinout : IPinDefinitions
{
    /// <inheritdoc/>
    public IEnumerator<IPin> GetEnumerator() => AllPins.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public IList<IPin> AllPins => new List<IPin>();

    /// <inheritdoc/>
    public IPinController? Controller { get; set; }

    internal EmptyPinout()
    {
    }
}
