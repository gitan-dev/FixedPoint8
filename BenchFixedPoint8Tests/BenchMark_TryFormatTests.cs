using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gitan.FixedPoint8.Tests
{
    [TestClass()]
    public class BenchMark_TryFormatTests
    {
        static readonly BenchMark_TryFormat instance = new();

        [TestMethod()]
        public void ReaderTest()
        {
            //var intFormat = instance.IntTryFormat();
            //var doubleFormat = instance.DoubleTryFormat();
            //var decimalFormat = instance.DecimalTryFormat();
            //var fp8Format = instance.FixedPoint8TryFormat();
            //var fp8DoubleFormat = instance.FixedPoint8DoubleTryFormat();
        }

#if NET8_0

        public void Test()
        {
            Span<byte> buffer = new byte[100];
            string value = "abc";
            byte[] value2 = "xyz"u8.ToArray();
            ReadOnlySpan<byte> value3 = "xyz"u8;
            var st = $"Value:{value}";

            System.Text.Unicode.Utf8.TryWrite(buffer, $"Value:{value3}", out int charsWritten);
        }
#endif
    }
}