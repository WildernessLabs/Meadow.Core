using System;
namespace Meadow.Hardware
{
    public class DigitalChannelIInfoBase : ChannelInfoBase, IDigitalChannelInfo
    {
        public bool InputCapable { get; protected set; }
        public bool OutputCapable { get; protected set; } // TODO: do we need IDigitalOutputChannel?
        public bool InterrruptCapable { get; protected set; }
        public bool PullDownCapable { get; protected set; }
        public bool PullUpCapable { get; protected set; }

        protected DigitalChannelIInfoBase(
            string name,
            bool inputCapable,
            bool outputCapable,
            bool interruptCapable,
            bool pullDownCapable,
            bool pullUpCapable)
            : base(name)
        {
            this.InputCapable = inputCapable;
            this.OutputCapable = outputCapable;
            this.InterrruptCapable = interruptCapable;
            this.PullDownCapable = pullDownCapable;
            this.PullUpCapable = pullUpCapable;
        }
    }
}
