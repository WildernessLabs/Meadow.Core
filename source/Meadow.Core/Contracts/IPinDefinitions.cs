using System;
using System.Collections.Generic;
using Meadow.Hardware;

namespace Meadow
{
    /// <summary>
    /// Contract for device pin lists.
    /// </summary>
    public interface IPinDefinitions
    {
        /// <summary>
        /// Convenience property which contains all the pins avaiable on the 
        /// device.
        /// </summary>
        /// <value>All the pins.</value>
        IList<IPin> AllPins { get; }
    }
}
