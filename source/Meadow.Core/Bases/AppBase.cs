using System;
namespace Meadow
{
    /// <summary>
    /// Provides a base implementation for the Meadow IApp contract. Use this 
    /// class for Meadow applications to get strongly-typed access to the current
    /// device information.
    /// </summary>
    public abstract class AppBase<D, A> : IApp where A : class, IApp where D : class, IDevice
    {
        /// <summary>
        /// </summary>
        /// <value>The current.</value>
        public static A Current
        {
            get { return _current; }
        } private static A _current;

        protected AppBase()
        {
            _current = this as A;
            _device = Activator.CreateInstance<D>();
        }

        public static D Device
        {
            get { return _device; }
        } private static D _device;

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
