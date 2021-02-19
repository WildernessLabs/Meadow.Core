using System.Net;
using Meadow.Gateway.WiFi;
using System.Collections.ObjectModel;
using System;
using System.Threading.Tasks;

namespace Meadow.Gateway
{
    public interface ICoprocessor
    {
        #region Enums

        /// <summary>
        /// State of the ESP32 coprocessor.
        /// </summary>
        enum CoprocessorState
        {
            /// <summary>
            /// Coprocessor is not ready or has not been detected.
            /// </summary>
            NotReady,

            /// <summary>
            /// Coprocessor is available.
            /// </summary>
            Ready,

            /// <summary>
            /// Coprocessor is available but is currently sleeping.
            /// </summary>
            Sleeping
        }

        #endregion Enums

        #region Properties

        /// <summary>
        /// Current status of the coprocessor.
        /// </summary>
        CoprocessorState Status { get; }

        /// <summary>
        /// Battery charge level in millivolts.
        /// </summary>
        UInt32 BatteryLevel { get; }

        #endregion Properties
    }
}