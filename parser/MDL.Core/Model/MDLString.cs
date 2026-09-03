namespace MDL.Core.Model
{
    /// <summary>
    /// A string value: quoted, raw or bare string from the MDL grammar.
    /// </summary>
    public sealed class MDLString : MDLValue
    {
        public MDLString(string value)
        {
            Value = value;
        }

        public override MdlValueKind Kind => MdlValueKind.String;

        /// <summary>The string content with escapes already resolved.</summary>
        public string Value { get; }
    }
}
