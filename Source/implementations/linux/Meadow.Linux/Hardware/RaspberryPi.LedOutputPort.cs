using Meadow.Hardware;
using System;
using System.IO;

namespace Meadow;

public partial class RaspberryPi
{
    public class LedOutputPort : DigitalOutputPortBase
    {
        private string brightnessFsPath;

        internal LedOutputPort(IPin pin, bool initialState)
            : base(pin, new DigitalChannelInfo(pin.Name, false, true, false, false, false), initialState, OutputType.PushPull)
        {
            brightnessFsPath = $"/sys/class/leds/{pin.Name}/brightness";
        }

        /// <inheritdoc/>
        public override bool State
        {
            get
            {
                var c = File.ReadAllText(brightnessFsPath).Trim();
                return c != "0";
            }
            set
            {
                try
                {
                    File.WriteAllText(brightnessFsPath, value ? "1" : "0");
                }
                catch (UnauthorizedAccessException)
                {
                    throw new Exception($"Not authorized to access '{brightnessFsPath}'.{Environment.NewLine}Try running as sudo or run 'sudo chmod 666 {brightnessFsPath}'");
                }
            }
        }
    }
}
