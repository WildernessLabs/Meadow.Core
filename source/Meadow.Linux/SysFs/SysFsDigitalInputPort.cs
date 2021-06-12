using Meadow.Hardware;
using System;

namespace Meadow
{
    public class SysFsDigitalInputPort : IDigitalInputPort
    {
        public bool State => throw new NotImplementedException();

        public ResistorMode Resistor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public InterruptMode InterruptMode => throw new NotImplementedException();

        public double DebounceDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double GlitchDuration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IDigitalChannelInfo Channel => throw new NotImplementedException();

        public IPin Pin => throw new NotImplementedException();

        public event EventHandler<DigitalPortResult> Changed;

        event System.EventHandler<DigitalPortResult> IDigitalInterruptPort.Changed
        {
            add
            {
                throw new System.NotImplementedException();
            }

            remove
            {
                throw new System.NotImplementedException();
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<IChangeResult<DigitalState>> observer)
        {
            throw new NotImplementedException();
        }
    }
}
