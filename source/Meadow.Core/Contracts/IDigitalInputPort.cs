using System;
namespace Meadow
{
    public interface IDigitalInputPort : IDigitalPort
    {
        event EventHandler<PortEventArgs> Changed;

        /// <summary>
        /// TODO: Rename this? EventEnabled? 
        /// Gets a value indicating whether this <see cref="T:Meadow.IDigitalInputPort"/> supports interrupts.
        /// </summary>
        /// <value><c>true</c> if interrupt enabled; otherwise, <c>false</c>.</value>
        bool InterrupEnabled { get; }

        bool Value { get; }

    }
}
