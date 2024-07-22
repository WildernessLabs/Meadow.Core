using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Pinouts;

/// <summary>
/// Defines the pinout configuration for Windows Subsystem for Linux (WSL)
/// </summary>
public class WSL2 : IPinDefinitions
{
    /// <inheritdoc/>
    public IEnumerator<IPin> GetEnumerator() => AllPins.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    public IList<IPin> AllPins => new List<IPin>();

    /// <inheritdoc/>
    public IPinController? Controller { get; set; }

    internal WSL2()
    {
    }
}
