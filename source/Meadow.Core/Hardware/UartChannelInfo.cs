﻿using System;
namespace Meadow.Hardware
{
    public class UartChannelInfo : DigitalChannelInfoBase, IUartChannelInfo
    {
        public SerialDirectionType SerialDirection { get; protected set; }

        public UartChannelInfo(string name,
            SerialDirectionType serialDirection,
            bool pullDownCapable = false,
            bool pullUpCapable = false)
            : base(
                name,
                inputCapable: true,
                outputCapable: true,
                interruptCapable: false, // ?? i mean, technically, yes, but will we have events?
                pullDownCapable: pullDownCapable,
                pullUpCapable: pullUpCapable,
                inverseLogic: false) //TODO: switch to C# 7.2+ to get rid of trailing names
        {
            this.SerialDirection = serialDirection;
        }
    }
}
