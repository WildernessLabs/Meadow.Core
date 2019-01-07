using Meadow.Hardware;

namespace Meadow
{
    public interface IGPIOManager
    {
        void Initialize();
        void SetDiscrete(IDigitalPin pin, bool value);
        bool GetDiscrete(IDigitalPin pin);

        void Configure(IDigitalPin pin, bool initialState);
        void Configure(IDigitalPin pin, bool glitchFilter, ResistorMode resistorMode);
    }
}
