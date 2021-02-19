using System;
namespace Meadow.Gateways
{
    // <summary>
    // The types of antenna that can be selected.
    // </summary>
    public enum AntennaType
    {
        OnBoard = 0,
        External = 1,
        // TODO: remove this and change `if` condition per Mark.
        Max = 1
    };
}
