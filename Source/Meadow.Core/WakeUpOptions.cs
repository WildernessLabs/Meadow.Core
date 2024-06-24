using Meadow.Hardware;
using System;
using System.Collections.Generic;

namespace Meadow;

/// <summary>
/// Options for waking a device passed to Sleep calls
/// </summary>
public class WakeUpOptions
{
    /// <summary>
    /// Sleep until a specific time
    /// </summary>
    public DateTime SleepUntil { get; set; }
    /// <summary>
    /// Sleep for a specific duration
    /// </summary>
    public TimeSpan SleepDuration { get; set; }
    /// <summary>
    /// Sleep until network traffic is detected
    /// </summary>
    public bool WakeOnNetwork { get; set; }
    /// <summary>
    /// Sleep until an interrupt occurs on a specific port
    /// </summary>
    public List<IDigitalInterruptPort> WakeOnInterruptPorts { get; } = new List<IDigitalInterruptPort>();

    /// <summary>
    /// Creates a set of wake options
    /// </summary>
    public WakeUpOptions()
    {
    }
}
