

using System;
using System.Threading;

namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for Meadow devices.
    /// </summary>
    public interface IIOController<T> where T : IPinDefinitions
    {
        /// <summary>
        /// Retrieves an IPin by name
        /// </summary>
        /// <param name="pinName">Registered name of the pin to retrieve</param>
        /// <returns>The requested pin or null if not found</returns>
        IPin GetPin(string pinName);

        // TODO: When we move to C# 9 or whatever that will allow more specific
        // types of the interface, e.g.; `IF7MicroPinout : IPinDefinitions`
        // then change this to remove the generic `<T> where T : IPinDefinitions`
        //IPinDefinitions Pins { get; protected set; }
        T Pins { get; }
    }
}
