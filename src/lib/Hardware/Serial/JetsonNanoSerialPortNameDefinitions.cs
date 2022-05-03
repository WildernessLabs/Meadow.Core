using Meadow.Hardware;

namespace Meadow
{
    public class JetsonXavierAGXSerialPortNameDefinitions : LinuxSerialPortNameDefinitions
    {
        public JetsonXavierAGXSerialPortNameDefinitions()
        {
            // TODO: verify this
            UART2 = new SerialPortName("UART2", "ttyTHS1");
        }
    }

    public class JetsonNanoSerialPortNameDefinitions : LinuxSerialPortNameDefinitions
    {
        public JetsonNanoSerialPortNameDefinitions()
        {
            UART2 = new SerialPortName("UART2", "ttyTHS1");
        }
    }
}
