using Microsoft.Xna.Framework.Content;

using MDL.Core;

namespace MDL.Content;

public class MDLReader : ContentTypeReader<MDLDocument>
{
    protected override MDLDocument Read(ContentReader input, MDLDocument existingInstance)
    {
        // Object kind
        input.ReadByte();

        MDLObject root = ReadObject(input);
        return new MDLDocument(root);
    }

    private MDLObject ReadObject(ContentReader input)
    {
        int count = input.ReadInt32();
        var obj = new MDLObject();

        for (int i = 0; i < count; i++)
        {
            string key = input.ReadString();
            MDLValue value = ReadValue(input);

            obj.Add(key, value);
        }

        return obj;
    }

    private MDLList ReadList(ContentReader input)
    {
        int count = input.ReadInt32();

        var list = new MDLList();

        for (int i = 0; i < count; i++)
        {
            list.Add(ReadValue(input));
        }

        return list;
    }

    private MDLValue ReadValue(ContentReader input)
    {
        return (MDLValueKind)input.ReadByte() switch
        {
            MDLValueKind.Object => ReadObject(input),
            MDLValueKind.List => ReadList(input),
            MDLValueKind.String => new MDLString(input.ReadString()),
            MDLValueKind.Integer => new MDLInteger(input.ReadInt64()),
            MDLValueKind.Float => new MDLFloat(input.ReadDouble()),
            MDLValueKind.Boolean => new MDLBoolean(input.ReadBoolean()),
            _ => throw new InvalidOperationException($"Unsupported MDL value kind: ..."),
        };
    }
}
