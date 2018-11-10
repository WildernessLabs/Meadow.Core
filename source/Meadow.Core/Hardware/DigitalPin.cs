using System;
namespace Meadow.Hardware
{
	public class DigitalPin : DigitalPinBase
    {
        public DigitalPin(string name, uint address, 
                          bool interruptCapable = false, 
                          bool pullDownCapable = false, 
                          bool pullUpCapable = false) 
            : base(name, address, interruptCapable, pullDownCapable, 
                   pullUpCapable)
        {


        }
    }
}
