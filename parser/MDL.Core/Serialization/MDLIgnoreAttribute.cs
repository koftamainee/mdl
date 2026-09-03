using System;

namespace MDL.Core.Serialization
{
    /// <summary>
    /// Excludes a member of a CLR type from MDL deserialization. Useful for
    /// computed or internal fields that should not be populated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class MDLIgnoreAttribute : Attribute
    {
    }
}
