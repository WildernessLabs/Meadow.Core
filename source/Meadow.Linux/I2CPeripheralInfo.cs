namespace Meadow
{
    internal class I2CPeripheralInfo
    {
        public int BusNumber { get; set; }
        public byte BusAddress { get; set; }
        public int DriverHandle { get; set; }
        public bool IsOpen { get; set; }
    }
}
