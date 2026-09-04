namespace MDL.Core;

/// <summary>
/// A string value: quoted, raw or bare string from the MDL grammar.
/// </summary>
public sealed class MDLString : MDLValue
{
    /// <summary>Creates a new string value.</summary>
    public MDLString(string value)
    {
        Value = value;
    }

    /// <inheritdoc/>
    public override MDLValueKind Kind => MDLValueKind.String;

    /// <summary>The string content with escapes already resolved.</summary>
    public string Value { get; }
}
