using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Describes the type of error encountered during serial communication.
    /// </summary>
    public enum SerialErrorType
    {
        TXFull = 0,
        RXOver = 1,
        Overrun = 2,
        RXParity = 3,
        Frame = 4
    }
}
