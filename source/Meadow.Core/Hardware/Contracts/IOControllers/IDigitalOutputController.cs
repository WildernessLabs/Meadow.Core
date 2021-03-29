using System;
namespace Meadow.Hardware
{
    public interface IDigitalOutputController
    {
        IDigitalOutputPort CreateDigitalOutputPort(
            IPin pin,
            bool initialState = false,
            OutputType initialOutputType = OutputType.PushPull);

    }
}