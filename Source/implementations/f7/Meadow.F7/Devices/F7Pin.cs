using Meadow.Hardware;
using System.Collections.Generic;
using static Meadow.Core.Interop;

namespace Meadow.Devices;

internal class F7Pin : Pin
{
    public STM32.GpioPort ProcessorPort { get; }
    public int ProcessorPin { get; }

    public F7Pin(
        IPinController? controller,
        string name,
        object key,
        STM32.GpioPort port,
        int pin,
        IList<IChannelInfo>? supportedChannels)
        : base(controller, name, key, supportedChannels)
    {
        ProcessorPort = port;
        ProcessorPin = pin;
    }
}
