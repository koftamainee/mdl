using System;
using System.Collections.Generic;

namespace MDL.Core;

/// <summary>
/// An ordered set of key/value pairs, the container used at the root of a document
/// and for <c>{ ... }</c> objects in the MDL grammar.
/// </summary>
public sealed class MDLObject : MDLValue
{
    public override MDLValueKind Kind => MDLValueKind.Object;
    private readonly List<MDLPair> _pairs = new List<MDLPair>();

    /// <summary>The pairs in declaration order.</summary>
    public IReadOnlyList<MDLPair> Pairs => _pairs;

    /// <summary>The number of pairs contained.</summary>
    public int Count => _pairs.Count;

    /// <summary>
    /// Gets the value associated with <paramref name="key"/>, or <see langword="null"/>
    /// when the key is absent.
    /// </summary>
    public MDLValue? GetValue(string key)
    {
        for (int i = 0; i < _pairs.Count; i++)
        {
            if (string.Equals(_pairs[i].Key, key, StringComparison.Ordinal))
                return _pairs[i].Value;
        }
        return null;
    }

    /// <summary>
    /// Gets the value associated with <paramref name="key"/>, or the provided
    /// <paramref name="defaultValue"/> when the key is absent.
    /// </summary>
    public MDLValue GetValueOrDefault(string key, MDLValue defaultValue)
    {
        var v = GetValue(key);
        return v ?? defaultValue;
    }

    /// <summary>
    /// Appends a key/value pair to the object. Duplicate keys are permitted
    /// and preserve declaration order (mirroring the source MDL document).
    /// </summary>
    public void Add(string key, MDLValue value) => _pairs.Add(new MDLPair(key, value));

    /// <summary>Appends an existing pair to the object.</summary>
    public void Add(MDLPair pair) => _pairs.Add(pair);

    /// <summary>
    /// Gets the string associated with <paramref name="key"/>, or <see langword="null"/>
    /// when the key is absent or is not a string.
    /// </summary>
    public MDLString? GetString(string key) => GetValue(key) as MDLString;

    /// <summary>
    /// Gets the object associated with <paramref name="key"/>, or <see langword="null"/>
    /// when the key is absent or is not an object.
    /// </summary>
    public MDLObject? GetObject(string key) => GetValue(key) as MDLObject;

    /// <summary>
    /// Gets the list associated with <paramref name="key"/>, or <see langword="null"/>
    /// when the key is absent or is not a list.
    /// </summary>
    public MDLList? GetList(string key) => GetValue(key) as MDLList;

    /// <summary>
    /// Gets the integer associated with <paramref name="key"/>, or <see langword="null"/>
    /// when the key is absent or is not an integer.
    /// </summary>
    public MDLInteger? GetInteger(string key) => GetValue(key) as MDLInteger;

    /// <summary>
    /// Gets the float associated with <paramref name="key"/>, or <see langword="null"/>
    /// when the key is absent or is not a float.
    /// </summary>
    public MDLFloat? GetFloat(string key) => GetValue(key) as MDLFloat;

    /// <summary>
    /// Gets the boolean associated with <paramref name="key"/>, or <see langword="null"/>
    /// when the key is absent or is not a boolean.
    /// </summary>
    public MDLBoolean? GetBoolean(string key) => GetValue(key) as MDLBoolean;

    /// <summary>The values in declaration order.</summary>
    public IEnumerable<MDLValue> Values()
    {
        foreach (var pair in _pairs)
            yield return pair.Value;
    }
}
