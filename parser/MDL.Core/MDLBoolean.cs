namespace MDL.Core;

/// <summary>
/// A boolean literal (<c>true</c> or <c>false</c>) from the MDL grammar.
/// </summary>
public sealed class MDLBoolean : MDLValue
{
    public MDLBoolean(bool value)
    {
        Value = value;
    }

    public override MDLValueKind Kind => MDLValueKind.Boolean;

    /// <summary>The boolean value.</summary>
    public bool Value { get; }
}
