using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides data for events that come from an IDigitalInputPort.
    /// </summary>
    public struct DigitalInputPortChangeResult : IChangeResult<DateTime>
    {
        public bool Value { get; set; }
        public DateTime New { get; set; }
        public DateTime? Old { get; set; }

        public TimeSpan? Delta { get { return New - Old; } }

        public DigitalInputPortChangeResult(bool value, DateTime time, DateTime previous) {
            this.Value = value;
            this.New = time;
            this.Old = previous;
        }
    }
}