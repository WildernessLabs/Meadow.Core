using Meadow.Hardware;
using Meadow.Devices;
using System;
using System.Threading;

namespace Meadow
{
    /// <summary>
    /// Provides a base implementation for the Meadow IApp contract. Use this 
    /// class for Meadow applications to get strongly-typed access to the current
    /// device information.
    /// </summary>
    public abstract class App<D, A> : IApp 
        where A : class, IApp
        where D : class, IMeadowDevice
    {
        /// <summary>
        /// </summary>
        /// <value>The current.</value>
        public static A Current
        {
            get { return _current; }
        } private static A? _current;

        private static SynchronizationContext _mainContext { get; }

        static App()
        {
            AppDomain.CurrentDomain.UnhandledException += StaticOnUnhandledException;

            _mainContext = new MeadowSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(_mainContext);
        }

        protected App()
        {
            _device = Activator.CreateInstance<D>();
            _device.SetSynchronizationContext(_mainContext);
            _device.Initialize();
            MeadowOS.CurrentDevice = _device;

            //BUGBUG: because a user's `App` class doesn't have to call this
            // base ctor, then this might not ever run, or it might run
            // non-deterministically. so we need to figure out how to make sure
            // this stuff happens
            _current = this as A;
        }

        public static D Device
        {
            get => _device;
        } private static D _device;

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

        private static void StaticOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if(_current == null)
            {
                // in the event the user never called the base c'tor
                Console.WriteLine($"Unhandled Exception: {e.ExceptionObject}");
            }
            else
            {
                (_current as App<D,A>)?.OnUnhandledException(sender, e);
            }
        }

        /// <summary>
        /// Called when the application encounters an unhandled Exception
        /// </summary>
        /// <param name="sender">The source of the exception</param>
        /// <param name="e">The unhandled Exception object</param>
        internal virtual void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"Unhandled Exception: {e.ExceptionObject}");
        }
    }
}
