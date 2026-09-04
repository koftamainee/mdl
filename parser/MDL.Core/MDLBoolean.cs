namespace MDL.Core;

/// <summary>
/// A boolean literal (<c>true</c> or <c>false</c>) from the MDL grammar.
/// </summary>
public sealed class MDLBoolean : MDLValue
{
    /// <summary>Creates a new boolean value.</summary>
    public MDLBoolean(bool value)
    {
        Value = value;
    }

    /// <inheritdoc/>
    public override MDLValueKind Kind => MDLValueKind.Boolean;

    /// <summary>The boolean value.</summary>
    public bool Value { get; }
}
