using System;

namespace Meadow.Cloud;

/// <summary>
/// Represents errors that occur during MeadowCloud requests 
/// </summary>
public class MeadowCloudException : Exception
{
    /// <summary>
    /// Initializes a new instance of the MeadowCloudException class with a specified error message.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public MeadowCloudException(string message, Exception? innerException) : base(message, innerException) { }
}