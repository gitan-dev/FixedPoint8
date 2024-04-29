using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gitan.FixedPoint8.Tests
{
    [TestClass()]
    public class BenchMark_CharactersTests
    {
        static readonly BenchMark_Characters instance = new();

        /////////////////////////////////////// ROS Parse


        [TestMethod()]
        public void ParseTests()
        {
            var resultInt = instance.StringToInt();
            Assert.IsTrue(resultInt == -1);

            var resultDouble = instance.StringToDouble();
            Assert.IsTrue(resultDouble == -0.12345678);

            var resultDecimal = instance.StringToDecimal();
            Assert.IsTrue(resultDecimal == -0.12345678m);

            var resultStringFp8 = instance.StringToFixedPoint8();
            Assert.IsTrue(resultStringFp8 == new FixedPoint8(-12_345_678));
            Assert.IsTrue(resultDecimal == (decimal)resultStringFp8);

            var resultUtf8Fp8 = instance.Utf8ToFixedPoint8();
            Assert.IsTrue(resultUtf8Fp8 == new FixedPoint8(-12_345_678));
            Assert.IsTrue(resultDecimal == (decimal)resultUtf8Fp8);

            var resultCharFp8 = instance.CharArrayToFixedPoint8();
            Assert.IsTrue(resultCharFp8 == new FixedPoint8(-12_345_678));
            Assert.IsTrue(resultDecimal == (decimal)resultCharFp8);
        }

        /////////////////////////////////////// GetUtf8


        [TestMethod()]
        public void Tostring_Utf8Tests()
        {
            var resultInt = instance.IntToString();
            Assert.IsTrue(resultInt == 103);

            var resultDouble = instance.DoubleToString();
            Assert.IsTrue(resultDouble == 85);

            var resultDecimal = instance.DecimalToString();
            Assert.IsTrue(resultDecimal == 85);

            var resultFp8 = instance.FixedPoint8ToString();
            Assert.IsTrue(resultFp8 == 85);
            Assert.IsTrue(resultDecimal == resultFp8);

            var resultFp8Utf8 = instance.FixedPoint8ToUtf8();
            Assert.IsTrue(resultFp8Utf8 == resultDecimal);
        }


        /////////////////////////////////////// WriteCharsUtf8

#pragma warning disable IDE0042
        [TestMethod()]
        public void WriteUtf8CharsTest()
        {
            var tryWriteChars = instance.FixedPoint8_TryWriteChars();

            var origin = tryWriteChars.buffer.AsSpan(0, tryWriteChars.offset);

            var writeChars = instance.FixedPoint8_WriteChars();

            var check = writeChars.buffer.AsSpan(0, writeChars.offset);
            Assert.IsTrue(origin.SequenceEqual(check));

            var tryWriteUtf8 = instance.FixedPoint8_TryWriteUtf8();

            check = tryWriteUtf8.buffer[..tryWriteUtf8.offset].Select(x => (char)x).ToArray();
            Assert.IsTrue(origin.SequenceEqual(check));

            var writeUtf8 = instance.FixedPoint8_WriteUtf8();

            check = writeUtf8.buffer[..writeUtf8.offset].Select(x => (char)x).ToArray();
            Assert.IsTrue(origin.SequenceEqual(check));
        }

        /////////////////////////////////////// WriteCharsUtf8IBufferWriter
        
        [TestMethod()]
        public void WriteUtf8CharsIbufferWriterTest()
        {
            var writeChars = instance.FixedPoint8_WriteChars();
            var writeCharsIBuffer = instance.FixedPoint8_WriteCharsIBufferWriter();
            Assert.IsTrue(writeChars.buffer.AsSpan(0, writeChars.offset).SequenceEqual(writeCharsIBuffer.WrittenSpan));

            var writeUtf8 = instance.FixedPoint8_WriteUtf8();
            var writeUtf8IBuffer = instance.FixedPoint8_WriteUtf8IBufferWriter();
            Assert.IsTrue(writeUtf8.buffer.AsSpan(0, writeUtf8.offset).SequenceEqual(writeUtf8IBuffer.WrittenSpan));
        }
#pragma warning restore IDE0042
    }
}