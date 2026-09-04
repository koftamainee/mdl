using Microsoft.Xna.Framework.Content.Pipeline;

using MDL.Core;

namespace MDL.Content.Pipeline;

/// <summary>Pass-through content processor for <see cref="MDLDocument"/>.</summary>
[ContentProcessor(DisplayName = "MDL Processor")]
public class MDLProcessor : ContentProcessor<MDLDocument, MDLDocument>
{
    /// <inheritdoc/>
    public override MDLDocument Process(MDLDocument input, ContentProcessorContext context)
    {
        return input;
    }
}
