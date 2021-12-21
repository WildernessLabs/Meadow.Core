namespace Meadow
{
    public partial class I2CBus
    {
        private class I2CPeripheralInfo
        {
            public int BusNumber { get; set; }
            public byte BusAddress { get; set; }
            public int DriverHandle { get; set; }
            public bool IsOpen { get; set; }
        }
    }
}
