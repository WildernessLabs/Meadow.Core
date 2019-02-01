namespace Meadow.Peripherals.Speakers
{
    /// <summary>
    /// Audio tones generator that plays tones at a given frequency.
    /// </summary>
    interface IToneGenerator
    {
        /// <summary>
        /// Plays the tone with a especified frequency and duration.
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="duration"></param>
        void PlayTone(float frequency, int duration = 0);

        /// <summary>
        /// Stops the tone playing.
        /// </summary>
        void StopTone();
    }
}