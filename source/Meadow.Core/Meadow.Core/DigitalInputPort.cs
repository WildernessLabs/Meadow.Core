﻿using Meadow.Hardware;
using System;

namespace Meadow
{
    public class DigitalInputPort : DigitalInputPortBase
    {
        public bool GlitchFilter { get; set; }
        public ResistorMode Resistor { get; set; }

        public DigitalInputPort(Pins portId, bool glitchFilter, ResistorMode resistorMode)
        {

        }

        public DigitalInputPort()
        {

        }

        public override bool Value => throw new NotImplementedException();
    }
}