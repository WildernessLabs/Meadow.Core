using System;

namespace Meadow.Hardware
{
    public class InterruptGroupInUseException : Exception
    {
        public int Group { get; }
        public int PinAlreadyAssigned { get; }

        internal InterruptGroupInUseException(int group, int pinAlreadyAssigned)
            : base($"Interrupt group {group} is already in use")
        {
            PinAlreadyAssigned = pinAlreadyAssigned;
        }
    }
}
