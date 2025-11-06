using static Meadow.Gpiod2.Interop;

namespace Meadow;

internal static class GpiodExtensions
{
    public static Gpiod3.Interop.gpiod_line_bias AsGpiod3LineBias(this GpiodLineBias bias)
    {
        return bias switch
        {
            GpiodLineBias.Disabled => Gpiod3.Interop.gpiod_line_bias.GPIOD_LINE_BIAS_DISABLED,
            GpiodLineBias.PullUp => Gpiod3.Interop.gpiod_line_bias.GPIOD_LINE_BIAS_PULL_UP,
            GpiodLineBias.PullDown => Gpiod3.Interop.gpiod_line_bias.GPIOD_LINE_BIAS_PULL_DOWN,
            _ => Gpiod3.Interop.gpiod_line_bias.GPIOD_LINE_BIAS_AS_IS
        };
    }
    public static Gpiod2.Interop.line_request_flags AsGpiod2Flags(this GpiodLineBias bias)
    {
        return bias switch
        {
            GpiodLineBias.Disabled => line_request_flags.GPIOD_LINE_REQUEST_FLAG_BIAS_DISABLE,
            GpiodLineBias.PullUp => line_request_flags.GPIOD_LINE_REQUEST_FLAG_BIAS_PULL_UP,
            GpiodLineBias.PullDown => line_request_flags.GPIOD_LINE_REQUEST_FLAG_BIAS_PULL_DOWN,
            _ => 0 // AsIs = no flags
        };
    }
}
