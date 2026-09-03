namespace MDL.Core;

/// <summary>
/// Discriminator tag for an <see cref="MDLValue"/> node.
/// </summary>
public enum MDLValueKind : byte
{
    Object = 0,
    List = 1,
    String = 2,
    Integer = 3,
    Float = 4,
    Boolean = 5,
}
