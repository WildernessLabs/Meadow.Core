using System;
namespace Meadow
{
    public abstract class App : IApp
    {
        public static IApp Current
        {
            get { return _current; }
        } private static IApp _current;

        //TODO: i think this has to happen via meadow
        // this will always be called.
        // do we need to mark it protected? any reason to?
        protected App()
        {
            _current = this;
        }

        public abstract void OnSleep();

        public abstract void OnWake();

        public abstract void Run();
    }
}
