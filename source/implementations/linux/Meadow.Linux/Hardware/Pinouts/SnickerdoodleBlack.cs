using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Pinouts
{
    public class SnickerdoodleBlack : IPinDefinitions
    {
        public IEnumerator<IPin> GetEnumerator() => AllPins.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IList<IPin> AllPins => new List<IPin>
        {
            GPIO1, GPIO2, GPIO3, GPIO4, GPIO5, GPIO6, GPIO7, GPIO8
//            GPIO0, GPIO1, GPIO2, GPIO3, GPIO4, GPIO5, GPIO6, GPIO7, GPIO8, GPIO9
        };

        public IPinController Controller { get; set; }

        public SnickerdoodleBlack()
        {
        }

        //        public IPin GPIO0 => new GpiodPin("GPIO0", "PIN001", "gpiochip0", 0);
        public IPin GPIO1 => new GpiodPin(Controller, "GPIO1", "PIN001", "gpiochip0", 1);
        public IPin GPIO2 => new GpiodPin(Controller, "GPIO2", "PIN001", "gpiochip0", 2);
        public IPin GPIO3 => new GpiodPin(Controller, "GPIO3", "PIN001", "gpiochip0", 3);
        public IPin GPIO4 => new GpiodPin(Controller, "GPIO4", "PIN001", "gpiochip0", 4);
        public IPin GPIO5 => new GpiodPin(Controller, "GPIO5", "PIN001", "gpiochip0", 5);
        public IPin GPIO6 => new GpiodPin(Controller, "GPIO6", "PIN001", "gpiochip0", 6);
        public IPin GPIO7 => new GpiodPin(Controller, "GPIO7", "PIN001", "gpiochip0", 7);
        public IPin GPIO8 => new GpiodPin(Controller, "GPIO8", "PIN001", "gpiochip0", 8);
        //        public IPin GPIO9 => new GpiodPin("GPIO9", "PIN001", "gpiochip0", 9);

    }
}