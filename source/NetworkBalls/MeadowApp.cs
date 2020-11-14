using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;

namespace NetworkBalls
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        WiFiAdapter wifiAdapter;
        Esp32Coprocessor esp32;

        public MeadowApp()// : base()
        {
            Initialize();



            //Device.WiFiAdapterInitilaized += (s,e) => {

            ScanForAccessPoints();

            GetWebPageAsync("http://52.219.117.19");

            //};

        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            esp32 = new Esp32Coprocessor();
            esp32.Reset();
            Thread.Sleep(5000);

            this.wifiAdapter = new WiFiAdapter(this.esp32);

            Console.WriteLine($"Connecting to WiFi Network {Secrets.WIFI_NAME}");

            if (wifiAdapter.Connect(Secrets.WIFI_NAME, Secrets.WIFI_PASSWORD).ConnectionStatus != ConnectionStatus.Success)
            {
                throw new Exception("Cannot connect to network, applicaiton halted.");
            }
            Console.WriteLine("Connection request completed.");

        }

        protected void ScanForAccessPoints()
        {
            Console.WriteLine("Getting list of access points.");
            this.wifiAdapter.Scan();
            if (this.wifiAdapter.Networks.Count > 0)
            {
                Console.WriteLine("|-------------------------------------------------------------|---------|");
                Console.WriteLine("|         Network Name             | RSSI |       BSSID       | Channel |");
                Console.WriteLine("|-------------------------------------------------------------|---------|");
                foreach (WifiNetwork accessPoint in this.wifiAdapter.Networks)
                {
                    Console.WriteLine($"| {accessPoint.Ssid,-32} | {accessPoint.SignalDbStrength,4} | {accessPoint.Bssid,17} |   {accessPoint.ChannelCenterFrequency,3}   |");
                }
            }
            else
            {
                Console.WriteLine($"No access points detected.");
            }
            Thread.Sleep(15000);
        }

        public void GetWebPageAsync(string uri)
        {
            Console.WriteLine($"Requesting {uri}");
            HttpClient client = new HttpClient();
            var content = client.GetStringAsync(uri).Result;
            Console.WriteLine(content);
        }

    }

    public class Secrets
    {
        /// <summary>
        /// Name of the WiFi network to use.
        /// </summary>
        public const string WIFI_NAME = "comeonentrada";
        /// <summary>
        /// Password for the WiFi network names in WIFI_NAME.
        /// </summary>
        public const string WIFI_PASSWORD = "brandon11763";
    }
}
