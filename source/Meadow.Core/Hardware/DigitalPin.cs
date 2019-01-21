using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a pin that can is connected to a digital input and/or output
    /// channel on the Meadow device.
    /// </summary>
	public class DigitalPin : DigitalPinBase
    {
        private IGPIOManager _manager;

        public DigitalPin(string name, 
                          object key, 
                          bool interruptCapable = false, 
                          bool pullDownCapable = false, 
                          bool pullUpCapable = false) 
            : base(name, key, interruptCapable, pullDownCapable, 
                   pullUpCapable)
        {


        }

        public override IGPIOManager GPIOManager 
        {
            get => _manager;
            internal set => _manager = value;
        }
    }
}
