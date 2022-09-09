using Meadow.Hardware;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Meadow.Devices
{
    public class CcmWiredNetworkAdapter : NetworkAdapterBase
    {
        public override bool IsConnected => false;
    }

    public abstract class NetworkAdapterBase : IWiredNetworkAdapter
    {
        public event NetworkConnectionHandler NetworkConnected;
        public event NetworkDisconnectionHandler NetworkDisconnected;

        private readonly Lazy<NetworkInterface?> nativeInterface;

        public abstract bool IsConnected { get; }

        public PhysicalAddress MacAddress { get; private set; }

        internal NetworkAdapterBase()
        {
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
            Resolver.Log.Info($"Searching for wired network interface...");

            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();

                Resolver.Log.Info($"{interfaces.Length} interfaces...");

                if (interfaces.Length > 0)
                {
                    foreach (var intf in interfaces)
                    {
                        Resolver.Log.Info($"Interface: {intf.Id}: {intf.Name} {intf.NetworkInterfaceType} {intf.OperationalStatus}");

                        var p = intf.GetIPProperties();
                        Resolver.Log.Info($"MA: {p.MulticastAddresses.FirstOrDefault()}");
                        Resolver.Log.Info($"GA: {p.GatewayAddresses.FirstOrDefault()}");

                        MacAddress = intf.GetPhysicalAddress();
                        Resolver.Log.Info($"Mac: {MacAddress}");

                        // TODO: check for wired (v. wifi)
                        if (intf.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                        {
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
