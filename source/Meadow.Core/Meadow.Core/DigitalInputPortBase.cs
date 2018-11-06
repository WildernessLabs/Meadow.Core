﻿using System;
namespace Meadow
{
    public abstract class DigitalInputPortBase 
    {
        public event EventHandler<PortEventArgs> Changed = delegate { };

        public bool InterruptEnabled
        {
            get { return _interruptEnabled; }
        }
        protected bool _interruptEnabled;

        public abstract bool Value { get; }

        protected DigitalInputPortBase(bool interruptEnabled = false)
        {
            _interruptEnabled = interruptEnabled;
        }

        protected void RaiseChanged(bool value)
        {
            Changed(this, new PortEventArgs() { Value = value });
        }
    }
}