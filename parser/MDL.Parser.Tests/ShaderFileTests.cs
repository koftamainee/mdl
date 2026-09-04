using MDL.Core;
using MDL.Parser;
using Xunit;

namespace MDL.Parser.Tests
{
    public class ShaderFileTests
    {
        private static string ShaderSource() => System.IO.File.ReadAllText("assets/shader.mdl");

        [Fact]
        public void Parse_Shader_AllConstructsAreCorrect()
        {
            var doc = new MDLParser().Parse(ShaderSource());

            Assert.Equal(8, doc.Root.Count);
            Assert.IsType<MDLString>(doc.Root.TryGetValue("shader"));
            Assert.Equal("pbr", ((MDLString)doc.Root.TryGetValue("shader")!).Value);

            var stages = Assert.IsType<MDLObject>(doc.Root.TryGetValue("stages"));
            Assert.Equal(2, stages.Count);
            var vertex = Assert.IsType<MDLObject>(stages.TryGetValue("vertex"));
            Assert.Equal(2, vertex.Count);
            Assert.Equal("shaders/pbr.slang", ((MDLString)vertex.TryGetValue("src")!).Value);
            Assert.Equal("vs_main", ((MDLString)vertex.TryGetValue("entry")!).Value);
            var fragment = Assert.IsType<MDLObject>(stages.TryGetValue("fragment"));
            Assert.Equal("shaders/pbr.slang", ((MDLString)fragment.TryGetValue("src")!).Value);
            Assert.Equal("fs_main", ((MDLString)fragment.TryGetValue("entry")!).Value);

            var pipeline = Assert.IsType<MDLObject>(doc.Root.TryGetValue("pipeline"));
            Assert.True(((MDLBoolean)pipeline.TryGetValue("depth_test")!).Value);
            Assert.True(((MDLBoolean)pipeline.TryGetValue("depth_write")!).Value);
            Assert.Equal("opaque", ((MDLString)pipeline.TryGetValue("blend")!).Value);
            Assert.Equal("back", ((MDLString)pipeline.TryGetValue("cull_mode")!).Value);
            Assert.Equal("fill", ((MDLString)pipeline.TryGetValue("polygon_mode")!).Value);
        }

        [Fact]
        public void Parse_Shader_ListsAndQuotedStrings()
        {
            var doc = new MDLParser().Parse(ShaderSource());

            var list = Assert.IsType<MDLList>(doc.Root.TryGetValue("some_list"));
            Assert.Equal(2, list.Count);
            Assert.Equal("Monday", ((MDLString)list.Items[0]).Value);
            Assert.Equal("Tuesday", ((MDLString)list.Items[1]).Value);

            Assert.Equal("Hello world!", ((MDLString)doc.Root.TryGetValue("string_with_spaces")!).Value);

            Assert.Equal("#FFFFFF", ((MDLString)doc.Root.TryGetValue("black_color")!).Value);
        }

        [Fact]
        public void Parse_Shader_RawStrings()
        {
            var doc = new MDLParser().Parse(ShaderSource());

            Assert.Equal(@"C:\Projects\Mantle", ((MDLString)doc.Root.TryGetValue("windows_path")!).Value);

            Assert.Equal("Hello ` World!", ((MDLString)doc.Root.TryGetValue("hello2")!).Value);
        }
    }
}