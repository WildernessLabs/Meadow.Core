using System;
namespace Meadow
{
    /// <summary>
    /// TODO: Is this necessary? We've decided that we want to use the app model, 
    /// as opposed to allowing users to create `Static void main()` implementations,
    /// but if we type the App.Current to the user's app somehow, we can potentially
    /// get rid of this.
    /// </summary>
    public interface IApp
    {
        /// <summary>
        /// Called the first time the application is started.
        /// TODO: Is this necessary? Why not just use the ctor?
        /// </summary>
        void Run();

        /// <summary>
        /// Called when the application is going to be sleep.
        /// </summary>
        void WillSleep();

        /// <summary>
        /// Called when the application wakes up from sleep.
        /// </summary>
        void OnWake();

        /// <summary>
        /// Called in case the OS needs to restart the app. Will have limited 
        /// processing time.
        /// </summary>
        void WillReset();

    }
}
