using System.Collections.Generic;
using Meadow.Hardware;
using Meadow.Gateway.WiFi;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents a Meadow F7 micro device. Includes device-specific IO mapping,
    /// capabilities and provides access to the various device-specific features.
    /// </summary>
    // TODO: Bryan: this is not my finest architecture, folks. Need to consider
    // some simplifications.
    public class F7Micro : IDevice
    {
        //public List<WiFiAdapter> WiFiAdapters { get; }

        public DeviceCapabilities Capabilities { get; protected set; }

        /// <summary>
        /// Gets the pins.
        /// </summary>
        /// <value>The pins.</value>
        public IPinDefinitions Pins => _pins;

        public IGPIOManager GPIOManager { get; protected set; }

        protected IPinDefinitions _pins;

        // private static
        static F7Micro() { }

        public F7Micro()
        {
            this.Capabilities = new DeviceCapabilities();
            //this.WiFiAdapters = new List<WiFiAdapter>
            //{
            //    // TODO: stuff.
            //    new WiFiAdapter()
            //};

            this.GPIOManager = new F7GPIOManager();
            this.GPIOManager.Initialize();

            // 
            _pins = new F7MicroPinDefinitions(GPIOManager);
        }


        public class F7MicroPinDefinitions : IPinDefinitions
        {
            public readonly AnalogPin A01 = AnalogChannels.A01;
            public readonly AnalogPin A02 = AnalogChannels.A02;
            public readonly AnalogPin A03 = AnalogChannels.A03;
            public readonly AnalogPin A04 = AnalogChannels.A04;
            public readonly AnalogPin A05 = AnalogChannels.A05;

            public readonly DigitalPin OnboardLEDRed = DigitalChannels.OnboardLEDRed;
            public readonly DigitalPin OnboardLEDGreen = DigitalChannels.OnboardLEDGreen;
            public readonly DigitalPin OnboardLEDBlue = DigitalChannels.OnboardLEDBlue;

            public readonly DigitalPin D00 = DigitalChannels.D00;
            public readonly DigitalPin D01 = DigitalChannels.D01;
            public readonly DigitalPin D02 = DigitalChannels.D02;
            public readonly DigitalPin D03 = DigitalChannels.D03;
            public readonly DigitalPin D04 = DigitalChannels.D04;
            public readonly DigitalPin D05 = DigitalChannels.D05;
            public readonly DigitalPin D06 = DigitalChannels.D06;
            public readonly DigitalPin D07 = DigitalChannels.D07;
            public readonly DigitalPin D08 = DigitalChannels.D08;
            public readonly DigitalPin D09 = DigitalChannels.D09;
            public readonly DigitalPin D10 = DigitalChannels.D10;
            public readonly DigitalPin D11 = DigitalChannels.D11;
            public readonly DigitalPin D12 = DigitalChannels.D12;
            public readonly DigitalPin D13 = DigitalChannels.D13;
            public readonly DigitalPin D14 = DigitalChannels.D14;
            public readonly DigitalPin D15 = DigitalChannels.D15;

            public F7MicroPinDefinitions(IGPIOManager manager)
            {
                OnboardLEDRed.GPIOManager = manager;
                OnboardLEDGreen.GPIOManager = manager;
                OnboardLEDBlue.GPIOManager = manager;

                D00.GPIOManager
                    = D01.GPIOManager
                    = D02.GPIOManager
                    = D03.GPIOManager
                    = D04.GPIOManager
                    = D05.GPIOManager
                    = D06.GPIOManager
                    = D07.GPIOManager
                    = D08.GPIOManager
                    = D09.GPIOManager
                    = D10.GPIOManager
                    = D11.GPIOManager
                    = D12.GPIOManager
                    = D13.GPIOManager
                    = D14.GPIOManager
                    = D15.GPIOManager
                    = manager;

                _allPins.Add(this.A01);
                _allPins.Add(this.A02);
                _allPins.Add(this.A03);
                _allPins.Add(this.A04);
                _allPins.Add(this.A05);
                _allPins.Add(this.D00);
                _allPins.Add(this.D01);
                _allPins.Add(this.D02);
                _allPins.Add(this.D03);
                _allPins.Add(this.D04);
                _allPins.Add(this.D05);
                _allPins.Add(this.D06);
                _allPins.Add(this.D07);
                _allPins.Add(this.D08);
                _allPins.Add(this.D09);
                _allPins.Add(this.D10);
                _allPins.Add(this.D11);
                _allPins.Add(this.D12);
                _allPins.Add(this.D13);
                _allPins.Add(this.D14);
                _allPins.Add(this.D15);
            }

            public IList<IPin> AllPins => _allPins;
            protected IList<IPin> _allPins = new List<IPin>();
        }

        // NOTE: this are split into three different classes in the chance that 
        // we decide to expose them as groups, as in;
        // Device.PwmChannels, Device.DigitalPins, etc.

        private static class DigitalChannels
        {
            // example for boards that have digital pins that don't have PWM timers
            // enabled on them.
            //public static readonly DigitalPin D99 = new DigitalPin("D99", 0x128);

            public static readonly DigitalPin OnboardLEDBlue = new DigitalPin("OnboardLEDBlue", "PA0");
            public static readonly DigitalPin OnboardLEDGreen = new DigitalPin("OnboardLEDGreen", "PA1");
            public static readonly DigitalPin OnboardLEDRed = new DigitalPin("OnboardLEDRed", "PA2");

            public static readonly DigitalPin D00 = new DigitalPin("D00", "PI9");
            public static readonly DigitalPin D01 = new DigitalPin("D01", "PH13");
            public static readonly DigitalPin D02 = new DigitalPin("D02", "PC6");
            public static readonly DigitalPin D03 = new DigitalPin("D03", "PB8");
            public static readonly DigitalPin D04 = new DigitalPin("D04", "PB9");
            public static readonly DigitalPin D05 = new DigitalPin("D05", "PC7");
            public static readonly DigitalPin D06 = new DigitalPin("D06", "PB0");
            public static readonly DigitalPin D07 = new DigitalPin("D07", "PB7");
            public static readonly DigitalPin D08 = new DigitalPin("D08", "PB6");
            public static readonly DigitalPin D09 = new DigitalPin("D09", "PB1");
            public static readonly DigitalPin D10 = new DigitalPin("D10", "PH10");
            public static readonly DigitalPin D11 = new DigitalPin("D11", "PC9");
            public static readonly DigitalPin D12 = new DigitalPin("D12", "PB14");
            public static readonly DigitalPin D13 = new DigitalPin("D13", "PB15");
            public static readonly DigitalPin D14 = new DigitalPin("D14", "PG3");
            public static readonly DigitalPin D15 = new DigitalPin("D15", "PE3");
        }

        private static class PWMChannels
        {
            //public static readonly PwmPin Pwm01 = new PwmPin("Pwm01", 0x128);
            //public static readonly PwmPin Pwm02 = new PwmPin("Pwm02", 0x256);
            //public static readonly PwmPin Pwm03 = new PwmPin("Pwm03", 0x256);
            //public static readonly PwmPin Pwm04 = new PwmPin("Pwm04", 0x256);
            //public static readonly PwmPin Pwm05 = new PwmPin("Pwm05", 0x256);
            //public static readonly PwmPin Pwm06 = new PwmPin("Pwm06", 0x256);
            //public static readonly PwmPin Pwm07 = new PwmPin("Pwm07", 0x256);
            //public static readonly PwmPin Pwm08 = new PwmPin("Pwm08", 0x256);
            //public static readonly PwmPin Pwm09 = new PwmPin("Pwm09", 0x256);
            //public static readonly PwmPin Pwm10 = new PwmPin("Pwm10", 0x256);
            //public static readonly PwmPin Pwm11 = new PwmPin("Pwm11", 0x256);
            //public static readonly PwmPin Pwm12 = new PwmPin("Pwm12", 0x256);
            //public static readonly PwmPin Pwm13 = new PwmPin("Pwm13", 0x256);
            //public static readonly PwmPin Pwm14 = new PwmPin("Pwm14", 0x256);
            //public static readonly PwmPin Pwm15 = new PwmPin("Pwm15", 0x256);
            public static readonly PwmPin OnboardLEDRed = new PwmPin("Pwm01", "PA2");
            public static readonly PwmPin OnboardLEDGreen = new PwmPin("Pwm01", "PA1");
            public static readonly PwmPin OnboardLEDBlue = new PwmPin("Pwm01", "PA0");
        }

        private static class AnalogChannels
        {
            public static readonly AnalogPin A01 = new AnalogPin("A01", 0x0);
            public static readonly AnalogPin A02 = new AnalogPin("A02", 0x0);
            public static readonly AnalogPin A03 = new AnalogPin("A03", 0x0);
            public static readonly AnalogPin A04 = new AnalogPin("A04", 0x0);
            public static readonly AnalogPin A05 = new AnalogPin("A05", 0x0);
        }


    }

}
