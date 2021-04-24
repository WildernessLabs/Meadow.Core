
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Gateways.Bluetooth
{
    public class CharacteristicCollection : IEnumerable<ICharacteristic>
    {
        private Dictionary<string, ICharacteristic> m_characteristics = new Dictionary<string, ICharacteristic>(StringComparer.InvariantCultureIgnoreCase);

        internal CharacteristicCollection()
        {

        }

        public int Count
        {
            get => m_characteristics.Count;
        }

        internal void Add(ICharacteristic characteristic)
        {
            m_characteristics.Add(characteristic.Name, characteristic);
        }

        internal void AddRange(IEnumerable<ICharacteristic> characteristics)
        {
            foreach (var c in characteristics)
            {
                Add(c);
            }
        }

        public IEnumerator<ICharacteristic> GetEnumerator()
        {
            return m_characteristics.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ICharacteristic? this[string nameOrId]
        {
            get
            {
                if (m_characteristics.ContainsKey(nameOrId))
                {
                    return m_characteristics[nameOrId];
                }

                return m_characteristics.Values.FirstOrDefault(c => string.Compare(c.Uuid, nameOrId, true) == 0);
            }
        }

        public ICharacteristic? this[int index]
        {
            get => m_characteristics.Values.ElementAt(index);
        }

    }
}
