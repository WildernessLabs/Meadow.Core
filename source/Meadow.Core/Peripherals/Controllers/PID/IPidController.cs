namespace Meadow.Peripherals.Controllers.PID
{
    /// <summary>
    /// Represents a Proportional, Integral, Derivative (PID) controller.
    /// </summary>
    public interface IPidController
    {
        /// <summary>
        /// The minimum allowable control output value. The control output is clamped to this value after being calculated via the CalculateControlOutput method.
        /// </summary>
        float OutputMin { get; set; }

        /// <summary>
        /// The maximum allowable control output value. The control output is clamped to this value after being calculated via the CalculateControlOutput method.
        /// </summary>
        float OutputMax { get; set; }

        /// <summary>
        /// Represents the Process Variable (PV), or the actual signal reading of the system in its current state.
        /// </summary>
        float ActualInput { get; set; }

        /// <summary>
        /// Represents the set point (SP), or the reference target signal to achieve.
        /// </summary>
        float TargetInput { get; set; }

        /// <summary>
        /// The value to use when calculating the integral corrective action.
        /// </summary>
        float IntegralComponent { get; set; }

        /// <summary>
        /// The value to use when calculating the derivative corrective action.
        /// </summary>
        float DerivativeComponent { get; set; }

        /// <summary>
        /// The value to use when calculating the proportional corrective action.
        /// </summary>
        float ProportionalComponent { get; set; }

        /// <summary>
        /// Whether or not to print the calculation information to the output console in an comma-delimited form.
        /// </summary>
        bool OutputTuningInformation { get; set; }

        /// <summary>
        /// Resets the integrator error history.
        /// </summary>
        void ResetIntegrator();

        /// <summary>
        /// Calculates the control output based on the difference (error) between the ActualInput and TargetInput, using the supplied ProportionalComponent, IntegralComponent, and DerivativeComponent.
        /// </summary>
        /// <returns></returns>
        float CalculateControlOutput();
    }
}
