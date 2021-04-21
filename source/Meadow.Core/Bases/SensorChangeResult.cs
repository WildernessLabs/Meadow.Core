using Meadow.Units;

namespace Meadow.Bases
{
    public class SensorChangeResult<U> where U : IUnitType
    {
        public U Old { get; set; }
        public U New { get; set; }
        public U Delta { get; set; }
    }
}
