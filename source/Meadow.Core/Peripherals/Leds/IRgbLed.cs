using System;
namespace Meadow.Peripherals.Leds
{
    public interface IRgbLed
    {
        CommonType Common { get; }

        public enum CommonType
        {
            CommonCathode,
            CommonAnode
        }
    }
}
