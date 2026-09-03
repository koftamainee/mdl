using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

using MDL.Core;

namespace MDL.Content.Pipeline;


[ContentTypeWriter]
public class MDLWriter : ContentTypeWriter<MDLDocument>
{
    public override string GetRuntimeType(TargetPlatform targetPlatform)
    {
        return typeof(MDLDocument).AssemblyQualifiedName!;
    }

    public override string GetRuntimeReader(TargetPlatform targetPlatform)
    {
        return "MDL.Content.MDLReader, MDL.Content";
    }

    protected override void Write(ContentWriter output, MDLDocument value)
    {
        output.Write((byte)value.Root.Kind);
        WriteObject(output, value.Root);
    }

    private void WriteObject(ContentWriter output, MDLObject value)
    {
        output.Write(value.Count);
        foreach (var pair in value.Pairs)
        {
            output.Write(pair.Key);
            WriteValue(output, pair.Value);
        }
    }

    private void WriteList(ContentWriter output, MDLList value)
    {
        output.Write(value.Count);
        foreach (var item in value.Items)
            WriteValue(output, item);
    }

    private void WriteValue(ContentWriter output, MDLValue value)
    {
        output.Write((byte)value.Kind);
        switch (value.Kind)
        {
            case MDLValueKind.Object:
                WriteObject(output, (MDLObject)value);
                break;
            case MDLValueKind.List:
                WriteList(output, (MDLList)value);
                break;
            case MDLValueKind.String:
                output.Write(((MDLString)value).Value);
                break;
            case MDLValueKind.Integer:
                output.Write(((MDLInteger)value).Value);
                break;
            case MDLValueKind.Float:
                output.Write(((MDLFloat)value).Value);
                break;
            case MDLValueKind.Boolean:
                output.Write(((MDLBoolean)value).Value);
                break;
            default:
                throw new InvalidOperationException($"Unsupported MDL value kind: {value.Kind}");
        }
    }
}
