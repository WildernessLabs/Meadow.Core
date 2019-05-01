using System;
using System.Collections.Generic;
using Meadow.Hardware;

namespace Meadow.Devices
{
    public class NamedPinGroup
    {
        public string Name { get; protected set; }  

        public IPin[] Pins { get; protected set; }

        public NamedPinGroup(string name, IPin[] pins)
        {
            this.Name = name;
            this.Pins = pins;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
