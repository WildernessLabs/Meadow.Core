using System;
namespace Meadow.Hardware
{
    //TODO: should this inherit from ChangeResult<DateTime> and not eventargs?
    // eventargs is a class, and our change results are structs.
    /// <summary>
    /// Provides data for events that come from an IDigitalInputPort.
    /// </summary>
    public struct DigitalInputPortChangeResult : IChangeResult<DateTime>
    {
        public bool Value { get; set; }
        public DateTime New { get; set; }
        public DateTime? Old { get; set; }

        public TimeSpan? Delta { get { return New - Old; } }

        //public DigitalInputPortEventArgs() { }

        public DigitalInputPortChangeResult(bool value, DateTime time, DateTime previous) {
            this.Value = value;
            this.New = time;
            this.Old = previous;
        }
    }
}