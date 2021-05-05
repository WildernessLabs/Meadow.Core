using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Provides data for events that come from an IDigitalInputPort.
    /// </summary>
    //public class DigitalInputPortEventArgs : EventArgs, IChangeResult<DateTime>
    public class DigitalInputPortEventArgs : EventArgs, ITimeChangeResult
    {
        public bool Value { get; set; }
        public DateTime New { get; set; }
        public DateTime? Old { get; set; }

        public TimeSpan? Delta { get { return New - Old; } }
        //public DateTime Delta { get { return DateTime.MinValue.Add(New - Old); } }

        public DigitalInputPortEventArgs() { }

        public DigitalInputPortEventArgs(bool value, DateTime time, DateTime previous) {
            this.Value = value;
            this.New = time;
            this.Old = previous;
        }
    }
}