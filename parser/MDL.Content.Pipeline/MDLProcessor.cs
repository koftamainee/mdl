using Microsoft.Xna.Framework.Content.Pipeline;

using MDL.Core;

namespace MDL.Content.Pipeline;

[ContentProcessor(DisplayName = "MDL Processor")]
public class MDLProcessor : ContentProcessor<MDLDocument, MDLDocument>
{
    public override MDLDocument Process(MDLDocument input, ContentProcessorContext context)
    {
        return input;
    }
}
