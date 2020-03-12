﻿using System;
namespace Meadow.Hardware
{
    public class DigitalChannelInfoBase : ChannelInfoBase, IDigitalChannelInfo
    {
        public bool InputCapable { get; protected set; }
        public bool OutputCapable { get; protected set; } // TODO: do we need IDigitalOutputChannel?
        public bool InterruptCapable { get; protected set; }
        public bool PullDownCapable { get; protected set; }
        public bool PullUpCapable { get; protected set; }
        public bool InverseLogic { get; protected set; }

        protected DigitalChannelInfoBase(
            string name,
            bool inputCapable,
            bool outputCapable,
            bool interruptCapable,
            bool pullDownCapable,
            bool pullUpCapable,
            bool inverseLogic)
            : base(name)
        {
            this.InputCapable = inputCapable;
            this.OutputCapable = outputCapable;
            this.InterruptCapable = interruptCapable;
            this.PullDownCapable = pullDownCapable;
            this.PullUpCapable = pullUpCapable;
            this.InverseLogic = inverseLogic;
        }
    }
}
