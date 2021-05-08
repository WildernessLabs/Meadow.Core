using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides data for events that come from an IDigitalInputPort.
    /// </summary>
    public struct DigitalInputPortChangeResult : IChangeResult<DateTime>
    {
        /// <summary>
        /// The state of the port at the time of the event or notification.
        /// `true` == `HIGH`. `false` == `LOW`
        /// </summary>
        public bool Value { get; set; }
        /// <summary>
        /// The time at the event or notification.
        /// </summary>
        public DateTime New { get; set; }
        /// <summary>
        /// The last time the event was raised or the notification sent.
        /// </summary>
        public DateTime? Old { get; set; }

        /// <summary>
        /// The duration of time in between the time the event or notification
        /// ocurred, and the the time it occured before.
        /// </summary>
        public TimeSpan? Delta { get { return New - Old; } }

        /// <summary>
        /// Creates a new DigitalInputPortChangeResult.
        /// </summary>
        /// <param name="value">The state of the port, `true` == `HIGH`. `false` == `LOW`</param>
        /// <param name="time">The time the event occured.</param>
        /// <param name="previous">The time that the previous event occured.</param>
        public DigitalInputPortChangeResult(bool value, DateTime time, DateTime previous) {
            this.Value = value;
            this.New = time;
            this.Old = previous;
        }
    }
}