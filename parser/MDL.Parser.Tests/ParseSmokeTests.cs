using MDL.Core;
using MDL.Parser;
using Xunit;

namespace MDL.Parser.Tests
{
    public class ParseSmokeTests
    {
        [Fact]
        public void Parse_ReturnsDocument()
        {
            var doc = new MDLParser().Parse("hp 100\nname \"hero\"\n");

            Assert.NotNull(doc.Root);
            Assert.Equal(2, doc.Root.Count);
        }

        [Fact]
        public void Parse_ObjectNesting_ProducesObjectValues()
        {
            var doc = new MDLParser().Parse("player { hp 100\n speed 5.5 }");

            var player = doc.Root.GetValue("player");
            Assert.IsType<MDLObject>(player);
            Assert.Equal(2, ((MDLObject)player).Count);
        }

        [Fact]
        public void Parse_List_ProducesListValues()
        {
            var doc = new MDLParser().Parse("tags [a b \"c d\"]");

            var tags = Assert.IsType<MDLList>(doc.Root.GetValue("tags"));
            Assert.Equal(3, tags.Count);
        }

        [Fact]
        public void Parse_ValueKinds_AreCorrect()
        {
            var doc = new MDLParser().Parse("i 10\nf 1.5\ne 1e3\nb true\ns \"x\\\"y\"");

            Assert.IsType<MDLInteger>(doc.Root.GetValue("i"));
            Assert.Equal(10L, ((MDLInteger)doc.Root.GetValue("i")!).Value);
            Assert.IsType<MDLFloat>(doc.Root.GetValue("f"));
            Assert.IsType<MDLFloat>(doc.Root.GetValue("e"));
            Assert.Equal(1e3, ((MDLFloat)doc.Root.GetValue("e")!).Value);
            Assert.IsType<MDLBoolean>(doc.Root.GetValue("b"));
            Assert.Equal("x\"y", ((MDLString)doc.Root.GetValue("s")!).Value);
        }

        [Fact]
        public void Parse_IntegerOverflow_Throws()
        {
            Assert.Throws<MDLParserException>(() =>
                new MDLParser().Parse("val 99999999999999999999"));
        }

        [Fact]
        public void Parse_BareString_IsString()
        {
            var doc = new MDLParser().Parse("kind hero");

            var kind = Assert.IsType<MDLString>(doc.Root.GetValue("kind"));
            Assert.Equal("hero", kind.Value);
        }

        [Fact]
        public void Parse_CommentsAreSkipped()
        {
            var doc = new MDLParser().Parse("# top comment\nhp 100 # inline\n");

            Assert.Equal(1, doc.Root.Count);
        }

        [Fact]
        public void Parse_Invalid_ThrowsWithPosition()
        {
            var ex = Assert.Throws<MDLParserException>(() => new MDLParser().Parse("a { b } c )"));
            Assert.True(ex.Line >= 1);
        }
    }
}