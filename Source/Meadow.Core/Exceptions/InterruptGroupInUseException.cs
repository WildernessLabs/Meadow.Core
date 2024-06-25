using System;

namespace Meadow.Hardware;

/// <summary>
/// Exception thrown when attempting to use an interrupt group that is already in use.
/// </summary>
public class InterruptGroupInUseException : Exception
{
    /// <summary>
    /// Gets the interrupt group that is already in use.
    /// </summary>
    public int Group { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InterruptGroupInUseException"/> class with the specified interrupt group.
    /// </summary>
    /// <param name="group">The interrupt group that is already in use.</param>
    public InterruptGroupInUseException(int group)
        : base($"Interrupt group {group} is already in use")
    {
        Group = group;
    }
}
