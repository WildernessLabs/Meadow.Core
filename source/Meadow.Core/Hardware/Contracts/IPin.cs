using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for a pin on the Meadow board.
    /// </summary>
    public interface IPin
    {
        string Name { get; }
        object Key { get; }
    }
}
