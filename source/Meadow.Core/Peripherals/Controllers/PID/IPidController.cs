namespace Meadow.Peripherals.Controllers.PID
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPidController
    {
        /// <summary>
        /// 
        /// </summary>
        float OutputMin { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float OutputMax { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float ActualInput { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float TargetInput { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float IntegralComponent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float DerivativeComponent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float ProportionalComponent { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool OutputTuningInformation { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        void ResetIntegrator();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        float CalculateControlOutput();
    }
}
