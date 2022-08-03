using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;

namespace Meadow
{
    public class NetworkAdapterCollection : INetworkAdapterCollection
    {
        public event NetworkConnectionHandler NetworkConnected = delegate { };
        public event NetworkDisconnectionHandler NetworkDisconnected = delegate { };

        private List<INetworkAdapter> _adapters = new List<INetworkAdapter>();

        public INetworkAdapter this[int index] => _adapters[index];

        public void Add(INetworkAdapter adapter)
        {
            _adapters.Add(adapter);

            adapter.NetworkConnected += (s, e) => NetworkConnected.Invoke(s, e);
            adapter.NetworkDisconnected += (s) => NetworkDisconnected.Invoke(s);
        }

        public IEnumerator<INetworkAdapter> GetEnumerator()
        {
            return _adapters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
