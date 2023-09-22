using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gitan.FixedPoint8.Tests
{
    [TestClass()]
    public class BenchMark_Utf8JsonTests
    {
        static readonly BenchMark_Utf8Json instance = new();

        /////////////////////////////////////// Reader
        

        [TestMethod()]
        public void ReaderTest()
        {
            var resultInt = instance.ReadInt();
            Assert.IsTrue(resultInt == 0);

            var resultLong = instance.ReadLong();
            Assert.IsTrue(resultLong == 0);

            decimal d = 0m;
            var resultDouble = instance.ReadDouble();
            Assert.AreEqual((double)d, resultDouble, 0.00000001);

            var resultFp8 = instance.ReadFixedPoint8();
            Assert.IsTrue(resultFp8 == FixedPoint8.Zero);
            Assert.AreEqual((double)resultFp8,resultDouble, 0.00000001);
        }


        /////////////////////////////////////// Deserialize
        
        [TestMethod()]
        public void DeserializeTest()
        {
            var resultInt = instance.DeserializeInt();
            Assert.IsTrue(resultInt == 0);

            var resultDecimal = instance.DeserializeDecimal();
            Assert.IsTrue(resultDecimal == 0m);

            var resultDouble = instance.DeserializeDouble();
            Assert.AreEqual((double)resultDecimal, resultDouble, 0.00000001);

            var resultFp8 = instance.DeserializeFixedPoint8();
            Assert.IsTrue(resultFp8 == FixedPoint8.Zero);
            Assert.IsTrue(resultDecimal == (decimal)resultFp8);
        }


        /////////////////////////////////////// Writer

        [TestMethod()]
        public void WriterTest()
        {
            var resultInt = instance.WriteInt();
            Assert.IsTrue(resultInt == 103);

            var resultLong = instance.WriteLong();
            Assert.IsTrue(resultLong == 103);

            var resultDouble = instance.WriteDouble();
            Assert.IsTrue(resultDouble == 85);

            var resultFp8 = instance.WriteFixedPoint8();
            Assert.IsTrue(resultFp8 == 85);
            Assert.IsTrue(resultFp8 == resultDouble);
        }

        /////////////////////////////////////// Serialize

        [TestMethod()]
        public void SerializeIntTest()
        {
            var resultInt = instance.SerializeInt();
            Assert.IsTrue(resultInt == 273);

            var resultDouble = instance.SerializeDouble();
            Assert.IsTrue(resultDouble == 255);

            var resultDecimal = instance.SerializeDecimal();
            Assert.IsTrue(resultDecimal == 255);

            var resultFp8 = instance.SerializeFixedPoint8();
            Assert.IsTrue(resultFp8 == 255);
            Assert.IsTrue(resultDecimal == resultFp8);
        }

    }
}