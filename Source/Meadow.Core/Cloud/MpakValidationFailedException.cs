using System;

namespace Meadow;

/// <summary>
/// Exception thrown when validation of an mpak update package fails.
/// </summary>
public class MpakValidationFailedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MpakValidationFailedException"/> class with a specified error message describing the validation failure.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the validation failure.</param>
    public MpakValidationFailedException(string message) : base(message) { }
}
