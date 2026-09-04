using System;

namespace MDL.Serializer;

/// <summary>
/// Overrides the key used to map a member of a CLR type to a key in an
/// MDL object. When absent, the member name is used as the key.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class MDLNameAttribute : Attribute
{
    /// <summary>Creates a new name attribute.</summary>
    public MDLNameAttribute(string name)
    {
        Name = name;
    }

    /// <summary>The MDL object key to map to.</summary>
    public string Name { get; }
}
