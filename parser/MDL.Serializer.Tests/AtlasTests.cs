using System;
using System.Collections.Generic;
using MDL.Parser;
using MDL.Serializer;
using Xunit;

namespace MDL.Serializer.Tests
{
    public class TextureRegion
    {
        public string Name { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class AtlasAnimation
    {
        public string Name { get; set; } = "";
        public int Delay { get; set; }
        public List<string> Frames { get; set; } = new List<string>();
    }

    public class TextureAtlas
    {
        public string Texture { get; set; } = "";
        public List<TextureRegion> Regions { get; set; } = new List<TextureRegion>();
        public List<AtlasAnimation> Animations { get; set; } = new List<AtlasAnimation>();
    }

    public class AtlasTests
    {
        [Fact]
        public void Deserialize_AtlasMDL_PopulatesAllData()
        {
            var atlas = MDLSerializer.Deserialize<TextureAtlas>(new MDLParser().Parse(System.IO.File.ReadAllText("assets/atlas.mdl")));


            Assert.Equal("Images/Atlas", atlas.Texture);

            Assert.Equal(5, atlas.Regions.Count);
            AssertRegion(atlas.Regions[0], "slime-1", 0, 0, 20, 20);
            AssertRegion(atlas.Regions[1], "slime-2", 0, 20, 20, 20);
            AssertRegion(atlas.Regions[2], "bat-1", 20, 0, 20, 20);
            AssertRegion(atlas.Regions[3], "bat-2", 20, 20, 20, 20);
            AssertRegion(atlas.Regions[4], "bat-3", 40, 0, 20, 20);

            Assert.Equal(2, atlas.Animations.Count);

            var anim = atlas.Animations[0];
            Assert.Equal("slime-animation", anim.Name);
            Assert.Equal(200, anim.Delay);
            Assert.Equal(new[] { "slime-1", "slime-2" }, anim.Frames);

            var bat = atlas.Animations[1];
            Assert.Equal("bat-animation", bat.Name);
            Assert.Equal(200, bat.Delay);
            Assert.Equal(new[] { "bat-1", "bat-2", "bat-1", "bat-3" }, bat.Frames);
        }

        private static void AssertRegion(TextureRegion reg, string name, int x, int y, int w, int h)
        {
            Assert.Equal(name, reg.Name);
            Assert.Equal(x, reg.X);
            Assert.Equal(y, reg.Y);
            Assert.Equal(w, reg.Width);
            Assert.Equal(h, reg.Height);
        }
    }
}