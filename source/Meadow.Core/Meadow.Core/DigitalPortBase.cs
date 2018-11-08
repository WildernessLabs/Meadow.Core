using System;
namespace Meadow
{
    /// <summary>
    /// DigitalPortBase provides a base implementation for much of the 
    /// common tasks of classes implementing IDigitalPort.
    /// </summary>
    public abstract class DigitalPortBase : IDigitalPort
    {
        public enum InterruptMode
        {
            InterruptNone = 0,
            InterruptEdgeLow = 1,
            InterruptEdgeHigh = 2,
            InterruptEdgeBoth = 3,
            InterruptEdgeLevelHigh = 4,
            InterruptEdgeLevelLow = 5
        }
        public enum ResistorMode
        {
            Disabled = 0,
            PullDown = 1,
            PullUp = 2
        }

        /// <summary>
        /// The PortDirectionType property is backed by the readonly _direction member. 
        /// This member must be set during the constructor and describes whether the 
        /// port in an input or output port.
        /// </summary>
        public PortDirectionType DirectionType
        {
            get { return _direction; }
        }
        protected readonly PortDirectionType _direction;

        /// <summary>
        /// The PortSignalType property returns PortSignalType.Digital.
        /// </summary>
        public SignalType SignalType { get { return SignalType.Digital; } }

        /// <summary>
        /// Gets or sets the port state, either high (true), or low (false).
        /// </summary>
        public virtual bool State
        {
            get { return _state; }
            set { _state = value; }
        }
        protected bool _state = false;

        protected DigitalPortBase(PortDirectionType direction)
        {
            _direction = direction;
        }
    }
}
