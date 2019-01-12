using Meadow.Hardware;

namespace Meadow
{
    public interface IGPIOManager
    {
        void Initialize();
        void SetDiscrete(IDigitalPin pin, bool value);
        bool GetDiscrete(IDigitalPin pin);

        void ConfigureOutput(IDigitalPin pin, bool initialState);
        void ConfigureInput(IDigitalPin pin, bool glitchFilter, ResistorMode resistorMode, bool interruptEnabled);
    }
}
