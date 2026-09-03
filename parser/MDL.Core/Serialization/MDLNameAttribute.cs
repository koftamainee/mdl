using System;

namespace MDL.Core.Serialization
{
    /// <summary>
    /// Overrides the key used to map a member of a CLR type to a key in an
    /// MDL object. When absent, the member name is used as the key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class MDLNameAttribute : Attribute
    {
        public MDLNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>The MDL object key to map to.</summary>
        public string Name { get; }
    }
}
