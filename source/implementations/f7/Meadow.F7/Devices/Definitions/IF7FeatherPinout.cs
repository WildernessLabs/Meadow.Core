namespace Meadow.Hardware
{
    public interface IF7FeatherPinout : I32PinFeatherBoardPinout, IEspCoprocessorPinout, IPinDefinitions
    {
        //==== LED
        IPin OnboardLedBlue { get; }
        IPin OnboardLedGreen { get; }
        IPin OnboardLedRed { get; }
    }
}
