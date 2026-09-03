namespace MDL.Core;

/// <summary>
/// Base class for every value stored in an MDL document.
/// Concrete kinds: <see cref="MDLObject"/>, <see cref="MDLList"/>,
/// <see cref="MDLInteger"/>, <see cref="MDLFloat"/>,
/// <see cref="MDLBoolean"/> and <see cref="MDLString"/>.
/// </summary>
public abstract class MDLValue
{
    /// <summary>The discriminator tag for this value node.</summary>
    public abstract MDLValueKind Kind { get; }

    /// <summary>
    /// Allocates a new <see cref="MDLInteger"/> value.
    /// </summary>
    public static MDLValue Integer(long value) => new MDLInteger(value);

    /// <summary>
    /// Allocates a new <see cref="MDLFloat"/> value.
    /// </summary>
    public static MDLValue Float(double value) => new MDLFloat(value);

    /// <summary>
    /// Allocates a new <see cref="MDLBoolean"/> value.
    /// </summary>
    public static MDLValue Boolean(bool value) => new MDLBoolean(value);

    /// <summary>
    /// Allocates a new <see cref="MDLString"/> value.
    /// </summary>
    public static MDLValue String(string value) => new MDLString(value);

    /// <summary>
    /// Returns the underlying string when this value is an <see cref="MDLString"/>,
    /// otherwise <see langword="null"/>.
    /// </summary>
    public string? AsString() => this is MDLString s ? s.Value : null;

    /// <summary>
    /// Returns the underlying value as a 32-bit integer when this value is a number,
    /// otherwise <see langword="null"/>.
    /// </summary>
    public int? AsInt32()
    {
        return this switch
        {
            MDLInteger i => (int)i.Value,
            MDLFloat f => (int)f.Value,
            _ => null,
        };
    }

    /// <summary>
    /// Returns the underlying value as a 64-bit integer when this value is a number,
    /// otherwise <see langword="null"/>.
    /// </summary>
    public long? AsInt64()
    {
        return this switch
        {
            MDLInteger i => i.Value,
            MDLFloat f => (long)f.Value,
            _ => null,
        };
    }

    /// <summary>
    /// Returns the underlying value as a 32-bit float when this value is a number,
    /// otherwise <see langword="null"/>.
    /// </summary>
    public float? AsFloat()
    {
        return this switch
        {
            MDLInteger i => i.Value,
            MDLFloat f => (float)f.Value,
            _ => null,
        };
    }

    /// <summary>
    /// Returns the underlying value as a 64-bit float when this value is a number,
    /// otherwise <see langword="null"/>.
    /// </summary>
    public double? AsDouble()
    {
        return this switch
        {
            MDLInteger i => i.Value,
            MDLFloat f => f.Value,
            _ => null,
        };
    }

    /// <summary>
    /// Returns the underlying boolean when this value is an <see cref="MDLBoolean"/>,
    /// otherwise <see langword="null"/>.
    /// </summary>
    public bool? AsBool() => this is MDLBoolean b ? b.Value : null;


    /// <summary>
    /// Returns the underlying object when this value is an <see cref="MDLObject"/>,
    /// otherwise <see langword="null"/>.
    /// </summary>
    public MDLObject? AsObject() => this is MDLObject o ? o : null;

    /// <summary>
    /// Returns the underlying list when this value is an <see cref="MDLList"/>,
    /// otherwise <see langword="null"/>.
    /// </summary>
    public MDLList? AsList() => this is MDLList l ? l : null;
}
