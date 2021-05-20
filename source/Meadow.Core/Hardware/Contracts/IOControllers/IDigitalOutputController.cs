using System;
namespace Meadow.Hardware
{
    /// <summary>
    /// Contract for IO devices that are capable of creating `IDigitalOuputPort`
    /// instances.
    /// </summary>
    public interface IDigitalOutputController
    {
        /// <summary>
        /// Creates an IDigitalOutputPort on the specified pin.
        /// </summary>
        /// <param name="pin">The pin on which to create the port.</param>
        /// <param name="initialState">The default initial state of the port,
        /// either `LOW` (`false`), or `HIGH` (`true`).</param>
        /// <param name="initialOutputType">Whether the port is initially configured
        /// as PushPull or OpenDrain. PushPull by default.</param>
        /// <returns></returns>
        IDigitalOutputPort CreateDigitalOutputPort(
            IPin pin,
            bool initialState = false,
            OutputType initialOutputType = OutputType.PushPull);
    }
}