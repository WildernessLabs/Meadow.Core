using System;
using System.Collections.Generic;

namespace Meadow.Hardware.Communications
{
    public interface ISpiBus
    {
        List<ISpiPeripheral> Peripherals { get; }
    }
}
