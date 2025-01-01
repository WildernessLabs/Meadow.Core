using System;
using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            public enum AnalyzerType
            {
                NoDutyCycle = 1,
                WithDutyCycle = 2,
                Unconfigure = 1
            }

            [StructLayout(LayoutKind.Explicit)]
            public struct AnalyzerConfig
            {
                [FieldOffset(0)]
                public int TimerNumber;
                [FieldOffset(4)]
                public int ChannelNumber;
                [FieldOffset(8)]
                public int PortAndPin;
                [FieldOffset(12)]
                public AnalyzerType Type;

                public int Port
                {
                    set
                    {
                        PortAndPin &= 0x0f;
                        PortAndPin |= (value << 4);
                    }
                }
                public int Pin
                {
                    set
                    {
                        if (value > 15) throw new ArgumentException();
                        PortAndPin &= 0xf0;
                        PortAndPin |= value;
                    }
                }
            }

            [StructLayout(LayoutKind.Explicit)]
            public struct AnalyzerData
            {
                [FieldOffset(0)]
                public int TimerNumber;
                [FieldOffset(4)]
                public int ChannelNumber;
                [FieldOffset(8)]
                public int Frequency1k;
                [FieldOffset(12)]
                public int DutyCycle1k;
                [FieldOffset(16)]
                public int AvgFrequency1k;
                [FieldOffset(20)]
                public int GpioCount;
            }

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern AnalyzerCallStatus meadow_measure_freq_configure(ref AnalyzerConfig config);

            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern AnalyzerCallStatus meadow_measure_freq_return_freq_info(ref AnalyzerData config);

            public enum AnalyzerCallStatus
            {
                // configure results
                ConfigureSuccess = (10),
                UndefinedOption = (-1),
                IllegalTimerNumber = (-2),
                IllegalChannelNumber = (-3),
                BadTimerForPortAndPin = (-4),
                BadChannelForPortAndPin = (-5),
                TimerNotAvailable = (-6),
                ChannelInUse = (-7),
                MemoryAllocationFailed = (-8),
                GpioConfigError = (-9),
                InitChannelHardwareFailed = (-10),
                InitTimerHardwareFailed = (-11),
                // READ results
                ReadSuccess = (20),
                NoChannelActivity = (-21),
                InvalidTimerNumber = (-22),
                InvalidChannelNumber = (-23),
                TimerAccessNull = (-24),
                ChannelNotConfigured = (-25),
                ChannelDataNull = (-26),
                NoActiveChannels = (-27),
                FrequencyInputNotDetected = (-28),
                // unconfigure results
                UnconfigureSUCCESSFUL = (30),
                UnconfigureINVALID_TIMER_NUMB = (-31),
                UnconfigureINVALID_CHANNEL_NUMB = (-32),
                UnconfigureTIMER_ACCESS_NULL = (-33),
                UnconfigureCHAN_NOT_CONFIG = (-34),
                UnconfigureNO_CHANNEL = (-35),
                UnconfigureIRQ_DETACH_ERR = (-36),
            }

            /*
            int meadow_measure_freq_configure(mdwFreqCfgTimer_t* mdwCfgTimerChan);
            int meadow_measure_freq_return_freq_info(mdwFreqReturnData_t* mdwFreqReturnData);

            #define MEADOW_MEAS_FREQ_CONF_OPTION_NO_DC      (1)
            #define MEADOW_MEAS_FREQ_CONF_OPTION_WITH_DC    (2)
            #define MEADOW_MEAS_FREQ_CONF_OPTION_UNCFG      (3)

            struct mdwFreqCfgTimer_s
            {
              uint32_t timerNumber;   // 1-14 (1 & 8 not supported)
              uint32_t channelNumber; // 1-4
              uint32_t portAndPin;    // bits 7:4 port 0=A, 1=B & bits 3:0 pin 0-15
              uint32_t configOption;  // See below
              // 0 = illegal - Must set configOption to 1-3
              // 1 = Configure without Duty Cycle,
              // 2 = Configure with Duty Cycle (twice the interrupts),
              // 3 = Unconfigure
            };
            typedef struct mdwFreqCfgTimer_s mdwFreqCfgTimer_t;

            struct mdwFreqReturnData_s
            {
              uint32_t timerNumber;       // The timer number 1-14
              uint32_t channelNumber;     // The channel number 1-4
              uint32_t frequencyX1000;    // Frequency * 1000
              uint32_t dutyCycleX1000;    // Duty Cycle * 1000
              uint32_t avgFreqX1000;      // Average frequency * 1000
              uint32_t gpioCountForAvg;   // Number of transitions since list read
            };
            typedef struct mdwFreqReturnData_s mdwFreqReturnData_t;

            // Define values returned by measurement code
            #define MEADOW_MEAS_FREQ_CONF_SUCCESSFUL                (10)
            #define MEADOW_MEAS_FREQ_CONF_UNDEFINED_OPTION          (-1)
            #define MEADOW_MEAS_FREQ_CONF_TIM_NUMB_ILLEGAL          (-2)
            #define MEADOW_MEAS_FREQ_CONF_CHAN_NUMB_ILLEGAL         (-3)
            #define MEADOW_MEAS_FREQ_CONF_PORT_PIN_NOT_FOR_TIM      (-4)
            #define MEADOW_MEAS_FREQ_CONF_PORT_PIN_TIM_CHAN_INVALID (-5)
            #define MEADOW_MEAS_FREQ_CONF_TIM_NOT_USABLE            (-6)
            #define MEADOW_MEAS_FREQ_CONF_TIM_CHAN_IN_USE           (-7)
            #define MEADOW_MEAS_FREQ_CONF_CHAN_MEM_ALLOC_FAILED     (-8)
            #define MEADOW_MEAS_FREQ_CONF_CONFIGGPIO_ERR            (-9)
            #define MEADOW_MEAS_FREQ_CONF_INIT_CHAN_HW_FAIL         (-10)
            #define MEADOW_MEAS_FREQ_CONF_INIT_TIM_HW_FAIL          (-11)

            #define MEADOW_MEAS_FREQ_READ_SUCCESSFUL                (20)
            #define MEADOW_MEAS_FREQ_READ_NO_CHANS_ACTIVITY         (-21)
            #define MEADOW_MEAS_FREQ_READ_INVALID_TIMER_NUMB        (-22)
            #define MEADOW_MEAS_FREQ_READ_INVALID_CHANNEL_NUMB      (-23)
            #define MEADOW_MEAS_FREQ_READ_TIMER_ACCESS_NULL         (-24)
            #define MEADOW_MEAS_FREQ_READ_CHAN_NOT_CONFIG           (-25)
            #define MEADOW_MEAS_FREQ_READ_CHANNEL_DATA_NULL         (-26)
            #define MEADOW_MEAS_FREQ_READ_NO_CHANS_ACTIVE           (-27)
            #define MEADOW_MEAS_FREQ_READ_FREQ_INPUT_NOT_DETECTED   (-28)

            #define MEADOW_MEAS_FREQ_UNCFG_SUCCESSFUL               (30)
            #define MEADOW_MEAS_FREQ_UNCFG_INVALID_TIMER_NUMB       (-31)
            #define MEADOW_MEAS_FREQ_UNCFG_INVALID_CHANNEL_NUMB     (-32)
            #define MEADOW_MEAS_FREQ_UNCFG_TIMER_ACCESS_NULL        (-33)
            #define MEADOW_MEAS_FREQ_UNCFG_CHAN_NOT_CONFIG          (-34)
            #define MEADOW_MEAS_FREQ_UNCFG_NO_CHANNEL               (-35)
            #define MEADOW_MEAS_FREQ_UNCFG_IRQ_DETACH_ERR           (-36)

            */
        }
    }
}
