using System;

namespace NetGoLynx.Models
{
    /// <summary>
    /// Types of access an account can have
    /// </summary>
    [Flags]
    public enum AccessType
    {
        Default = 0,

        Admin = 1,
    }
}
