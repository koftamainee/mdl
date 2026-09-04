namespace MDL.Core;

/// <summary>
/// A single key/value pair inside an <see cref="MDLObject"/>.
/// </summary>
public readonly struct MDLPair
{
    /// <summary>Creates a new key/value pair.</summary>
    public MDLPair(string key, MDLValue value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>The object key (an identifier).</summary>
    public string Key { get; }

    /// <summary>The associated value.</summary>
    public MDLValue Value { get; }
}
