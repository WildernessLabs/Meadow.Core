using System;
namespace Meadow
{
    /// <summary>
    /// Contract for change notifications.
    /// </summary>
    /// <typeparam name="UNIT"></typeparam>
    public interface IChangeResult<UNIT> where UNIT: struct
    {
        /// <summary>
        /// Current/new event value.
        /// </summary>
        UNIT New { get; set; }
        /// <summary>
        /// Previous value.
        /// </summary>
        UNIT? Old { get; set; }
    }
}
