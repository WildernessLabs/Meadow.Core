using System;
using System.Collections.Generic;

namespace Meadow.Hardware
{
    public interface INamedPinGroups
    {
        IList<NamedPinGroup> AllGroups { get; }
    }
}
