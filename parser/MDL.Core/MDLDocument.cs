namespace MDL.Core;

/// <summary>
/// The root of a parsed MDL document. Wraps the top-level object plus
/// optional provenance information such as the source path.
/// </summary>
public sealed class MDLDocument
{
    public MDLDocument(MDLObject root)
        : this(root, null)
    {
    }

    public MDLDocument(MDLObject root, string? sourcePath)
    {
        Root = root;
        SourcePath = sourcePath;
    }

    /// <summary>The top-level object of the document.</summary>
    public MDLObject Root { get; }

    /// <summary>The file path the document was loaded from, when known.</summary>
    public string? SourcePath { get; }

    /// <summary>
    /// Gets the value associated with <paramref name="key"/> at the document root.
    /// </summary>
    public MDLValue? this[string key] => Root.GetValue(key);
}
