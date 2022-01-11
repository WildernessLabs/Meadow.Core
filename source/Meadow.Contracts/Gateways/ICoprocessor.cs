namespace Meadow.Gateways
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

        /// <summary>
        /// Indicate the reason the coprocessor was last restarted.
        /// </summary>
        /// <remarks>The codes match those returned by the coprocessor.</remarks>
        enum CoprocessorResetReason
        {
            /// <summary>
            /// Reason not specified or unknown.  Could also indicate that the coprocessor has not started.
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// Normal power on.
            /// </summary>
            PowerOn = 1,

            /// <summary>
            /// External GPIO has woken the coprocessof from sleep.
            /// </summary>
            ExternalGpio = 2,

            /// <summary>
            /// Coprocessor firmware requested a reset.
            /// </summary>
            Software = 3,

            /// <summary>
            /// Abnormal reset through software error.
            /// </summary>
            Panic = 4,

            /// <summary>
            /// Interrupt watchdog has been triggered.
            /// </summary>
            InterruptWatchdog = 5,

            /// <summary>
            /// Task watchdog has been triggered.
            /// </summary>
            TaskWatchdog = 6,

            /// <summary>
            /// Some other watchdog was triggered.
            /// </summary>
            OtherWatchdog = 7,

            /// <summary>
            /// Exiting from deep sleep.
            /// </summary>
            DeepSleep = 8,

            /// <summary>
            /// Brownout (low power condition).
            /// </summary>
            Brownout = 9,

            /// <summary>
            /// SDIO requested a reset.
            /// </summary>
            SDIO = 10
        }

        #endregion Enums

        #region Properties

        /// <summary>
        /// Current status of the coprocessor.
        /// </summary>
        CoprocessorState Status { get; }

        /// <summary>
        /// Reason for the last reset of the coprocessor.
        /// </summary>
        CoprocessorResetReason ResetReason { get; }

        /// <summary>
        /// Gets the current battery charge level in Volts (`V`).
        /// </summary>
        double GetBatteryLevel();

        #endregion Properties
    }
}