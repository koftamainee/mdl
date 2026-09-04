using System;
using System.Collections.Generic;

namespace MDL.Core;

/// <summary>
/// An ordered set of key/value pairs, the container used at the root of a document
/// and for <c>{ ... }</c> objects in the MDL grammar.
/// </summary>
public sealed class MDLObject : MDLValue
{
    /// <inheritdoc/>
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
    /// Gets the string value associated with <paramref name="key"/>,
    /// or <paramref name="defaultValue"/> when the key is absent or is not a string.
    /// </summary>
    public string GetString(string key, string defaultValue) => GetValue(key)?.AsString() ?? defaultValue;

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

    /// <summary>
    /// Gets the integer value associated with <paramref name="key"/>,
    /// or <paramref name="defaultValue"/> when the key is absent or is not a number.
    /// </summary>
    public int GetInt32(string key, int defaultValue = 0) => GetValue(key)?.AsInt32() ?? defaultValue;

    /// <summary>
    /// Gets the integer value associated with <paramref name="key"/>,
    /// or <paramref name="defaultValue"/> when the key is absent or is not a number.
    /// </summary>
    public long GetInt64(string key, long defaultValue = 0) => GetValue(key)?.AsInt64() ?? defaultValue;

    /// <summary>
    /// Gets the float value associated with <paramref name="key"/>,
    /// or <paramref name="defaultValue"/> when the key is absent or is not a number.
    /// </summary>
    public float GetFloat(string key, float defaultValue) => GetValue(key)?.AsFloat() ?? defaultValue;

    /// <summary>
    /// Gets the double value associated with <paramref name="key"/>,
    /// or <paramref name="defaultValue"/> when the key is absent or is not a number.
    /// </summary>
    public double GetDouble(string key, double defaultValue = 0d) => GetValue(key)?.AsDouble() ?? defaultValue;

    /// <summary>
    /// Gets the boolean value associated with <paramref name="key"/>,
    /// or <paramref name="defaultValue"/> when the key is absent or is not a boolean.
    /// </summary>
    public bool GetBool(string key, bool defaultValue = false) => GetValue(key)?.AsBool() ?? defaultValue;

    /// <summary>The values in declaration order.</summary>
    public IEnumerable<MDLValue> Values()
    {
        foreach (var pair in _pairs)
            yield return pair.Value;
    }
}
