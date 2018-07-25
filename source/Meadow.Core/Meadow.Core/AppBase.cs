using System;
namespace Meadow
{
    public abstract class AppBase<T> : IApp where T : class, IApp
    {
        /// <summary>
        /// </summary>
        /// <value>The current.</value>
        public static T Current
        {
            get { return _current; }
        } private static T _current;

        protected AppBase()
        {
            _current = this as T;
        }

        /// <summary>
        /// Called the first time the application is started.
        /// TODO: Is this necessary? Why not just use the ctor?
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Called when the application is put to sleep.
        /// </summary>
        public virtual void WillSleep() {}

        /// <summary>
        /// Called when the application wakes up from sleep.
        /// </summary>
        public virtual void OnWake() {}

        /// <summary>
        /// 
        /// </summary>
        public virtual void WillReset() {}
    }
}
