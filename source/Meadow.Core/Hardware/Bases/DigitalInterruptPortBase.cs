using System;
namespace Meadow.Hardware
{
    public class DigitalInterruptPortBase : DigitalInputPortBase, IDigitalInterruptPort
    {
public DigitalInterruptPortBase()
        {
        }

        public override bool Value => throw new NotImplementedException();

        public bool InterrupEnabled => true;
    }
}
