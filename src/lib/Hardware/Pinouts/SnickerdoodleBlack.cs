using Meadow.Hardware;
using System.Collections.Generic;

namespace Meadow.Pinouts
{
    public class SnickerdoodleBlack : IPinDefinitions
    {
        public IList<IPin> AllPins => new List<IPin> 
        {
        };

        public IPin GPIO1 => new GpiodPin("GPIO1", "PIN001", "gpiochip0", 1);
    }
}