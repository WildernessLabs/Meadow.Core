using System;

namespace Meadow
{
    /// <summary>
    /// A general Network exception
    /// </summary>
    public class NetworkException : Exception
    {
        public int StatusCode { get; }

        /// <summary>
        /// Creates a NetworkException instance
        /// </summary>
        public NetworkException()
        {
        }

        /// <summary>
        /// Creates a NetworkException instance
        /// </summary>
        /// <param name="message"></param>
        /// <param name="statusCode"></param>
        public NetworkException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Creates a NetworkException instance
        /// </summary>
        /// <param name="statusCode"></param>
        public NetworkException(int statusCode)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Creates a NetworkException instance
        /// </summary>
        /// <param name="message"></param>
        public NetworkException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// An exception indicating a requested network was not found
    /// </summary>
    public class NetworkNotFoundException : NetworkException
    {
        /// <summary>
        /// Creates a NetworkNotFoundException instance
        /// </summary>
        /// <param name="message"></param>
        public NetworkNotFoundException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// An exception indicating a network authentication failure
    /// </summary>
    public class NetworkAuthenticationException : NetworkException
    {
        /// <summary>
        /// Creates a NetworkAuthenticationException instance
        /// </summary>
        /// <param name="message"></param>
        public NetworkAuthenticationException(string message)
            : base(message)
        {
        }
    }
}
