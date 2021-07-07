namespace Meadow.Peripherals.Leds
{
    /// <summary>
    /// Defines a simple Light Emitting Diode (LED).
    /// </summary>
    public interface ILed
    {
        ///// <summary>
        ///// The IDigitalOutputPort that the LED is connected to.
        ///// </summary>
        ///// <value>The digital output port.</value>
        //IDigitalOutputPort Port { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the LED is on.
        /// </summary>
        /// <value><c>true</c> if is on; otherwise, <c>false</c>.</value>
        bool IsOn { get; set; }

        /// <summary>
        /// Blink animation that turns the LED on and off based on the OnDuration and offDuration values in ms
        /// </summary>
        /// <param name="onDuration"></param>
        /// <param name="offDuration"></param>
        void StartBlink(int onDuration = 200, int offDuration = 200);

        /// <summary>
        /// Stops blink animation.
        /// </summary>
        void Stop();
    }
}