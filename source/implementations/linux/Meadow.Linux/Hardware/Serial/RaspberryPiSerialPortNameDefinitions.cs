using Meadow.Hardware;

namespace Meadow
{
    public class RaspberryPiSerialPortNameDefinitions : LinuxSerialPortNameDefinitions
    {
        public SerialPortName Serial0 => new SerialPortName("serial0", "/dev/serial0");
        public SerialPortName TtyAma0 => new SerialPortName("serial0", "/dev/ttyAMA0");


        public RaspberryPiSerialPortNameDefinitions()
        {
        }


    }
}
