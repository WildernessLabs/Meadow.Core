using System;
namespace Meadow.Hardware
{
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
