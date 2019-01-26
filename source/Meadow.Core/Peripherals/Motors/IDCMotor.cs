namespace Meadow.Peripherals.Motors
{
    /// <summary>
    /// Interface describing DC motors.
    /// </summary>
    public interface IDCMotor
    {
        /// <summary>
        /// The speed of the motor from -1 to 1.
        /// </summary>
        float Speed { get; set; }

        /// <summary>
        /// When true, the wheels spin "freely"
        /// </summary>
        bool IsNeutral { get; set; }
    }
}
