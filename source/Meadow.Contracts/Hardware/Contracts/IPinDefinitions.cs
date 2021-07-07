using System;
using System.Collections.Generic;
using System.Linq;
using Meadow.Hardware;

namespace Meadow.Hardware
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

        IPin this[string name] 
        {
            get => AllPins.FirstOrDefault(p => 
                string.Compare(p.Name, name, true) == 0 
                || string.Compare($"{p.Key}", name, true) == 0);
        }

        // TODO: if we do this, we can't type the instance version
        // so we won't get good autocomplete for specific devices, e.g.:
        // f7Micro.Groups.I2c1
        //INamedPinGroups Groups { get; } 
    }
}
