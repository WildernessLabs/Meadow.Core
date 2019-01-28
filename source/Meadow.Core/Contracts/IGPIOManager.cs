using Meadow.Hardware;

namespace Meadow
{
    /// <summary>
    /// Defines the GPIO Manager
    /// </summary>
    public interface IGPIOManager
    {
        /// <summary>
        /// Initializes the device pins to their default power-up status (outputs, low and pulled down where applicable).
        /// </summary>
        void Initialize();

        /// <summary>
        /// Sets the value out a discrete (digital output)
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="value"></param>
        void SetDiscrete(IDigitalPin pin, bool value);

        /// <summary>
        /// Gets the value of a discrete (digital input)
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        bool GetDiscrete(IDigitalPin pin);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="initialState"></param>
        void ConfigureOutput(IDigitalPin pin, bool initialState);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="glitchFilter"></param>
        /// <param name="resistorMode"></param>
        /// <param name="interruptEnabled"></param>
        void ConfigureInput(IDigitalPin pin, bool glitchFilter, ResistorMode resistorMode, bool interruptEnabled);
    }
}
