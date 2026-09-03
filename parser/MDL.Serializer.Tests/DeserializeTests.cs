using MDL.Parser;
using MDL.Serializer;
using Xunit;

namespace MDL.Serializer.Tests
{
    public class DeserializeTests
    {
        public enum Faction { Orcs, Humans }

        public class Character
        {
            public string Name { get; set; } = "";
            public int Hp { get; set; }
            public double Speed { get; set; }
            public bool IsBoss { get; set; }
            [MDLName("faction_name")]
            public Faction Faction { get; set; }
            public string[] Tags { get; set; } = System.Array.Empty<string>();
            [MDLIgnore]
            public string Ignored { get; set; } = "default";
        }

        [Fact]
        public void Deserialize_Primitives_AreMapped()
        {
            var c = MDLSerializer.Deserialize<Character>(new MdlParser().Parse(System.IO.File.ReadAllText("assets/character.mdl")));
            Assert.Equal("Grom", c.Name);
            Assert.Equal(100, c.Hp);
            Assert.Equal(5.5, c.Speed);
            Assert.False(c.IsBoss);
        }

        [Fact]
        public void Deserialize_Enum_ByString()
        {
            var c = MDLSerializer.Deserialize<Character>(new MdlParser().Parse(System.IO.File.ReadAllText("assets/character.mdl")));
            Assert.Equal(Faction.Orcs, c.Faction);
        }

        [Fact]
        public void Deserialize_AttributeName_AndIgnore()
        {
            var c = MDLSerializer.Deserialize<Character>(new MdlParser().Parse(System.IO.File.ReadAllText("assets/character.mdl")));
            Assert.Equal(Faction.Orcs, c.Faction);
            Assert.Equal("default", c.Ignored);
        }

        [Fact]
        public void Deserialize_ListToArray()
        {
            var c = MDLSerializer.Deserialize<Character>(new MdlParser().Parse(System.IO.File.ReadAllText("assets/character.mdl")));
            Assert.Equal(new[] { "sword", "shield" }, c.Tags);
        }

        [Fact]
        public void Deserialize_FromString()
        {
            var c = MDLSerializer.Deserialize<Character>("name \"X\"\nhp 5\n");
            Assert.Equal("X", c.Name);
            Assert.Equal(5, c.Hp);
        }

        [Fact]
        public void Deserialize_MissingKey_KeepsDefault()
        {
            var c = MDLSerializer.Deserialize<Character>("name \"X\"\n");
            Assert.Equal(0, c.Hp);
        }
    }
}