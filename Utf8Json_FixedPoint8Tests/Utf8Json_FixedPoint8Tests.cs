using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Gitan.FixedPoint8.Tests
{
    [TestClass()]
    public class Utf8Json_FixedPoint8Tests
    {
   
        [TestMethod()]
        public void ReadWriteFixedPoint8TestAll()
        {
            CheckFixedPoint8("+0,"u8, "0");
            CheckFixedPoint8("-0,"u8, "0");
            CheckFixedPoint8("0,"u8, "0");
            CheckFixedPoint8("000,"u8, "0");
            CheckFixedPoint8("123.456,"u8, "123.456");
            CheckFixedPoint8("-123.456,"u8, "-123.456");
            CheckFixedPoint8("123456e4,"u8, "1234560000");
            CheckFixedPoint8("0.123456789e4,"u8, "1234.56789");
            CheckFixedPoint8("0.123456e4,"u8, "1234.56");
            CheckFixedPoint8("0.123456e-4,"u8, "0.00001234");
            CheckFixedPoint8("-123.456e4,"u8, "-1234560");
            CheckFixedPoint8("-0.123456e-4,"u8, "-0.00001234");
            CheckFixedPoint8("12345678.12345678E-12,"u8, "0.00001234");
            CheckFixedPoint8("92.23372036854775807e9,"u8, "92233720368.54775807");
            CheckFixedPoint8("-922337203685477580.8e-7,"u8, "-92233720368.54775808");
            CheckFixedPoint8("0.00000001,"u8, "0.00000001");
            CheckFixedPoint8("0.000000001,"u8, "0");
            CheckFixedPoint8("12.0456,"u8, "12.0456");
        }

        public void CheckFixedPoint8(ReadOnlySpan<byte> json,string resultString)
        {
            var jsonArray = json.ToArray();
            var reader = new Utf8Json.JsonReader(jsonArray);
            var result = reader.ReadFixedPoint8();
            Assert.IsTrue(result.ToString() == resultString);

            var writer = new Utf8Json.JsonWriter(sharedBuffer);
            writer.WriteFixedPoint8(result);

            var result2 = Encoding.UTF8.GetString(writer.GetBuffer().ToArray());
            Assert.IsTrue(result2 == resultString);
        }

        readonly byte[] sharedBuffer = new byte[65535];
    }
}