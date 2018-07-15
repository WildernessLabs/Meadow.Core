using System;
namespace Meadow
{
    public interface IDigitalOutputPort : IDigitalPort
    {
        /// <summary>
        /// Gets the port’s initial state, either low (false), or high (true), as typically configured during the port’s constructor.
        /// </summary>
        bool InitialState { get; }
    }
}