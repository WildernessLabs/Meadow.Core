
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Gateways.Bluetooth
{
    public class ServiceCollection : IEnumerable<IService>
    {
        private Dictionary<string, IService> m_services = new(StringComparer.InvariantCultureIgnoreCase);

        public ServiceCollection()
        {

        }

        public int Count
        {
            get => m_services.Count;
        }

        public void Add(IService service)
        {
            m_services.Add(service.Name, service);
        }

        public void AddRange(IEnumerable<IService> services)
        {
            foreach (var s in services)
            {
                Add(s);
            }
        }

        public IEnumerator<IService> GetEnumerator()
        {
            return m_services.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IService? this[string serviceName]
        {
            get => m_services.ContainsKey(serviceName) ? m_services[serviceName] : null;
        }

        public IService? this[int index]
        {
            get => m_services.Values.ElementAt(index);
        }
    }
}
