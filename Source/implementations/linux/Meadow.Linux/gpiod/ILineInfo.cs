using Meadow.Hardware;

namespace Meadow;

/// <summary>
/// Bias settings for GPIO lines (unified across v2 and v3)
/// </summary>
public enum GpiodLineBias
{
    /// <summary>Don't change bias</summary>
    AsIs,
    /// <summary>Disable internal bias</summary>
    Disabled,
    /// <summary>Enable pull-up resistor</summary>
    PullUp,
    /// <summary>Enable pull-down resistor</summary>
    PullDown
}

/// <summary>
/// Interface for GPIO line information across libgpiod versions
/// </summary>
internal interface ILineInfo
{
    /// <summary>
    /// Line name (may be empty)
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Consumer name (application using this line)
    /// </summary>
    string Consumer { get; }

    /// <summary>
    /// Line offset within the chip
    /// </summary>
    int Offset { get; }

    /// <summary>
    /// Whether this line handle is invalid
    /// </summary>
    bool IsInvalid { get; }

    /// <summary>
    /// Request this line for output with specified bias and initial state
    /// </summary>
    bool RequestOutput(GpiodLineBias bias, bool initialState);

    /// <summary>
    /// Request this line for input with specified bias
    /// </summary>
    void RequestInput(GpiodLineBias bias);

    /// <summary>
    /// Request this line for interrupts with edge detection
    /// </summary>
    void RequestInterrupts(InterruptMode mode, GpiodLineBias bias);

    /// <summary>
    /// Release this line
    /// </summary>
    void Release();

    /// <summary>
    /// Set the output value of this line
    /// </summary>
    void SetValue(bool state);

    /// <summary>
    /// Get the input value of this line
    /// </summary>
    bool GetValue();
}
