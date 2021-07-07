using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Describes the type of serial data received, either characters or an end 
    /// of file notification.
    /// </summary>
    public enum SerialDataType
    {
        /// <summary>
        /// Character data.
        /// </summary>
        Chars = 0,
        /// <summary>
        /// An End of File (EOF) notification.
        /// </summary>
        Eof = 1
    }
}
