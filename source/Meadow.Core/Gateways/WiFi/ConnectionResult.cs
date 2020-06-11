using System;
namespace Meadow.Gateway.WiFi
{
    /// <summary>
    /// Describes the result of an attempted WiFi network connection.
    /// </summary>
    public class ConnectionResult
    {
        #region Properties
        
        /// <summary>
        /// Connecrtion status.
        /// </summary>
        public ConnectionStatus ConnectionStatus { get; protected set; }
        
        #endregion Properties

        #region Constructor(s)

        /// <summary>
        /// Default constructor is private to prevent it from being used.
        /// </summary>
        private ConnectionResult()
        {
        }
        
        /// <summary>
        /// Create a new ConnectionResult object.
        /// </summary>
        /// <param name="connectionStatus">Status of the connection attempt.</param>
        public ConnectionResult(ConnectionStatus connectionStatus)
        {
            ConnectionStatus = connectionStatus;
        }
        
        #endregion Consytructor(s)
    }
}
