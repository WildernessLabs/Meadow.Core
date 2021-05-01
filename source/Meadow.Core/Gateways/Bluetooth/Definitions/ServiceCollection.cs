
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Gateways.Bluetooth
{
    public class ServiceCollection : IEnumerable<Service>
    {
        private Dictionary<string, Service> m_services = new Dictionary<string, Service>(StringComparer.InvariantCultureIgnoreCase);

        internal ServiceCollection()
        {

        }

        public int Count
        {
            get => m_services.Count;
        }

        internal void Add(Service service)
        {
            m_services.Add(service.Name, service);
        }

        internal void AddRange(IEnumerable<Service> services)
        {
            foreach (var s in services)
            {
                Add(s);
            }
        }

        public IEnumerator<Service> GetEnumerator()
        {
            return m_services.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Service? this[string serviceName]
        {
            get => m_services.ContainsKey(serviceName) ? m_services[serviceName] : null;
        }

        public Service? this[int index]
        {
            get => m_services.Values.ElementAt(index);
        }
    }
}
