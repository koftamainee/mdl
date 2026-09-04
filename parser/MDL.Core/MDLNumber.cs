namespace MDL.Core;

/// <summary>
/// Marker interface for numeric values (<see cref="MDLInteger"/> and <see cref="MDLFloat"/>).
/// Use for type-checking without coupling to conversion policy.
/// </summary>
public interface IMDLNumber { }

/// <summary>
/// An integer literal from the MDL grammar. Stored internally as <see cref="long"/>.
/// </summary>
public sealed class MDLInteger : MDLValue, IMDLNumber
{
    /// <summary>Creates a new integer value.</summary>
    public MDLInteger(long value)
    {
        Value = value;
    }

    /// <inheritdoc/>
    public override MDLValueKind Kind => MDLValueKind.Integer;

    /// <summary>The integer value.</summary>
    public long Value { get; }
}

/// <summary>
/// A floating-point literal from the MDL grammar (decimal point or exponent).
/// Stored internally as <see cref="double"/>.
/// </summary>
public sealed class MDLFloat : MDLValue, IMDLNumber
{
    /// <summary>Creates a new float value.</summary>
    public MDLFloat(double value)
    {
        Value = value;
    }

    /// <inheritdoc/>
    public override MDLValueKind Kind => MDLValueKind.Float;

    /// <summary>The floating-point value.</summary>
    public double Value { get; }
}
