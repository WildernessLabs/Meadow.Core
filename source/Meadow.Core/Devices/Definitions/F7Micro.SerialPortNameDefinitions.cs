using System;
using Meadow.Hardware;

namespace Meadow.Devices
{
	public partial class F7Micro
	{
        public class SerialPortNameDefinitions {
            public SerialPortName Com1 { get; } = new SerialPortName("COM1", "ttyS0");
            public SerialPortName Com4 { get; } = new SerialPortName("COM4", "ttyS1");
        }
    }
}
