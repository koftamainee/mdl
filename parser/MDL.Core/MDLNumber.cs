namespace MDL.Core;

/// <summary>
/// Marker interface for numeric values (<see cref="MDLInteger"/> and <see cref="MDLFloat"/>).
/// Use for type-checking without coupling to conversion policy.
/// </summary>
public interface IMdlNumber { }

/// <summary>
/// An integer literal from the MDL grammar. Stored internally as <see cref="long"/>.
/// </summary>
public sealed class MDLInteger : MDLValue, IMdlNumber
{
    public MDLInteger(long value)
    {
        Value = value;
    }

    public override MdlValueKind Kind => MdlValueKind.Integer;

    /// <summary>The integer value.</summary>
    public long Value { get; }
}

/// <summary>
/// A floating-point literal from the MDL grammar (decimal point or exponent).
/// Stored internally as <see cref="double"/>.
/// </summary>
public sealed class MDLFloat : MDLValue, IMdlNumber
{
    public MDLFloat(double value)
    {
        Value = value;
    }

    public override MdlValueKind Kind => MdlValueKind.Float;

    /// <summary>The floating-point value.</summary>
    public double Value { get; }
}
