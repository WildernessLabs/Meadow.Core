using System;
namespace Meadow
{
    public class DeviceCapabilities
    {
        public NetworkCapabilities Network { get; protected set; }
        public AnalogCapabilities Analog { get; protected set; }

        public DeviceCapabilities(
            AnalogCapabilities analog,
            NetworkCapabilities network
            )
        {
            this.Network = network;
            this.Analog = analog;
        }
    }

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
                    _maxRawAdcVoltageValue = (int?)Math.Pow(2, (double)AdcResolution);
                    return _maxRawAdcVoltageValue;
                }
            }
        } protected int? _maxRawAdcVoltageValue;
    }

    public class NetworkCapabilities
    {
        public bool HasWiFi { get; protected set; }
        public bool HasEthernet { get; protected set; }

        public NetworkCapabilities(
            bool hasWifi,
            bool hasEthernet) {
            this.HasWiFi = hasWifi;
            this.HasEthernet = hasEthernet;
        }
    }
}

