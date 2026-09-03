using System;
using System.Collections.Generic;

namespace MDL.Core.Model
{
    /// <summary>
    /// An ordered list of values, corresponding to <c>[ ... ]</c> in the MDL grammar.
    /// </summary>
    public sealed class MDLList : MDLValue
    {
        public override MdlValueKind Kind => MdlValueKind.List;
        private readonly List<MDLValue> _items = new List<MDLValue>();

        /// <summary>The contained items.</summary>
        public IReadOnlyList<MDLValue> Items => _items;

        /// <summary>The number of items.</summary>
        public int Count => _items.Count;

        /// <summary>Appends an item to the list.</summary>
        public void Add(MDLValue value) => _items.Add(value);
    }
}
