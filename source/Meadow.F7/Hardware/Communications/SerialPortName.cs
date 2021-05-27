using System;
namespace Meadow.Hardware
{
    public class SerialPortName
    {
        public string FriendlyName { get; set; }
        public string SystemName { get; set; }

        public SerialPortName(string friendlyName, string systemName)
        {
            this.FriendlyName = friendlyName;
            this.SystemName = systemName;
        }
    }
}
