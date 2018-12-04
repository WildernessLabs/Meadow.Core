using System;
namespace Meadow.Gateway.WiFi
{
    public class ConnectionResult
    {
        public ConnectionStatus ConnectionStatus {get; protected set;}

        public ConnectionResult(ConnectionStatus status)
        {
            this.ConnectionStatus = status;
        }
    }
}
