using System;
namespace Meadow.Hardware
{
    public class DigitalInterruptPortBase : DigitalInputPortBase, IDigitalInterruptPort
    {
        public DigitalInterruptPortBase()
        {
        }

        public override bool State { get; }

        bool IDigitalInterruptPort.InterrupEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
