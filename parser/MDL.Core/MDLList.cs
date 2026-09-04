using System.Collections.Generic;
using System.Linq;

namespace MDL.Core;

/// <summary>
/// An ordered list of values, corresponding to <c>[ ... ]</c> in the MDL grammar.
/// </summary>
public sealed class MDLList : MDLValue
{
    /// <inheritdoc/>
    public override MDLValueKind Kind => MDLValueKind.List;
    private readonly List<MDLValue> _items = new List<MDLValue>();

    /// <summary>The contained items.</summary>
    public IReadOnlyList<MDLValue> Items => _items;

    /// <summary>The number of items.</summary>
    public int Count => _items.Count;

    /// <summary>Appends an item to the list.</summary>
    public void Add(MDLValue value) => _items.Add(value);

    /// <summary>The items in order.</summary>
    public IEnumerable<MDLValue> Values() => _items;

    /// <summary>The items that are objects, in order.</summary>
    public IEnumerable<MDLObject> Objects() => _items.OfType<MDLObject>();

    /// <summary>The items that are lists, in order.</summary>
    public IEnumerable<MDLList> Lists() => _items.OfType<MDLList>();

    /// <summary>String values in order.</summary>
    public IEnumerable<string> Strings()
    {
        foreach (var item in _items)
            if (item is MDLString s)
                yield return s.Value;
    }

    /// <summary>Int32 values in order.</summary>
    public IEnumerable<int> Ints32()
    {
        foreach (var item in _items)
            if (item is MDLInteger i)
                yield return (int)i.Value;
    }

    /// <summary>Int64 values in order.</summary>
    public IEnumerable<long> Ints64()
    {
        foreach (var item in _items)
            if (item is MDLInteger i)
                yield return i.Value;
    }

    /// <summary>Float values in order.</summary>
    public IEnumerable<float> Floats()
    {
        foreach (var item in _items)
            if (item is MDLFloat f)
                yield return (float)f.Value;
    }

    /// <summary>Double values in order.</summary>
    public IEnumerable<double> Doubles()
    {
        foreach (var item in _items)
            if (item is MDLFloat f)
                yield return f.Value;
    }

    /// <summary>Boolean values in order.</summary>
    public IEnumerable<bool> Bools()
    {
        foreach (var item in _items)
            if (item is MDLBoolean b)
                yield return b.Value;
    }
}
