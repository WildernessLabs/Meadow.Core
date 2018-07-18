using System;
namespace Meadow
{
    public abstract class AppBase : IApp
    {
        /// <summary>
        /// TODO: is there a better way to do this? could we somehow type Current
        /// to be the type of the consumers app, so they didn't have to cast?
        /// 
        /// could we abuse generics or something?
        /// 
        /// What about a lambda?
        /// </summary>
        /// <value>The current.</value>
        public static IApp Current
        {
            get { return _current; }
        } private static IApp _current;

        protected AppBase()
        {
            _current = this;
        }

        //TODO: do we make these non-abstract, so that we can fufill the contract
        // and not require the consumer to implement them?

        /// <summary>
        /// Called the first time the application is started.
        /// TODO: Is this necessary? Why not just use the ctor?
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Called when the application is put to sleep.
        /// </summary>
        public abstract void WillSleep();

        /// <summary>
        /// Called when the application wakes up from sleep.
        /// </summary>
        public abstract void OnWake();

        /// <summary>
        /// 
        /// </summary>
        public abstract void WillReset();
    }
}
