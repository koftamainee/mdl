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
            var doc = new MdlParser().Parse(ShaderSource());

            Assert.Equal(8, doc.Root.Count);
            Assert.IsType<MDLString>(doc.Root.GetValue("shader"));
            Assert.Equal("pbr", ((MDLString)doc.Root.GetValue("shader")!).Value);

            var stages = Assert.IsType<MDLObject>(doc.Root.GetValue("stages"));
            Assert.Equal(2, stages.Count);
            var vertex = Assert.IsType<MDLObject>(stages.GetValue("vertex"));
            Assert.Equal(2, vertex.Count);
            Assert.Equal("shaders/pbr.slang", ((MDLString)vertex.GetValue("src")!).Value);
            Assert.Equal("vs_main", ((MDLString)vertex.GetValue("entry")!).Value);
            var fragment = Assert.IsType<MDLObject>(stages.GetValue("fragment"));
            Assert.Equal("shaders/pbr.slang", ((MDLString)fragment.GetValue("src")!).Value);
            Assert.Equal("fs_main", ((MDLString)fragment.GetValue("entry")!).Value);

            var pipeline = Assert.IsType<MDLObject>(doc.Root.GetValue("pipeline"));
            Assert.True(((MDLBoolean)pipeline.GetValue("depth_test")!).Value);
            Assert.True(((MDLBoolean)pipeline.GetValue("depth_write")!).Value);
            Assert.Equal("opaque", ((MDLString)pipeline.GetValue("blend")!).Value);
            Assert.Equal("back", ((MDLString)pipeline.GetValue("cull_mode")!).Value);
            Assert.Equal("fill", ((MDLString)pipeline.GetValue("polygon_mode")!).Value);
        }

        [Fact]
        public void Parse_Shader_ListsAndQuotedStrings()
        {
            var doc = new MdlParser().Parse(ShaderSource());

            var list = Assert.IsType<MDLList>(doc.Root.GetValue("some_list"));
            Assert.Equal(2, list.Count);
            Assert.Equal("Monday", ((MDLString)list.Items[0]).Value);
            Assert.Equal("Tuesday", ((MDLString)list.Items[1]).Value);

            Assert.Equal("Hello world!", ((MDLString)doc.Root.GetValue("string_with_spaces")!).Value);

            Assert.Equal("#FFFFFF", ((MDLString)doc.Root.GetValue("black_color")!).Value);
        }

        [Fact]
        public void Parse_Shader_RawStrings()
        {
            var doc = new MdlParser().Parse(ShaderSource());

            Assert.Equal(@"C:\Projects\Mantle", ((MDLString)doc.Root.GetValue("windows_path")!).Value);

            Assert.Equal("Hello ` World!", ((MDLString)doc.Root.GetValue("hello2")!).Value);
        }
    }
}