using System;
namespace Meadow
{
    /// <summary>
    /// TODO: Is this necessary? it allows more flexibility, but we lose 
    /// some control. also, requires us to do weird stuff. can't "protect" these
    /// methods from being called publicly.
    /// </summary>
    public interface IApp
    {
        

        /// <summary>
        /// Called the first time the application is started.
        /// TODO: Is this necessary? Why not just use the ctor?
        /// </summary>
        void Run();

        /// <summary>
        /// Called when the application is put to sleep.
        /// </summary>
        void OnSleep();

        /// <summary>
        /// Called when the application wakes up from sleep.
        /// </summary>
        void OnWake();

    }
}
