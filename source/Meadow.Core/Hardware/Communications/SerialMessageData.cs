using System;
using System.Text;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a `SerialMessagePort` message consiting of a `byte[]` of the
    /// actual message data.
    /// </summary>
    public class SerialMessageData : EventArgs
    {
        /// <summary>
        /// A `byte[]` of the actual message data.
        /// </summary>
        public byte[] Message { get; set; } = new byte[0];

        /// <summary>
        /// Returns a decoded version of the message bytes.
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public string GetMessageString(Encoding encoding)
        {
            return encoding.GetString(this.Message);
        }

        public static SerialMessageData FromString(string message, Encoding encoding)
        {
            return new SerialMessageData() { Message = encoding.GetBytes(message) };
        }

    }
}
