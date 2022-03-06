using System;
using Meadow.Hardware;

namespace Meadow.Devices
{
	public partial class F7FeatherV1
	{
        public class F7SerialPortNameDefinitions {
            public SerialPortName Com1 { get; } = new SerialPortName("COM1", "ttyS0");
            public SerialPortName Com4 { get; } = new SerialPortName("COM4", "ttyS1");
        }
    }
}
