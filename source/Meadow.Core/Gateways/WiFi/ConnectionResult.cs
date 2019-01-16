using System;
namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Describes the result of an attempted WiFi network connection.
    /// </summary>
    public class ConnectionResult
    {
        public ConnectionStatus ConnectionStatus {get; protected set;}

        public ConnectionResult(ConnectionStatus status)
        {
            this.ConnectionStatus = status;
        }
    }
}
