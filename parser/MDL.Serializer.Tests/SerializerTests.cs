using MDL.Core;
using MDL.Parser;
using MDL.Serializer;
using Xunit;

namespace MDL.Serializer.Tests
{
    public class SerializerTests
    {
        private class Target
        {
            public int IntProp { get; set; }
            public long LongProp { get; set; }
            public float FloatProp { get; set; }
            public double DoubleProp { get; set; }
        }

        [Fact]
        public void Deserialize_Integer_ToInt_Truncates()
        {
            var doc = new MdlParser().Parse("IntProp 42\nLongProp 100\n");
            var t = MDLSerializer.Deserialize<Target>(doc);
            Assert.Equal(42, t.IntProp);
            Assert.Equal(100L, t.LongProp);
        }

        [Fact]
        public void Deserialize_Float_ToDouble_Preserves()
        {
            var doc = new MdlParser().Parse("DoubleProp 3.14\nFloatProp 2.5\n");
            var t = MDLSerializer.Deserialize<Target>(doc);
            Assert.Equal(3.14, t.DoubleProp);
            Assert.Equal(2.5f, t.FloatProp);
        }

        [Fact]
        public void Deserialize_Integer_ToFloat_Works()
        {
            var doc = new MdlParser().Parse("FloatProp 10\n");
            var t = MDLSerializer.Deserialize<Target>(doc);
            Assert.Equal(10f, t.FloatProp);
        }

        [Fact]
        public void Deserialize_Integer_ToDouble_Works()
        {
            var doc = new MdlParser().Parse("DoubleProp 7\n");
            var t = MDLSerializer.Deserialize<Target>(doc);
            Assert.Equal(7.0, t.DoubleProp);
        }
    }
}