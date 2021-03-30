using System;
namespace Meadow.Hardware
{
    public interface IDigitalOutputPort : IDigitalPort
    {
        /// <summary>
        /// Gets the port’s initial state, either low (false), or high (true), as typically configured during the port’s constructor.
        /// </summary>
        bool InitialState { get; }
        /// <summary>
        /// Gets or sets the state of the port.
        /// </summary>
        /// <value><c>true</c> for `HIGH`; otherwise, <c>false</c>, for `LOW`.</value>
        bool State { get; set; }
    }
}