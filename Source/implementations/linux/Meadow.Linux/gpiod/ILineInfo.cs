using Meadow.Hardware;
using System;

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
/// Edge event type for GPIO interrupts
/// </summary>
public enum GpiodEdgeEventType
{
    /// <summary>Rising edge detected</summary>
    Rising,
    /// <summary>Falling edge detected</summary>
    Falling
}

/// <summary>
/// Event arguments for GPIO line edge events (unified across v2 and v3)
/// </summary>
public class GpiodEdgeEventArgs : EventArgs
{
    /// <summary>Type of edge detected</summary>
    public GpiodEdgeEventType EventType { get; set; }

    /// <summary>Timestamp in nanoseconds</summary>
    public ulong TimestampNs { get; set; }

    /// <summary>Line offset that triggered the event</summary>
    public int LineOffset { get; set; }
}

/// <summary>
/// Delegate for line edge events
/// </summary>
internal delegate void LineEdgeEventHandler(ILineInfo sender, GpiodEdgeEventArgs args);

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
    /// Event fired when an edge interrupt occurs on this line
    /// </summary>
    event LineEdgeEventHandler InterruptOccurred;

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
