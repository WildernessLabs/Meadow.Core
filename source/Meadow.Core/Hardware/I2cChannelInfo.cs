﻿using System;
namespace Meadow.Hardware
{
    //TODO: what else should this have? allowed speeds?
    public class I2cChannelInfo : DigitalChannelInfoBase, II2cChannelInfo
    {
        public I2cChannelFunctionType ChannelFunction { get; protected set; }

        public I2cChannelInfo(string name,
            I2cChannelFunctionType channelFunction,
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
            this.ChannelFunction = channelFunction;
        }
    }
}
