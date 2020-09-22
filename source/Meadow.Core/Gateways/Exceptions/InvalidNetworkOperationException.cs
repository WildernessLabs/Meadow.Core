using System;

namespace Meadow.Gateways.Exceptions
{
    /// <summary>
    /// Invalid network operation exception.
    /// </summary>
    public class InvalidNetworkOperationException : Exception
    {
        /// <summary>
        /// Default constructor for the CoprocessorResponseNotFoundException.
        /// </summary>
        public InvalidNetworkOperationException()
        {
        }

        /// <summary>
        /// Create a new CoprocessorResponseNotFoundException object passing on the
        /// message information.
        /// </summary>
        /// <param name="message">Message for the calling application.</param>
        public InvalidNetworkOperationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Create a new CoprocessorResponseNotFoundException object passing on the
        /// message and inner exception details.
        /// </summary>
        /// <param name="message">Message for the calling application.</param>
        /// <param name="inner">Inner exception information.</param>
        public InvalidNetworkOperationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}