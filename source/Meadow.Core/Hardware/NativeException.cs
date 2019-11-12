using System;

namespace Meadow.Hardware
{
    public class NativeException : Exception
    {
        internal NativeException(string message)
            : base(message)
        {
        }

        internal NativeException(Core.Interop.Nuttx.ErrorCode errorCode)
            : base($"An exception occurred: {errorCode.ToString()}")
        {
        }
    }
}
