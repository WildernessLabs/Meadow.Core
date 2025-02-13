namespace Meadow.Hardware;

/// <summary>
/// Defines the contract for a Meadow IF7Feather device's pinout
/// </summary>
public interface IF7FeatherPinout : I32PinFeatherBoardPinout, IEspCoprocessorPinout, IPinDefinitions
{
    /// <summary>
    /// The pin for the on-board blue LED
    /// </summary>
    IPin OnboardLedBlue { get; }
    /// <summary>
    /// The pin for the on-board green LED
    /// </summary>
    IPin OnboardLedGreen { get; }
    /// <summary>
    /// The pin for the on-board red LED
    /// </summary>
    IPin OnboardLedRed { get; }
}
