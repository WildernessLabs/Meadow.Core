using System;

namespace Meadow.Hardware
{
    //TODO delete this class.
    public static class Cpu
    {
        public static TimeSpan GlitchFilterTime { get; set; }
        //public static uint SlowClock { get; }
        //public static uint SystemClock { get; }

        //public enum AnalogChannel
        //{
        //    ANALOG_NONE = -1,
        //    ANALOG_0 = 0,
        //    ANALOG_1 = 1,
        //    ANALOG_2 = 2,
        //    ANALOG_3 = 3,
        //    ANALOG_4 = 4,
        //    ANALOG_5 = 5,
        //    ANALOG_6 = 6,
        //    ANALOG_7 = 7
        //}
        //public enum AnalogOutputChannel
        //{
        //    ANALOG_OUTPUT_NONE = -1,
        //    ANALOG_OUTPUT_0 = 0,
        //    ANALOG_OUTPUT_1 = 1,
        //    ANALOG_OUTPUT_2 = 2,
        //    ANALOG_OUTPUT_3 = 3,
        //    ANALOG_OUTPUT_4 = 4,
        //    ANALOG_OUTPUT_5 = 5,
        //    ANALOG_OUTPUT_6 = 6,
        //    ANALOG_OUTPUT_7 = 7
        //}
        //public enum Pin
        //{
        //    GPIO_NONE = -1,
        //    GPIO_Pin0 = 0,
        //    GPIO_Pin1 = 1,
        //    GPIO_Pin2 = 2,
        //    GPIO_Pin3 = 3,
        //    GPIO_Pin4 = 4,
        //    GPIO_Pin5 = 5,
        //    GPIO_Pin6 = 6,
        //    GPIO_Pin7 = 7,
        //    GPIO_Pin8 = 8,
        //    GPIO_Pin9 = 9,
        //    GPIO_Pin10 = 10,
        //    GPIO_Pin11 = 11,
        //    GPIO_Pin12 = 12,
        //    GPIO_Pin13 = 13,
        //    GPIO_Pin14 = 14,
        //    GPIO_Pin15 = 15
        //}
        [Flags]
        public enum PinUsage : byte
        {
            NONE = 0,
            INPUT = 1,
            OUTPUT = 2,
            ALTERNATE_A = 4,
            ALTERNATE_B = 8
        }
        [Flags]
        public enum PinValidInterruptMode : byte
        {
            NONE = 0,
            InterruptEdgeLow = 2,
            InterruptEdgeHigh = 4,
            InterruptEdgeBoth = 8,
            InterruptEdgeLevelHigh = 16,
            InterruptEdgeLevelLow = 32
        }
        [Flags]
        public enum PinValidResistorMode : byte
        {
            NONE = 0,
            Disabled = 1,
            PullUp = 2,
            PullDown = 4
        }
        public enum PWMChannel
        {
            PWM_NONE = -1,
            PWM_0 = 0,
            PWM_1 = 1,
            PWM_2 = 2,
            PWM_3 = 3,
            PWM_4 = 4,
            PWM_5 = 5,
            PWM_6 = 6,
            PWM_7 = 7
        }
    }
}
