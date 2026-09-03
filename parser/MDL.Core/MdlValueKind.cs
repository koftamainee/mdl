namespace MDL.Core;

/// <summary>
/// Discriminator tag for an <see cref="MDLValue"/> node.
/// </summary>
public enum MdlValueKind : byte
{
    Null = 0,
    Object = 1,
    List = 2,
    String = 3,
    Integer = 4,
    Float = 5,
    Boolean = 6,
}
