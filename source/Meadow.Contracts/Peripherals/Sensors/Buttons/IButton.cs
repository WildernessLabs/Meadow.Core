using System;

namespace Meadow.Peripherals.Sensors.Buttons
{
    /// <summary>
    /// Interface describing button classes.
    /// </summary>
    public interface IButton : ISensor
    {
        /// <summary>
        /// Raised when a press starts (the button is pushed down; circuit is closed).
        /// </summary>
        event EventHandler PressStarted;

        /// <summary>
        /// Raised when a press ends (the button is released; circuit is opened).
        /// </summary>
        event EventHandler PressEnded;

        /// <summary>
        /// Raised when the button circuit is re-opened after it has been closed (at the end of a “press”.
        /// </summary>
        event EventHandler Clicked;
        
        /// <summary>
        /// Returns the current raw state of the switch. If the switch 
        /// is pressed (connected), returns true, otherwise false.
        /// </summary>
        bool State { get; }
    }
}
