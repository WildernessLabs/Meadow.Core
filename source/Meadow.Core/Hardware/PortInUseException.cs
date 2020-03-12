﻿using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Thrown when a port is attempted to be created on a pin or peripheral 
    /// that is already in use.
    /// </summary>
    public class PortInUseException : Exception
    {
        internal PortInUseException()
        {
        }

        internal PortInUseException(string message)
            : base(message)
        {
        }
    }
}
