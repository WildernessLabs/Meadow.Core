using Meadow.Hardware;

namespace Meadow.Hardware
{
    /// <summary>
    /// Defines the GPIO Manager
    /// </summary>
    public interface IGpioController
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
        void SetDiscrete(IPin pin, bool value);

        /// <summary>
        /// Gets the value of a discrete (digital input)
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        bool GetDiscrete(IPin pin);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="initialState"></param>
        void ConfigureOutput(IPin pin, bool initialState);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="glitchFilter"></param>
        /// <param name="resistorMode"></param>
        /// <param name="interruptEnabled"></param>
        void ConfigureInput(IPin pin, bool glitchFilter, ResistorMode resistorMode, bool interruptEnabled);
    }
}
