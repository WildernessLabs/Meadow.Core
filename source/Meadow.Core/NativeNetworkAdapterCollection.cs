using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Meadow.Devices
{
    /// <summary>
    /// Provides a collection of .NET native network adapters
    /// </summary>
    public class NativeNetworkAdapterCollection : INetworkAdapterCollection
    {
        // DEV NOTE: This collection is not used in the F7 - it is used by Linux and Windows
        //           For F7 implementations, see the NetworkAdapterCollection class
        private List<INetworkAdapter> _adapters = new List<INetworkAdapter>();

        /// <summary>
        /// Gets an INetworkAdapter from the collection at a specified index
        /// </summary>
        /// <param name="index">The index of the adapter to retrieve</param>
        public INetworkAdapter this[int index] => _adapters[index];

        /// <summary>
        /// Creates a NativeNetworkAdapterCollection
        /// </summary>
        public NativeNetworkAdapterCollection()
        {
            Refresh();
        }

        /// <summary>
        /// Gets an enumerator for the collection
        /// </summary>
        public IEnumerator<INetworkAdapter> GetEnumerator()
        {
            return _adapters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Refreshes the collection
        /// </summary>
        public Task Refresh()
        {
            lock (_adapters)
            {
                _adapters.Clear();

                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    switch (ni.NetworkInterfaceType)
                    {
                        case NetworkInterfaceType.Ethernet:
                            _adapters.Add(new WiredNetworkAdapter(ni));
                            break;
                        case NetworkInterfaceType.Wireless80211:
                            _adapters.Add(new WiFiNetworkAdapter(ni));
                            break;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
