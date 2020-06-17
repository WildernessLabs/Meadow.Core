using System;
using System.Collections.Generic;
using Meadow.Utilities;

namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    public class NmeaSentence
    {
        public string Prefix { get; set; }
        public List<string> DataElements { get; set; } = new List<string>();
        public byte Checksum {
            get { return ChecksumCalculator.XOR(GetDataString()); }
        }

        protected string GetDataString()
        {
            return Prefix + String.Join(",", DataElements);
        }

        public override string ToString()
        {
            string data = GetDataString(); //don't want to calculate twice
            return data + "*" + ChecksumCalculator.XOR(data);
        }

        public bool DebugMode { get; set; } = true;

        public NmeaSentence()
        {

        }

        public static NmeaSentence From(string sentence)
        {
            NmeaSentence newSentence = new NmeaSentence();

            if (string.IsNullOrWhiteSpace(sentence)) {
                throw new ArgumentException("Empty sentence. Nothing to parse.");
            }

            var checksumLocation = sentence.LastIndexOf('*');
            if (checksumLocation > 0) {
                // extract the data from the sentence
                var checksumString = sentence.Substring(checksumLocation + 1);
                var messageData = sentence.Substring(0, checksumLocation);
                byte parsedChecksum;
                // calculate the checksum (have to remove the first character, the "$")
                byte calculatedChecksum = ChecksumCalculator.XOR(messageData.Substring(1));
                try {
                    parsedChecksum = Convert.ToByte(checksumString.Trim(), 16);
                } catch (Exception e) {
                    throw new ArgumentException($"Checksum failed to parse, error: {e.Message}");
                }

                //if (DebugMode) {
                    //Console.WriteLine($"checksum in NMEA:'{checksumString}'");
                    //Console.WriteLine($"parsed checksum:{parsedChecksum:x}");
                    //Console.WriteLine($"actualData:{messageData}");
                    //Console.WriteLine($"Checksum match? {(calculatedChecksum == parsedChecksum ? "yes" : "no")}");
                //}

                // make sure data is good
                if (calculatedChecksum == parsedChecksum) {
                    // split the sentence data up by commas
                    var elements = messageData.Split(',').AsSpan<string>();
                    if (elements.Length <= 0) {
                        throw new ArgumentException("No data in sentence.");
                    }
                    // store the data
                    newSentence.Prefix = elements[0];
                    newSentence.DataElements.Clear();
                    newSentence.DataElements.AddRange(elements.Slice(1).ToArray());
                } else {
                    throw new ArgumentException("Checksum does not match data. Invalid data.");
                }
            } else {
                throw new ArgumentException("No checksum found. Invalid data.");
            }
            return newSentence;
        }
    }
}
