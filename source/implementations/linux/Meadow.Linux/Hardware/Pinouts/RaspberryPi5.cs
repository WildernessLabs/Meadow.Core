using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Pinouts
{
    public class RaspberryPi5 : RaspberryPi
    {
        public RaspberryPi5()
        {
            // the sysfs base for the pinctrl-rpi1 is 53
            // the gpiochip bank for pinctrl-rpi1 on rpi5 is gpiochip4
            foreach (LinuxFlexiPin pin in this.AllPins) {
                pin.SysFsGpio += 53;
                pin.GpiodChip = "gpiochip4";
            }
        }
    }
}
