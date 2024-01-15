using Meadow.Hardware;
using System;

namespace Meadow.Devices
{
    /// <summary>
    /// Represents the base class for Meadow F7 Microcontroller, implementing IBiDirectionalController.
    /// </summary>
    public abstract partial class F7MicroBase : IBiDirectionalController
    {
        /// <summary>
        /// Creates a bi-directional port with the specified parameters.
        /// </summary>
        /// <param name="pin">The pin for the port.</param>
        /// <param name="initialState">The initial state of the port.</param>
        /// <param name="resistorMode">The resistor mode for the port.</param>
        /// <param name="initialDirection">The initial direction of the port.</param>
        /// <returns>The created bi-directional port.</returns>
        public IBiDirectionalPort CreateBiDirectionalPort(
            IPin pin,
            bool initialState = false,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input
            )
        {
            return BiDirectionalPort.From(pin, this.IoController, initialState, resistorMode, initialDirection, OutputType.PushPull);
        }

        /// <summary>
        /// Creates a bi-directional interrupt port with the specified parameters.
        /// </summary>
        /// <param name="pin">The pin for the interrupt port.</param>
        /// <param name="initialState">The initial state of the interrupt port.</param>
        /// <param name="interruptMode">The interrupt mode for the port.</param>
        /// <param name="resistorMode">The resistor mode for the port.</param>
        /// <param name="initialDirection">The initial direction of the port.</param>
        /// <returns>The created bi-directional interrupt port.</returns>
        public IBiDirectionalInterruptPort CreateBiDirectionalInterruptPort(
            IPin pin,
            bool initialState = false,
            InterruptMode interruptMode = InterruptMode.None,
            ResistorMode resistorMode = ResistorMode.Disabled,
            PortDirectionType initialDirection = PortDirectionType.Input
            )
        {
            return CreateBiDirectionalInterruptPort(pin, initialState, interruptMode, resistorMode, initialDirection, TimeSpan.Zero, TimeSpan.Zero, OutputType.PushPull);
        }

        /// <summary>
        /// Creates a bi-directional interrupt port with extended parameters.
        /// </summary>
        /// <param name="pin">The pin for the interrupt port.</param>
        /// <param name="initialState">The initial state of the interrupt port.</param>
        /// <param name="interruptMode">The interrupt mode for the port.</param>
        /// <param name="resistorMode">The resistor mode for the port.</param>
        /// <param name="initialDirection">The initial direction of the port.</param>
        /// <param name="debounceDuration">The debounce duration for the interrupt.</param>
        /// <param name="glitchDuration">The glitch duration for the interrupt.</param>
        /// <param name="outputType">The output type for the port.</param>
        /// <returns>The created bi-directional interrupt port.</returns>
        public IBiDirectionalInterruptPort CreateBiDirectionalInterruptPort(
            IPin pin,
            bool initialState,
            InterruptMode interruptMode,
            ResistorMode resistorMode,
            PortDirectionType initialDirection,
            TimeSpan debounceDuration,
            TimeSpan glitchDuration,
            OutputType outputType
            )
        {
            // Convert durations to unsigned int with 100 usec resolution
            return BiDirectionalInterruptPort.From(pin, this.IoController, initialState, interruptMode, resistorMode, initialDirection, debounceDuration, glitchDuration, outputType);
        }

        /// <summary>
        /// Creates a bi-directional port with the specified pin and initial state.
        /// </summary>
        /// <param name="pin">The pin for the port.</param>
        /// <param name="initialState">The initial state of the port.</param>
        /// <returns>The created bi-directional port.</returns>
        public IBiDirectionalPort CreateBiDirectionalPort(IPin pin, bool initialState)
        {
            return BiDirectionalPort.From(pin, this.IoController, initialState);
        }
    }
}
