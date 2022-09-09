using Meadow.Hardware;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Meadow.Devices
{
    public class CcmWiredNetworkAdapter : IWiredNetworkAdapter
    {
        public event NetworkConnectionHandler NetworkConnected;
        public event NetworkDisconnectionHandler NetworkDisconnected;

        private NetworkInterface? nativeInterface;

        public bool IsConnected => false;
        public PhysicalAddress MacAddress { get; private set; }

        internal CcmWiredNetworkAdapter()
        {
            LoadAdapterInfo();
        }

        public IPAddress IpAddress
        {
            get
            {
                if(nativeInterface == null)
                {
                    return IPAddress.None;
                }

                return nativeInterface.GetIPProperties()?.UnicastAddresses?.FirstOrDefault()?.Address ?? IPAddress.None;
            }
        }

        public IPAddress SubnetMask
        {
            get
            {
                if(nativeInterface == null)
                {
                    return IPAddress.None;
                }

                return nativeInterface.GetIPProperties()?.UnicastAddresses?.FirstOrDefault()?.IPv4Mask ?? IPAddress.None;
            }
        }

        public IPAddress Gateway
        {
            get
            {
                if(nativeInterface == null)
                {
                    return IPAddress.None;
                }

                return nativeInterface.GetIPProperties()?.GatewayAddresses?.FirstOrDefault()?.Address ?? IPAddress.None;
            }
        }

        private void LoadAdapterInfo()
        {
            Task.Run(async () =>
            {
                Resolver.Log.Info($"Searching for wired network interface...");

                try
                {
                    while(true)
                    {
                        var interfaces = NetworkInterface.GetAllNetworkInterfaces();

                        Resolver.Log.Info($"{interfaces.Length} interfaces...");

                        if(interfaces.Length > 0)
                        {
                            foreach(var intf in interfaces)
                            {
                                Resolver.Log.Info($"Interface: {intf.Id}: {intf.Name} {intf.NetworkInterfaceType} {intf.OperationalStatus}");

                                var p = intf.GetIPProperties();
                                Resolver.Log.Info($"MA: {p.MulticastAddresses.FirstOrDefault()}");
                                Resolver.Log.Info($"GA: {p.GatewayAddresses.FirstOrDefault()}");

                                MacAddress = intf.GetPhysicalAddress();
                                Resolver.Log.Info($"Mac: {MacAddress}");

                                // TODO: check for wired (v. wifi)
                                if(intf.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                                {
                                    nativeInterface = intf;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            await Task.Delay(TimeSpan.FromSeconds(5));
                        }
                    }
                }
                catch(Exception ex)
                {
                    Resolver.Log.Error(ex.Message);
                }
            });
        }
    }
}
