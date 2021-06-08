using Meadow.Hardware;

namespace Meadow
{
    public class JetsonNanoSerialPortNameDefinitions : LinuxSerialPortNameDefinitions
    {
        public JetsonNanoSerialPortNameDefinitions()
        {
            UART2 = new SerialPortName("UART2", "ttyTHS1");
        }
    }
}
