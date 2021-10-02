using System;
using System.Threading;
using Meadow.Devices;

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
        //==== public properties
        /// <summary>
        /// </summary>
        /// <value>The current instance.</value>
        public static A Current {
            get { return _current; }
        }
        private static A _current = null!;

        /// <summary>
        /// The Device
        /// </summary>
        public static D Device {
            get => _device;
        }
        private static D _device = null!;

        /// <summary>
        /// Called when the application is put to sleep.
        /// </summary>
        public virtual void WillSleep() { }

        /// <summary>
        /// Called when the application wakes up from sleep.
        /// </summary>
        public virtual void OnWake() { }

        /// <summary>
        /// 
        /// </summary>
        public virtual void WillReset() { }

        /// <summary>
        /// this is fine.
        /// </summary>
        static App()
        {
            // capture unhandled exceptions so the runtime doesn't tear itself
            // apart
            //
            // BC NOTE: I tried to move this to the MeadowOS class but we have a
            // chicken/egg problem. If MeadowOS isn't initialized we throw an err,
            // but because it's not instantiated, the exception handler won't exist.
            AppDomain.CurrentDomain.UnhandledException += StaticOnUnhandledException;

            // initialze MeadowOS
            MeadowOS.Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        protected App()
        {
            // BC Note: near as i can tell, in C# if a default ctor is specified
            // in a base class, it will be called no matter what, whether the
            // derived class implicitly calls it (public foo() : base()) or not.
            // Even if the derived class has no default constructor.
            //
            // TL;DR: this will always be called.

            _device = Activator.CreateInstance<D>();

            _device.Initialize();
            MeadowOS.CurrentDevice = _device;

            // singleton setting
            _current = this as A ?? throw new Exception($"Unable to convert to {typeof(A).Name}");
        }

        //==== exception handler

        private static void StaticOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (_current == null) {
                // in the event the user never called the base c'tor
                Console.WriteLine($"Unhandled Exception: {e.ExceptionObject}");
            } else {
                (_current as App<D, A>)?.OnUnhandledException(sender, e);
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
