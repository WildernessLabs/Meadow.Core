using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Meadow.Simulation
{
    public partial class SimulatedPinout : IPinDefinitions
    {
        private Dictionary<string, SimulatedPin> _pins = new Dictionary<string, SimulatedPin>();

        public IPinController Controller { get; set; }

        public SimulatedPinout()
        {
            _pins.Add("LED0", new SimulatedPin(
                Controller,
                "LED0", "LED0",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("LED0", inputCapable: false, interruptCapable: false)
                }
            ));

            _pins.Add("AI0", new SimulatedPin(
                Controller,
                "AI0", "AI0",
                new List<IChannelInfo> {
                    new AnalogChannelInfo("AI0", 12, true, false)
                }
            ));

            _pins.Add("AI1", new SimulatedPin(
                Controller,
                "AI1", "AI1",
                new List<IChannelInfo> {
                    new AnalogChannelInfo("AI0", 12, true, false)
                }
            ));

            _pins.Add("D00", new SimulatedPin(
                Controller,
                "D00", "D00",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("D00", interruptGroup: 0)
                }
            ));

            _pins.Add("D01", new SimulatedPin(
                Controller,
                "D01", "D01",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("D01", interruptGroup: 0)
                }
            ));
            _pins.Add("D02", new SimulatedPin(
                Controller,
                "D02", "D02",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("D02", interruptGroup: 0)
                }
            ));
            _pins.Add("D03", new SimulatedPin(
                Controller,
                "D03", "D03",
                new List<IChannelInfo> {
                    new DigitalChannelInfo("D03", interruptGroup: 0)
                }
            ));
        }

        public IList<IPin> AllPins => _pins.Values.Cast<IPin>().ToList();

        public IPin LED0 => _pins["LED0"];
        public IPin AI0 => _pins["AI0"];
        public IPin AI1 => _pins["AI1"];


        public IPin D00 => _pins["D00"];
        public IPin D01 => _pins["D01"];
        public IPin D02 => _pins["D02"];
        public IPin D03 => _pins["D03"];

        public IEnumerator<IPin> GetEnumerator() => AllPins.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

