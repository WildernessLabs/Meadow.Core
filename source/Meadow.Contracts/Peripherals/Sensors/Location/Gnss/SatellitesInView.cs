using System;
using System.Text;

namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    public class SatellitesInView : IGnssResult
    {
        /// <summary>
        /// The first two letters (after the starting delimiter) comprise the
        /// Talker ID, which describes the system in use, for instance "GL" means
        /// that the data came from the GLONASS system. "BD" means BeiDou, etc.
        ///
        /// Default value is "GP".
        /// </summary>
        public string TalkerID { get; set; } = "GP";

        /// <summary>
        /// Retreives the full name associated with the TalkerID via the
        /// `KnownTalkerIDs` property of the Lookups class.
        /// </summary>
        public string TalkerSystemName {
            get {
                string name = Lookups.KnownTalkerIDs[TalkerID];
                return (name != null) ? name : "";
            }
        }

        public Satellite[] Satellites { get; protected set; }

        public SatellitesInView(Satellite[] satellites)
        {
            this.Satellites = satellites;
        }

        public override string ToString()
        {
            StringBuilder outString = new StringBuilder();

            outString.Append("SatellitesInView: {\r\n");
            outString.Append($"\tTalker ID: {TalkerID}, talker name: {TalkerSystemName}\r\n");
            outString.Append($"\tSatellites:\r\n");
            foreach (var sat in Satellites) {
                outString.Append($"\t{sat}\r\n");
            }
            outString.Append("}");

            return outString.ToString();
        }
    }
}
