namespace MDL.Core;

/// <summary>
/// Discriminator tag for an <see cref="MDLValue"/> node.
/// </summary>
public enum MDLValueKind : byte
{
    /// <summary>An object (key/value pairs).</summary>
    Object = 0,

    /// <summary>An ordered list of values.</summary>
    List = 1,

    /// <summary>A string literal.</summary>
    String = 2,

    /// <summary>An integer literal.</summary>
    Integer = 3,

    /// <summary>A floating-point literal.</summary>
    Float = 4,

    /// <summary>A boolean literal.</summary>
    Boolean = 5,
}
