using Microsoft.Xna.Framework.Content.Pipeline;

using MDL.Core;
using MDL.Parser;

namespace MDL.Content.Pipeline;

[ContentImporter(".mdl", DisplayName = "MDL Importer", DefaultProcessor = "MDLProcessor")]
public class MDLImporter : ContentImporter<MDLDocument>
{
    public override MDLDocument Import(string filename, ContentImporterContext context)
    {
        string data = File.ReadAllText(filename);
        var parser = new MDLParser();
        return parser.Parse(data);
    }
}
