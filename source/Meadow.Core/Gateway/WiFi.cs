using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meadow.Gateway
{
    public class WiFi
    {
        public static WiFi Current { get; private set; }

        public IObservable<IList<SSID>> Channels { get; set; }

        public bool IsConnected { get; }
        public bool HasInternetAccess { get; }

        static WiFi()
        {
            Current = new WiFi();
        }

        protected WiFi()
        {

        }

        public void RefreshSsidChannels()
        {
            return;
        }

    }
}
