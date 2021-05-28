using System;
using System.Collections.Generic;

namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    /// <summary>
    /// Repository of lookup information, such as Known Talker IDs for GNSS
    /// system information.
    /// </summary>
    public static class Lookups
    {
        public static Dictionary<string, string> KnownTalkerIDs { get; } = new Dictionary<string, string>();

        static Lookups()
        {
            PopulateKnownTalkerIDs();
        }

        private static void PopulateKnownTalkerIDs()
        {
            KnownTalkerIDs.Add("BD", "BeiDou");
            KnownTalkerIDs.Add("CD", "Digital Selective Calling (DSC)");
            KnownTalkerIDs.Add("EC", "Electronic Chart Display & Information System (ECDIS)");
            KnownTalkerIDs.Add("GA", "Galileo Positioning System");
            KnownTalkerIDs.Add("GB", "BeiDou");
            KnownTalkerIDs.Add("GL", "GLONASS");
            KnownTalkerIDs.Add("GN", "Combination of multiple satellite systems.");
            KnownTalkerIDs.Add("GP", "Global Positioning System receiver");
            KnownTalkerIDs.Add("II", "Integrated Instrumentation");
            KnownTalkerIDs.Add("IN", "Integrated Navigation");
            KnownTalkerIDs.Add("LC", "Loran-C receiver (obsolete)");
            KnownTalkerIDs.Add("QZ", "QZSS regional GPS augmentation system");
            KnownTalkerIDs.Add("GI", "NavIC (IRNSS)");

        }
    }
}
