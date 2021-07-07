using System;
namespace Meadow
{
    public class AnalogCapabilities
    {
        public AnalogCapabilities(
            bool hasAdc,
            int? adcResolution
            )
        {
            this.HasAdc = hasAdc;
            this.AdcResolution = adcResolution;
        }

        public bool HasAdc { get; protected set; }
        public int? AdcResolution { get; protected set; }
        public int? MaxRawAdcVoltageValue {
            get {
                if (_maxRawAdcVoltageValue != null) {
                    return _maxRawAdcVoltageValue;
                } else {
                    _maxRawAdcVoltageValue = (int?)Math.Pow(2, (double)(AdcResolution ?? 1));
                    return _maxRawAdcVoltageValue;
                }
            }
        } protected int? _maxRawAdcVoltageValue;
    }
}

