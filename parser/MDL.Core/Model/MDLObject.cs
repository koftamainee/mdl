using System;
using System.Collections.Generic;

namespace MDL.Core.Model
{
    /// <summary>
    /// An ordered set of key/value pairs, the container used at the root of a document
    /// and for <c>{ ... }</c> objects in the MDL grammar.
    /// </summary>
    public sealed class MDLObject : MDLValue
    {
        public override MdlValueKind Kind => MdlValueKind.Object;
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
    }
}
