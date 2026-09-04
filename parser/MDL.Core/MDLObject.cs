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
    public MDLValue? TryGetValue(string key)
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
    public MDLValue? GetValue(string key, MDLValue? defaultValue = null) => TryGetValue(key) ?? defaultValue;

    /// <summary>
    /// Appends a key/value pair to the object. Duplicate keys are permitted
    /// and preserve declaration order (mirroring the source MDL document).
    /// </summary>
    public void Add(string key, MDLValue value) => _pairs.Add(new MDLPair(key, value));

    /// <summary>Appends an existing pair to the object.</summary>
    public void Add(MDLPair pair) => _pairs.Add(pair);

    // ── Structural: Object / List ──

    /// <summary>
    /// Gets the object associated with <paramref name="key"/>, or <see langword="null"/>
    /// when the key is absent or is not an object.
    /// </summary>
    public MDLObject? TryGetObject(string key) => TryGetValue(key) as MDLObject;

    /// <summary>
    /// Gets the object associated with <paramref name="key"/>,
    /// or a new empty <see cref="MDLObject"/> when the key is absent or is not an object.
    /// </summary>
    public MDLObject GetObject(string key) => TryGetObject(key) ?? new MDLObject();

    /// <summary>
    /// Gets the list associated with <paramref name="key"/>, or <see langword="null"/>
    /// when the key is absent or is not a list.
    /// </summary>
    public MDLList? TryGetList(string key) => TryGetValue(key) as MDLList;

    /// <summary>
    /// Gets the list associated with <paramref name="key"/>,
    /// or a new empty <see cref="MDLList"/> when the key is absent or is not a list.
    /// </summary>
    public MDLList GetList(string key) => TryGetList(key) ?? new MDLList();

    // ── String ──

    /// <summary>
    /// Gets the string value associated with <paramref name="key"/>,
    /// or <see langword="null"/> when the key is absent or is not a string.
    /// </summary>
    public string? TryGetString(string key) => TryGetValue(key)?.AsString();

    /// <summary>
    /// Gets the string value associated with <paramref name="key"/>,
    /// or <paramref name="defaultValue"/> when the key is absent or is not a string.
    /// </summary>
    public string GetString(string key, string defaultValue = "") => TryGetString(key) ?? defaultValue;

    // ── Int32 ──

    /// <summary>
    /// Gets the 32-bit integer value associated with <paramref name="key"/>,
    /// or <see langword="null"/> when the key is absent or is not a number.
    /// </summary>
    public int? TryGetInt32(string key) => TryGetValue(key)?.AsInt32();

    /// <summary>
    /// Gets the 32-bit integer value associated with <paramref name="key"/>,
    /// or <paramref name="defaultValue"/> when the key is absent or is not a number.
    /// </summary>
    public int GetInt32(string key, int defaultValue = 0) => TryGetInt32(key) ?? defaultValue;

    // ── Int64 ──

    /// <summary>
    /// Gets the 64-bit integer value associated with <paramref name="key"/>,
    /// or <see langword="null"/> when the key is absent or is not a number.
    /// </summary>
    public long? TryGetInt64(string key) => TryGetValue(key)?.AsInt64();

    /// <summary>
    /// Gets the 64-bit integer value associated with <paramref name="key"/>,
    /// or <paramref name="defaultValue"/> when the key is absent or is not a number.
    /// </summary>
    public long GetInt64(string key, long defaultValue = 0) => TryGetInt64(key) ?? defaultValue;

    // ── Float ──

    /// <summary>
    /// Gets the 32-bit floating-point value associated with <paramref name="key"/>,
    /// or <see langword="null"/> when the key is absent or is not a number.
    /// </summary>
    public float? TryGetFloat(string key) => TryGetValue(key)?.AsFloat();

    /// <summary>
    /// Gets the 32-bit floating-point value associated with <paramref name="key"/>,
    /// or <paramref name="defaultValue"/> when the key is absent or is not a number.
    /// </summary>
    public float GetFloat(string key, float defaultValue = 0f) => TryGetFloat(key) ?? defaultValue;

    // ── Double ──

    /// <summary>
    /// Gets the 64-bit floating-point value associated with <paramref name="key"/>,
    /// or <see langword="null"/> when the key is absent or is not a number.
    /// </summary>
    public double? TryGetDouble(string key) => TryGetValue(key)?.AsDouble();

    /// <summary>
    /// Gets the 64-bit floating-point value associated with <paramref name="key"/>,
    /// or <paramref name="defaultValue"/> when the key is absent or is not a number.
    /// </summary>
    public double GetDouble(string key, double defaultValue = 0d) => TryGetDouble(key) ?? defaultValue;

    // ── Bool ──

    /// <summary>
    /// Gets the boolean value associated with <paramref name="key"/>,
    /// or <see langword="null"/> when the key is absent or is not a boolean.
    /// </summary>
    public bool? TryGetBool(string key) => TryGetValue(key)?.AsBool();

    /// <summary>
    /// Gets the boolean value associated with <paramref name="key"/>,
    /// or <paramref name="defaultValue"/> when the key is absent or is not a boolean.
    /// </summary>
    public bool GetBool(string key, bool defaultValue = false) => TryGetBool(key) ?? defaultValue;

    /// <summary>The values in declaration order.</summary>
    public IEnumerable<MDLValue> Values()
    {
        foreach (var pair in _pairs)
            yield return pair.Value;
    }
}
