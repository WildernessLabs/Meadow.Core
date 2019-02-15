using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meadow.Hardware
{
    public class AnalogInputSampleResult
    {
        public float NewValue { get; set; }
        public float ChangedValue { get; set; }
        public float PreviousValue { get; set; }
        public float PercentageChanged { get; set; }
    }
}
