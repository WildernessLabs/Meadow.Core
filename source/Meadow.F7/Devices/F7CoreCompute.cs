namespace Meadow.Devices
{
    public partial class F7CoreCompute : F7CoreComputeBase
    {
        public SerialPortNameDefinitions SerialPortNames => new SerialPortNameDefinitions();

        public F7CoreCompute()
            : base(new Pinout(),
                  new F7GPIOManager(),
                  new AnalogCapabilities(true, DefaultA2DResolution),
                  new NetworkCapabilities(true, false))
        {
        }
    }
}