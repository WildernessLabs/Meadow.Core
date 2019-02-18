using System;
namespace F7_Micro_Board_Diagnostics
{
    public class PortTestResult
    {
        public string PortName { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; }

        public PortTestResult(string pinName, bool result, string message = "")
        {
            PortName = pinName;
            Result = result;
            Message = message;
        }
    }
}
