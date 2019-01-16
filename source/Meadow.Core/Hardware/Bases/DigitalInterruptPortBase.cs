using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides a base implementation for digital interrupt ports; digital input
    /// ports that notify on change.
    /// </summary>
    public class DigitalInterruptPortBase : DigitalInputPortBase, IDigitalInterruptPort
    {
        public DigitalInterruptPortBase()
        {
        }

        public override bool State { get; }

        bool IDigitalInterruptPort.InterrupEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
