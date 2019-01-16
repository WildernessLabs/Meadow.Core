using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Represents a WiFi network adapter.
    /// </summary>
    public class WiFiAdapter
    {
        public ObservableCollection<WifiNetwork> Networks { get; set; }

        public bool IsConnected { get; }
        public bool HasInternetAccess { get; }

        public WiFiAdapter()
        {

        }

        public void BeginScan()
        {
            Task.Run(() => { 
                // do work.
            });
        }

        public Task<ConnectionResult> Connect(WifiNetwork network, 
            ReconnectionType reconnection = ReconnectionType.Automatic) {
            return new Task<ConnectionResult>(() =>
            {
                return new ConnectionResult(ConnectionStatus.Timeout);
            });
        }

        //TODO: we should probably be using some sort of password credential and secure storage see: https://docs.microsoft.com/en-us/uwp/api/windows.security.credentials.passwordcredential
        public Task<ConnectionResult> Connect(WifiNetwork network, 
            string password, 
            ReconnectionType reconnection = ReconnectionType.Automatic)
        {
            return new Task<ConnectionResult>(() =>
            {
                return new ConnectionResult(ConnectionStatus.Timeout);
            });
        }

        // for when the network is hidden
        public Task<ConnectionResult> Connect(WifiNetwork network, 
            string password,
            string ssid, 
            ReconnectionType reconnection = ReconnectionType.Automatic)
        {
            return new Task<ConnectionResult>(() =>
            {
                return new ConnectionResult(ConnectionStatus.Timeout);
            });
        }

        public Task<ConnectionResult> Connect(WifiNetwork network, 
            string password, 
            string ssid,
            ConnectionMethodType connectionMethodType,
            ReconnectionType reconnection = ReconnectionType.Automatic)
        {
            return new Task<ConnectionResult>(() =>
            {
                return new ConnectionResult(ConnectionStatus.Timeout);
            });
        }


        public void Disconect() { }

    }
}
