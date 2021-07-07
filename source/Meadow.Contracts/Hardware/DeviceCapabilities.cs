namespace Meadow
{
    public class DeviceCapabilities
    {
        public NetworkCapabilities Network { get; protected set; }
        public AnalogCapabilities Analog { get; protected set; }

        public DeviceCapabilities(
            AnalogCapabilities analog,
            NetworkCapabilities network
            )
        {
            this.Network = network;
            this.Analog = analog;
        }
    }
}

