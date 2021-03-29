using System;
namespace Meadow.Hardware
{
    public interface IDigitalOutputDevice
    {
        IDigitalOutputPort CreateDigitalOutputPort(
            IPin pin,
            bool initialState = false,
            OutputType initialOutputType = OutputType.PushPull);

    }
}
