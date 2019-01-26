namespace Meadow.Peripherals.Displays
{
    /// <summary>
    /// Defines a Text Display.
    /// </summary>
    public interface ITextDisplay
    {
        /// <summary>
        /// Gets a Display Configuration
        /// </summary>
        TextDisplayConfig DisplayConfig { get; }

        /// <summary>
        /// Writes the specified string on the display.
        /// </summary>
        /// <param name="text">String to display</param>
        void Write(string text);

        /// <summary>
        /// Writes the specified string to the specified line number on the display.
        /// </summary>
        /// <param name="text">String to display.</param>
        /// <param name="lineNumber">Line Number.</param>
        void WriteLine(string text, byte lineNumber);

        /// <summary>
        /// Clears the entire display.
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears the specified line of characters on the display.
        /// </summary>
        /// <param name="lineNumber">Line Number</param>
        void ClearLine(byte lineNumber);

        /// <summary>
        /// Sets the display's brightness.
        /// </summary>
        /// <param name="brightness">0 is lowest, 1 is highest</param>
        void SetBrightness(float brightness = 0.75f);

        /// <summary>
        /// Set cursor in the especified row and column.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        void SetCursorPosition(byte column, byte line);

        /// <summary>
        ///  is this going to be supported by all text displays?
        /// </summary>
        /// <param name="characterMap"></param>
        /// <param name="address"></param>
        void SaveCustomCharacter(byte[] characterMap, byte address);
    }
}
