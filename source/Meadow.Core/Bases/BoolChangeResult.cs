using System;

namespace Meadow
{
    public class BoolChangeResult
    {
        public BoolChangeResult(bool value, DateTime changedOn)
        {
            this.Value = value;
            this.ChangedOn = changedOn;
        }

        public bool Value { get; set; }
        public DateTime ChangedOn { get; set; }
    }
}
