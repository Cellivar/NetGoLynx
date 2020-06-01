using System;

namespace NetGoLynx.Models
{
    /// <summary>
    /// Types of access an account can have
    /// </summary>
    [Flags]
    public enum AccessType
    {
        /// <summary>
        /// Standard user access level
        /// </summary>
        Default = 0,

        /// <summary>
        /// Complete control access level
        /// </summary>
        Admin = 1,
    }
}
