using System;
using System.Collections.Generic;
using System.Diagnostics;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace F7_Micro_Board_Diagnostics
{
    public class F7MicroDiagApp : App<F7Micro, F7MicroDiagApp>
    {
        public F7MicroDiagApp()
        {
            // Simple Digital IO
            Tuple<bool, List<PortTestResult>> digitalIOResults = TestDigitalIO();
            Debug.WriteLine("Simple Digital IO Test Results: " + (digitalIOResults.Item1 ? "PASS" : "FAIL"));
            if (!digitalIOResults.Item1) {
                foreach (var r in digitalIOResults.Item2) {
                    Debug.WriteLineIf(!r.Result, "Port failure on pin: " + r.PortName);
                }
            }
        }

        protected bool TestNetwork() { return false; }

        protected bool TestBluetooth() { return false; }

        /// <summary>
        /// Tests the digital IO ports by performing write/reads on paired ports
        /// and seeing if nothing is shorted.
        /// </summary>
        /// <returns>Whether or not the digital IO passed.</returns>
        protected Tuple<bool, List<PortTestResult>> TestDigitalIO()
        {
            List<PortTestResult> portTestResults = new List<PortTestResult>();
            bool success = true;

            // all the digitio IO pins to test
            List<IBiDirectionalPort> testDigitalPorts = new List<IBiDirectionalPort> {
                Device.CreateBiDirectionalPort(Device.Pins.D00, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D01, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D02, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D03, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D04, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D05, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D06, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D07, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D08, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D09, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D10, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D11, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D12, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D13, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D14, resistorMode : ResistorMode.PullDown),
                Device.CreateBiDirectionalPort(Device.Pins.D15, resistorMode : ResistorMode.PullDown)
            };

            // ports that are connected together
            List<Tuple<IBiDirectionalPort, IBiDirectionalPort>> testPairs = new List<Tuple<IBiDirectionalPort, IBiDirectionalPort>> {
                new Tuple<IBiDirectionalPort, IBiDirectionalPort>(testDigitalPorts[0], testDigitalPorts[8]),
                new Tuple<IBiDirectionalPort, IBiDirectionalPort>(testDigitalPorts[1], testDigitalPorts[9]),
                new Tuple<IBiDirectionalPort, IBiDirectionalPort>(testDigitalPorts[2], testDigitalPorts[10]),
                new Tuple<IBiDirectionalPort, IBiDirectionalPort>(testDigitalPorts[3], testDigitalPorts[11]),
                new Tuple<IBiDirectionalPort, IBiDirectionalPort>(testDigitalPorts[4], testDigitalPorts[12]),
                new Tuple<IBiDirectionalPort, IBiDirectionalPort>(testDigitalPorts[5], testDigitalPorts[13]),
                new Tuple<IBiDirectionalPort, IBiDirectionalPort>(testDigitalPorts[6], testDigitalPorts[14]),
                new Tuple<IBiDirectionalPort, IBiDirectionalPort>(testDigitalPorts[7], testDigitalPorts[15]),
            };

            // Test all pins:
            for (int i = 0; i < testDigitalPorts.Count; i++) {

                // turn all pins low except for the one that's driving a high signal
                foreach (var port in testDigitalPorts) {
                    // turn them all input and pulled down, unless it's the high test pin
                    if (port == testDigitalPorts[i]) {
                        port.Direction = PortDirectionType.Input;
                    } else {
                        port.Direction = PortDirectionType.Output;
                        port.State = true;
                    }
                }

                // get a reference to the paired endpoint pin (the pin on the 
                // other side of the high test pin).
                IBiDirectionalPort pairedEndpointPort;
                if (i < 8) {
                    pairedEndpointPort = testPairs[i].Item2;
                } else {
                    pairedEndpointPort = testPairs[i].Item1;
                }

                // loop through all the ports and check to see if they're reading 
                // low except for the other end of the high pin.
                bool portSuccess = true;
                foreach (var port in testDigitalPorts) {

                    // if we're not the high port, or the paired port
                    if (port != testDigitalPorts[i] && port != pairedEndpointPort) {
                        if (port.State) {
                            // FAILURE: if this port is high, something is wrong.
                            success = portSuccess = false;
                            portTestResults.Add(new PortTestResult("", false)); // TODO: Name
                            Debug.WriteLine("Port failure on pin: " + port.Pin.Name + ", channel: " + port.Channel.Name + "; should be LOW, but is HIGH. Short detected.");
                        }
                    } // if it's the port on the other 
                    else if (port != testDigitalPorts[i] && port == pairedEndpointPort) {
                        if (!port.State) {
                            success = portSuccess = false;
                            portTestResults.Add(new PortTestResult("", false)); // TODO: Name
                            Debug.WriteLine("Port failure on pin: " + port.Pin.Name + ", channel: " + port.Channel.Name + ";  [name] pair should be HIGH, but it's LOW. Endpoint port read failure.");
                        }
                    }

                    if (portSuccess) {
                        Debug.WriteLine("port " + port.Pin.Name + ", channel: " + port.Channel.Name + " test success.");
                    }
                }
            }

            // report results.
            return new Tuple<bool, List<PortTestResult>>(success, portTestResults);
        }


        protected bool TestAnalogIO() { return false; }

    }
}
