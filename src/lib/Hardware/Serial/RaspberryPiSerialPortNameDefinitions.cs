using Meadow.Hardware;

namespace Meadow
{
    public class RaspberryPiSerialPortNameDefinitions : LinuxSerialPortNameDefinitions
    {
        public RaspberryPiSerialPortNameDefinitions()
        {
            UART1 = new SerialPortName("UART1", "/dev/ttyAMA0");
        }


    }
}
