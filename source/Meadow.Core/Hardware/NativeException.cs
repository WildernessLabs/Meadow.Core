using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Encapsulates a native hardware exception 
    /// </summary>
    public class NativeException : Exception
    {
        /// <summary>
        /// Optional Error Code information from the underlying OS
        /// </summary>
        public int? ErrorCode { get; }

        /// <summary>
        /// Creates a NativeException object
        /// </summary>
        /// <param name="message"></param>
        public NativeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a NativeException object
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        public NativeException(string message, int errorCode)
            : base(message)
        {
            this.ErrorCode = errorCode;
        }
    }
}
