using System.Collections.Generic;
using System.Linq;

namespace MDL.Core;

/// <summary>
/// An ordered list of values, corresponding to <c>[ ... ]</c> in the MDL grammar.
/// </summary>
public sealed class MDLList : MDLValue
{
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

    /// <summary>The items that are strings, in order.</summary>
    public IEnumerable<MDLString> Strings() => _items.OfType<MDLString>();

    /// <summary>The items that are integers, in order.</summary>
    public IEnumerable<MDLInteger> Integers() => _items.OfType<MDLInteger>();

    /// <summary>The items that are floats, in order.</summary>
    public IEnumerable<MDLFloat> Floats() => _items.OfType<MDLFloat>();

    /// <summary>The items that are booleans, in order.</summary>
    public IEnumerable<MDLBoolean> Booleans() => _items.OfType<MDLBoolean>();
}
