using Meadow.Hardware;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Meadow
{
    public abstract class NetworkAdapterBase : INetworkAdapter
    {
        public event NetworkConnectionHandler NetworkConnected;
        public event NetworkDisconnectionHandler NetworkDisconnected;

        private readonly Lazy<NetworkInterface?> nativeInterface;

        public abstract bool IsConnected { get; }

        public PhysicalAddress MacAddress { get; private set; }

        public NetworkInterfaceType InterfaceType { get; private set; }

        protected internal NetworkAdapterBase(NetworkInterfaceType expectedType)
        {
            InterfaceType = expectedType;
            nativeInterface = new Lazy<NetworkInterface?>(() =>
            {
                return LoadAdapterInfo();
            });
        }

        /// <summary>
        /// IP Address of the network adapter.
        /// </summary>
        public IPAddress IpAddress
        {
            get
            {
                if (nativeInterface == null)
                {
                    return IPAddress.None;
                }

                return nativeInterface.Value?.GetIPProperties()?.UnicastAddresses?.FirstOrDefault()?.Address ?? IPAddress.None;
            }
        }

        /// <summary>
        /// Subnet mask of the adapter.
        /// </summary>
        public IPAddress SubnetMask
        {
            get
            {
                if (nativeInterface == null)
                {
                    return IPAddress.None;
                }

                return nativeInterface.Value?.GetIPProperties()?.UnicastAddresses?.FirstOrDefault()?.IPv4Mask ?? IPAddress.None;
            }
        }

        /// <summary>
        /// Default gateway for the adapter.
        /// </summary>
        public IPAddress Gateway
        {
            get
            {
                if (nativeInterface == null)
                {
                    return IPAddress.None;
                }

                return nativeInterface.Value?.GetIPProperties()?.GatewayAddresses?.FirstOrDefault()?.Address ?? IPAddress.None;
            }
        }

        private NetworkInterface? LoadAdapterInfo()
        {
            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();

                if (interfaces.Length > 0)
                {
                    foreach (var intf in interfaces)
                    {
                        var p = intf.GetIPProperties();

                        MacAddress = intf.GetPhysicalAddress();

                        if (intf.NetworkInterfaceType == InterfaceType)
                        {
                            Resolver.Log.Trace($"Interface: {intf.Id}: {intf.Name} {intf.NetworkInterfaceType} {intf.OperationalStatus}");
                            return intf;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Error(ex.Message);
            }
            return null;
        }
    }
}
