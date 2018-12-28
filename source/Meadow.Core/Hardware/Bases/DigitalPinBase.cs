using System;
namespace Meadow.Hardware
{
    public abstract class DigitalPinBase : PinBase, IDigitalPin
    {
        protected DigitalPinBase(string name, object key, 
                                 bool interruptCapable = false, 
                                 bool pullDownCapable = false, 
                                 bool pullUpCapable = false) 
            : base(name, key)
        {
            _interruptCapable = interruptCapable;
            _pullDownCapable = pullDownCapable;
            _pullUpCapable = pullUpCapable;

        }

        protected bool _interruptCapable;
        public bool InterrruptCapable => _interruptCapable;

        protected bool _pullDownCapable;
        public bool PullDownCapable => _pullDownCapable;

        protected bool _pullUpCapable;
        public bool PullUpCapable => _pullUpCapable;

        public abstract IGPIOManager GPIOManager { get; internal set; }
    }
}
