using Meadow.Hardware;

namespace Meadow
{
    public class JetsonXavierAGXSerialPortNameDefinitions : LinuxSerialPortNameDefinitions
    {
        // TODO: verify this
        public SerialPortName Serial0 => new SerialPortName("UART2", "/dev/ttyTHS1");

        public JetsonXavierAGXSerialPortNameDefinitions()
        {
        }
    }

    public class JetsonNanoSerialPortNameDefinitions : LinuxSerialPortNameDefinitions
    {
        // TODO: verify this
        public SerialPortName Serial0 => new SerialPortName("UART2", "/dev/ttyTHS1");

        public JetsonNanoSerialPortNameDefinitions()
        {
        }
    }
}
