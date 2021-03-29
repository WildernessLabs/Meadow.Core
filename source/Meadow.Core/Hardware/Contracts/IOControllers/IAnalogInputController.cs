using System;

namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for devices that expose `IAnalogInputPort(s)`.
    /// </summary>
    public interface IAnalogInputController
    {
        public const float DefaultA2DReferenceVoltage = 3.3f;

        IAnalogInputPort CreateAnalogInputPort(
            IPin pin,
            float voltageReference = DefaultA2DReferenceVoltage
        );
    }
}
