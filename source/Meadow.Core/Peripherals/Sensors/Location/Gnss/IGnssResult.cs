using System;
namespace Meadow.Peripherals.Sensors.Location.Gnss
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGnssResult 
    {
        ///// <summary>
        ///// Time the reading was generated.
        ///// </summary>
        //DateTime TimeOfReading { get; set; }


        // TODO: when we switch to .NET Core, put the implementation here with a
        // default value of "GP";
        /// <summary>
        /// The first two letters (after the starting delimiter) comprise the
        /// Talker ID, which describes the system in use, for instance "GL" means
        /// that the data came from the GLONASS system. "BD" means BeiDou, etc.
        ///
        /// Default value is "GP".
        /// </summary>
        // public string TalkerID { get; set; } = "GP";
        string TalkerID { get; set; }

        // TODO: implementation @ core switchover
        ///// <summary>
        ///// Retreives the full name associated with the TalkerID via the
        ///// `KnownTalkerIDs` property of the Lookups class.
        ///// </summary>
        //public string TalkerSystemName {
        //    get {
        //        string name = Lookups.KnownTalkerIDs[TalkerID];
        //        return (name != null) ? name : "";
        //    }
        //}

        /// <summary>
        /// Retreives the full name associated with the TalkerID via the
        /// `KnownTalkerIDs` property of the Lookups class.
        /// </summary>
        public string TalkerSystemName { get; }

    }
}
