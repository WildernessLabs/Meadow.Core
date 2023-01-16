using System;

namespace Meadow.Hardware
{
    public class InterruptGroupInUseException : Exception
    {
        public int Group { get; }

        public InterruptGroupInUseException(int group)
            : base($"Interrupt group {group} is already in use")
        {
        }
    }
}
