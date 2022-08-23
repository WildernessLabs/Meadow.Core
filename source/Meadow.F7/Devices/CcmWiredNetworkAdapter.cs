using Meadow.Hardware;
using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Meadow.Devices
{
    public class CcmWiredNetworkAdapter : IWiredNetworkAdapter
    {
        internal CcmWiredNetworkAdapter()
        {
            Resolver.Log.Info($"Creating network interface...");

            try
            {
                var interfaces = NetworkInterface.GetAllNetworkInterfaces();

                Resolver.Log.Info($"{interfaces.Length} interfaces...");

                foreach(var intf in interfaces)
                {
                    Resolver.Log.Info($"Interface: {intf.Id}: {intf.Name} {intf.NetworkInterfaceType} {intf.OperationalStatus}");

                    var p = intf.GetIPProperties();
                    Resolver.Log.Info($"MA: {p.MulticastAddresses.FirstOrDefault()}");
                    Resolver.Log.Info($"GA: {p.GatewayAddresses.FirstOrDefault()}");

                    var mac = intf.GetPhysicalAddress();
                    Resolver.Log.Info($"Mac: {p.GatewayAddresses}");
                }
            }
            catch(Exception ex)
            {
                Resolver.Log.Error(ex.Message);
            }
        }

        public bool IsConnected => false;

        public IPAddress IpAddress => IPAddress.None;

        public IPAddress SubnetMask => IPAddress.None;

        public IPAddress Gateway => IPAddress.None;

        public PhysicalAddress MacAddress => PhysicalAddress.None;

        public event NetworkConnectionHandler NetworkConnected;
        public event NetworkDisconnectionHandler NetworkDisconnected;
    }
}
