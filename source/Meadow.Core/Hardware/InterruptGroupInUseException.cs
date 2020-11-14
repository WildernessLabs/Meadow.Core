using System;

namespace Meadow.Hardware
{
    public class InterruptGroupInUseException : Exception
    {
        public int Group { get; }

        internal InterruptGroupInUseException(int group)
            : base($"Interrupt group {group} is already in use")
        {
        }
    }
}
