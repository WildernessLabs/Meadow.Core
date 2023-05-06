using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Pinouts
{
    public class WSL2 : IPinDefinitions
    {
        public IEnumerator<IPin> GetEnumerator() => AllPins.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IList<IPin> AllPins => new List<IPin>();

        public IPinController Controller { get; set; }

        public WSL2()
        {
        }
    }
}
