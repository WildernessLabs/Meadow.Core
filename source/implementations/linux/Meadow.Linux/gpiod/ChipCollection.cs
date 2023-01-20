using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Meadow
{
    internal class ChipCollection : IEnumerable<ChipInfo>
    {
        private List<ChipInfo> _chips = new List<ChipInfo>();

        public ChipInfo this[int index]
        {
            get => _chips[index];
        }

        public bool Contains(string name)
        {
            return _chips.Any(c => c.Name == name);
        }

        public void Add(ChipInfo chip)
        {
            _chips.Add(chip);
        }

        public ChipInfo this[string name]
        {
            get => _chips.FirstOrDefault(c=> c.Name == name);
        }

        public IEnumerator<ChipInfo> GetEnumerator()
        {
            return _chips.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
