using Meadow.Hardware;
using Meadow.Units;

namespace Meadow.Hardware
{
    public delegate void InterruptHandler(IPin pin, bool state);

    /// <summary>
    /// Defines the GPIO Manager
    /// </summary>
    public interface IMeadowIOController
    {
        event InterruptHandler Interrupt;

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
        /// Sets the resistor mode for the given Pin
        /// </summary>
        /// <param name="pin">Pin.</param>
        /// <param name="mode">Mode.</param>
        void SetResistorMode(IPin pin, ResistorMode mode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="initialState"></param>
        /// <param name="outputType"></param>
        void ConfigureOutput(
            IPin pin,
            bool initialState,
            OutputType outputType
            );
            
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="resistorMode"></param>
        /// <param name="interruptMode"></param>
        /// <param name="debounceDuration"></param>
        /// <param name="glitchDuration"></param>
        void ConfigureInput(
            IPin pin,
            ResistorMode resistorMode,
            InterruptMode interruptMode,
            double debounceDuration,
            double glitchDuration
            );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="interruptMode"></param>
        /// <param name="debounceDuration"></param>
        /// <param name="glitchDuration"></param>
        void WireInterrupt(IPin pin,
            InterruptMode interruptMode,
            ResistorMode resistorMode,
            double debounceDuration,
            double glitchDuration
            );

        bool UnconfigureGpio(IPin pin);

        void ConfigureAnalogInput(IPin pin);
        int GetAnalogValue(IPin pin);
        void ReassertConfig(IPin pin);

        Temperature GetTemperature();
    }
}
