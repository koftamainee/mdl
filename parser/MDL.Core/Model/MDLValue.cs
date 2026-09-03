namespace MDL.Core.Model
{
    /// <summary>
    /// Base class for every value stored in an MDL document.
    /// Concrete kinds: <see cref="MDLObject"/>, <see cref="MDLList"/>,
    /// <see cref="MDLInteger"/>, <see cref="MDLFloat"/>,
    /// <see cref="MDLBoolean"/> and <see cref="MDLString"/>.
    /// </summary>
    public abstract class MDLValue
    {
        /// <summary>The discriminator tag for this value node.</summary>
        public abstract MdlValueKind Kind { get; }

        /// <summary>
        /// Allocates a new <see cref="MDLInteger"/> value.
        /// </summary>
        public static MDLValue Integer(long value) => new MDLInteger(value);

        /// <summary>
        /// Allocates a new <see cref="MDLFloat"/> value.
        /// </summary>
        public static MDLValue Float(double value) => new MDLFloat(value);

        /// <summary>
        /// Allocates a new <see cref="MDLBoolean"/> value.
        /// </summary>
        public static MDLValue Boolean(bool value) => new MDLBoolean(value);

        /// <summary>
        /// Allocates a new <see cref="MDLString"/> value.
        /// </summary>
        public static MDLValue String(string value) => new MDLString(value);
    }
}
