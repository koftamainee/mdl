using System;
using System.IO;
using MDL.Core.Model;
using MDL.Core.Parsing;
using MDL.Core.Serialization;

namespace MDL.Core
{
    /// <summary>
    /// High-level facade over MDL parsing and deserialization.
    /// </summary>
    public static class MDL
    {
        private static readonly System.Threading.ThreadLocal<MdlParser> Parser =
            new System.Threading.ThreadLocal<MdlParser>(() => new MdlParser());

        /// <summary>
        /// Parses MDL source text into a document (raw value tree).
        /// </summary>
        public static MDLDocument Parse(string source) => Parser.Value.Parse(source);

        /// <summary>
        /// Reads an MDL file from <paramref name="path"/> and returns its raw
        /// document tree.
        /// </summary>
        public static MDLDocument Read(string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            string source = File.ReadAllText(path);
            return Parser.Value.Parse(source);
        }

        /// <summary>
        /// Deserializes MDL source text into an instance of <typeparamref name="T"/>.
        /// </summary>
        public static T Deserialize<T>(string source) => MDLSerializer.Deserialize<T>(Parser.Value.Parse(source));

        /// <summary>
        /// Deserializes an <see cref="MDLDocument"/> into an instance of
        /// <typeparamref name="T"/>.
        /// </summary>
        public static T Deserialize<T>(MDLDocument document) => MDLSerializer.Deserialize<T>(document);

        /// <summary>
        /// Loads an MDL file from <paramref name="path"/> and deserializes it into
        /// an instance of <typeparamref name="T"/>.
        /// </summary>
        public static T Load<T>(string path) => MDLSerializer.Deserialize<T>(Read(path));
    }
}
