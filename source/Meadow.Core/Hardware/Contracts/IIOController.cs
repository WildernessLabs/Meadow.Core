using Meadow.Hardware;

namespace Meadow.Hardware
{
    public delegate void InterruptHandler(IPin pin);

    // TODO: Consider renaming to match MCP23008 driver: https://github.com/WildernessLabs/Netduino.Foundation/blob/master/Source/Peripheral_Libs/ICs.IOExpanders.MCP23008/Driver/MCP23008.cs
    // Write(IPin pin, bool value)
    // Write(IPin[] pin, byte value)? think about this. need 
    // Read(pin)

    /// <summary>
    /// Defines the GPIO Manager
    /// </summary>
    public interface IIOController
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
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="initialState"></param>
        void ConfigureOutput(IPin pin, bool initialState);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pin"></param>
        /// <param name="resistorMode"></param>
        /// <param name="interruptMode"></param>
        void ConfigureInput(
            IPin pin,
            ResistorMode resistorMode,
            InterruptMode interruptMode,
            int debounceDuration = 0,
            int glitchFilterCycleCount = 0
            );

        void ConfigureAnalogInput(IPin pin);
        int GetAnalogValue(IPin pin);
    }
}
