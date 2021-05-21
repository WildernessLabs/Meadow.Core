using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a snapshot of the state of a digital port at a given time.
    /// </summary>
    public struct DigitalState
    {
        /// <summary>
        /// The state of the port at the time of the event or notification.
        /// `true` == `HIGH`. `false` == `LOW`
        /// </summary>
        public bool State { get; set; }
        /// <summary>
        /// The time at the event or notification.
        /// </summary>
        public DateTime Time { get; set; }

        public DigitalState(bool state, DateTime time)
        {
            State = state;
            Time = time;
        }
    }
}
