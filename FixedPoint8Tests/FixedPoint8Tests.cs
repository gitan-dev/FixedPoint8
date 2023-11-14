using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Buffers;
using System.Text;

namespace Gitan.FixedPoint8.Tests;

[TestClass()]
public class FixedPoint8Tests
{
    public static List<decimal> GetDecimals()
    {
        var result = new List<decimal>()
        {
            0m,
            1m,
            13m,
            100m,
            0.03m,
            0.00001m,
            0.0000001m,
            1.0m,
            1.00909m,
            1.215m,
            1.225m,
            1.2215m,
            1.2225m,
            1.2345m,
            1.2356m,
            1.5m,
            1.5645m,
            1.5656m,
            2.5m,
            123.456m,
            123.789m,
            1234.56m,
            1234.5678m,
            5500.00m,
            6500.00m,
            123456.789m,
            1234567.89m,
            303699.99m,
            12345678912.12345678m,
            46116860184.27387903m,
            92233720368.54775806m,
            92233720368.54775807m,
            -1m,
            -13m,
            -100m,
            -0.03m,
            -0.00001m,
            -0.0000001m,
            -1.0m,
            -1.00909m,
            -1.215m,
            -1.225m,
            -1.2215m,
            -1.2225m,
            -1.2345m,
            -1.2356m,
            -1.5m,
            -1.5645m,
            -1.5656m,
            -2.5m,
            -123.456m,
            -123.789m,
            -1234.56m,
            -1234.5678m,
            -5500.00m,
            -6500.00m,
            -123456.789m,
            -1234567.89m,
            -303699.99m,
            -12345678912.12345678m,
            -46116860184.27387903m,
            -92233720368.54775807m,
            -92233720368.54775808m,
        };

        return result;
    }


    [TestMethod()]
    public void ToUtf8Test()
    {
        var list = GetDecimals();
        
        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            var fp8Utf8 = fp8.ToUtf8();
            var fp8String = Encoding.UTF8.GetString(fp8Utf8);

            var decimalString = decimal_a.ToString();

            // 小数点以下の右側の0を消す
            // "1.0200" → "1.02"
            if (decimalString.Contains('.'))
            {
                while (decimalString[^1..] == "0")
                {
                    decimalString = decimalString[..^1];
                }
                if (decimalString[^1..] == ".")
                {
                    decimalString = decimalString[..^1];
                }
            }
            Assert.IsTrue(fp8String == decimalString);
        }
    }

    [TestMethod()]
    public void CalcTest()
    {
        var list = GetDecimals();
        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                if (-46116860184.27387903m < decimal_a && decimal_a < 46116860184.27387903m
                    && -46116860184.27387903m < decimal_b && decimal_b < 46116860184.27387903m)
                {
                    var fp8ResultAdd = fp8_a + fp8_b;
                    Assert.IsTrue(fp8ResultAdd == (FixedPoint8)(decimal_a + decimal_b));

                    var fp8ResultSub = fp8_a - fp8_b;
                    Assert.IsTrue(fp8ResultSub == (FixedPoint8)(decimal_a - decimal_b));
                }

                if ((Math.Abs(decimal_a) < 303700) && (Math.Abs(decimal_b) < 303700))
                {
                    var fp8ResultMul = fp8_a * fp8_b;
                    Assert.IsTrue(fp8ResultMul == (FixedPoint8)(decimal_a * decimal_b));
                }

                if ((Math.Abs(decimal_a) < 303700) && (Math.Abs(decimal_b) > 1.0m / 303700))
                {
                    var fp8ResultDiv = fp8_a / fp8_b;
                    Assert.IsTrue(fp8ResultDiv == (FixedPoint8)(decimal_a / decimal_b));
                }
            }
        }
    }

    [TestMethod()]
    public void PercentTest()
    {
        var list = GetDecimals();
        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                decimal decimalResult = 0m;
                bool decimalException = false;
                try
                {
                    decimalResult = decimal_a % decimal_b;
                }
                catch
                {
                    decimalException = true;
                }

                FixedPoint8 fp8Result = FixedPoint8.Zero;
                bool fp8Exception = false;
                try
                {
                    fp8Result = fp8_a % fp8_b;
                }
                catch
                {
                    fp8Exception = true;
                }

                Assert.IsTrue(decimalException == fp8Exception);

                if (decimalException == false)
                {
                    Assert.IsTrue((FixedPoint8)decimalResult == fp8Result);
                }
            }
        }
    }


    [TestMethod()]
    public void CompTest()
    {
        var list = GetDecimals();

        // ==
        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                bool decimalResult = decimal_a == decimal_b;
                bool fp8Result = fp8_a == fp8_b;
                Assert.IsTrue(decimalResult == fp8Result);
            }
        }

        // !=
        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                bool decimalResult = decimal_a != decimal_b;
                bool fp8Result = fp8_a != fp8_b;
                Assert.IsTrue(decimalResult == fp8Result);
            }
        }

        // <
        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                bool decimalResult = decimal_a < decimal_b;
                bool fp8Result = fp8_a < fp8_b;
                Assert.IsTrue(decimalResult == fp8Result);
            }
        }

        // <=
        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                bool decimalResult = decimal_a <= decimal_b;
                bool fp8Result = fp8_a <= fp8_b;
                Assert.IsTrue(decimalResult == fp8Result);
            }
        }

        // >
        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                bool decimalResult = decimal_a > decimal_b;
                bool fp8Result = fp8_a > fp8_b;
                Assert.IsTrue(decimalResult == fp8Result);
            }
        }

        // >=
        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                bool decimalResult = decimal_a >= decimal_b;
                bool fp8Result = fp8_a >= fp8_b;
                Assert.IsTrue(decimalResult == fp8Result);
            }
        }

        // Equals
        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                bool decimalResult = decimal_a.Equals(decimal_b);
                bool fp8Result = fp8_a.Equals(fp8_b);
                Assert.IsTrue(decimalResult == fp8Result);
            }
        }
    }


    [TestMethod()]
    public void IncrementDecrementTest()
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            if (decimal_a < 92233720367m)
            {
                var fp8 = (FixedPoint8)decimal_a;
                var decimalResult = decimal_a + 1;
                fp8++;

                Assert.IsTrue((FixedPoint8)decimalResult == fp8);
            }
        }

        foreach (var decimal_a in list)
        {
            if (-92233720367m < decimal_a)
            {
                var fp8 = (FixedPoint8)decimal_a;
                var decimalResult = decimal_a - 1;
                fp8--;

                Assert.IsTrue((FixedPoint8)decimalResult == fp8);
            }
        }
    }

    [TestMethod()]
    public void PlusMinusTest()
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            if (-92233720367m < decimal_a && decimal_a < 92233720367m)
            {
                var fp8 = (FixedPoint8)decimal_a;
                {
                    var fp8Result = -fp8;
                    var decimalResult = -decimal_a;

                    Assert.IsTrue(fp8Result == (FixedPoint8)decimalResult);
                }
                {
                    var fp8Result = +fp8;
                    var decimalResult = +decimal_a;

                    Assert.IsTrue(fp8Result == (FixedPoint8)decimalResult);
                }
            }
        }
    }

#pragma warning disable IDE0059
    [TestMethod()]
    public void GetHashCodeTest()
    {
        // 実行してエラーが出ないことのみ確認
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            var hash =fp8.GetHashCode();
        }
    }
#pragma warning restore IDE0059

    [TestMethod()]
    public void CompareToTest()
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                var fp8Result = fp8_a.CompareTo(fp8_b);
                var decimalResult = decimal_a.CompareTo(decimal_b);

                Assert.IsTrue(fp8Result == decimalResult);
            }
        }
    }

    [TestMethod()]
    public void AbsTest()
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            if (-46116860184.27387903m < decimal_a && decimal_a < 46116860184.27387903m)
            {
                var fp8 = (FixedPoint8)decimal_a;
                var fp8Result = FixedPoint8.Abs(fp8);
                var decimalResult = decimal.Abs(decimal_a);

                Assert.IsTrue(fp8Result == (FixedPoint8)decimalResult);
            }
        }
    }

    [TestMethod()]
    public void IsCanonicalTest() // 値が正規表現内にあるかどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsCanonical(fp8);

            Assert.IsTrue(fp8Result);
        }
    }

    [TestMethod()]
    public void IsComplexNumberTest() // 値が複素数を表すかどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsComplexNumber(fp8);

            Assert.IsFalse(fp8Result);
        }
    }

    [TestMethod()]
    public void IsEvenIntegerTest() // 値が偶数の整数を表すかどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsEvenInteger(fp8);
            bool decimalResult = decimal.IsEvenInteger(decimal_a);

            Assert.IsTrue(fp8Result == decimalResult);
        }
    }

    [TestMethod()]
    public void IsFiniteTest() // 値が有限かどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsFinite(fp8);
            bool doubleResult = double.IsFinite((double)decimal_a);

            Assert.IsTrue(fp8Result == doubleResult);
        }
    }

    [TestMethod()]
    public void IsImaginaryNumberTest() // 値が虚数を表すかどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsImaginaryNumber(fp8);

            Assert.IsFalse(fp8Result);
        }
    }

    [TestMethod()]
    public void IsInfinityTest() // 値が無限かどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsInfinity(fp8);
            bool doubleResult = double.IsInfinity((double)decimal_a);

            Assert.IsTrue(fp8Result == doubleResult);
        }
    }

    [TestMethod()]
    public void IsIntegerTest() // 値が整数を表すかどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsInteger(fp8);
            bool decimalResult = decimal.IsInteger(decimal_a);

            Assert.IsTrue(fp8Result == decimalResult);
        }
    }

    [TestMethod()]
    public void IsNaNTest() // 値が NaN かどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsNaN(fp8);
            bool doubleResult = double.IsNaN((double)decimal_a);

            Assert.IsTrue(fp8Result == doubleResult);
        }
    }

    [TestMethod()]
    public void IsNegativeTest() // 値が負の値かどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsNegative(fp8);
            bool decimalResult = decimal.IsNegative(decimal_a);

            Assert.IsTrue(fp8Result == decimalResult);
        }
    }

    [TestMethod()]
    public void IsNegativeInfinityTest() // 値が負の無限大かどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsNegativeInfinity(fp8);
            bool doubleResult = double.IsNegativeInfinity((double)decimal_a);

            Assert.IsTrue(fp8Result == doubleResult);
        }
    }

    [TestMethod()]
    public void IsNormalTest() // 値が正常かどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsNormal(fp8);
            bool doubleResult = double.IsNormal((double)decimal_a);

            Assert.IsTrue(fp8Result == doubleResult);
        }
    }


    [TestMethod()]
    public void IsOddIntegerTest() // 値が奇数の整数を表すかどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsOddInteger(fp8);
            bool decimalResult = decimal.IsOddInteger(decimal_a);

            Assert.IsTrue(fp8Result == decimalResult);
        }
    }

    [TestMethod()]
    public void IsPositiveTest() // 値が正かどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsPositive(fp8);
            bool decimalResult = decimal.IsPositive(decimal_a);

            Assert.IsTrue(fp8Result == decimalResult);
        }
    }

    [TestMethod()]
    public void IsPositiveInfinityTest() // 値が正の無限大かどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsPositiveInfinity(fp8);
            bool doubleResult = double.IsPositiveInfinity((double)decimal_a);

            Assert.IsTrue(fp8Result == doubleResult);
        }
    }

    [TestMethod()]
    public void IsRealNumberTest() // 値が実数を表すかどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsRealNumber(fp8);
            bool doubleResult = double.IsRealNumber((double)decimal_a);

            Assert.IsTrue(fp8Result == doubleResult);
        }
    }

    [TestMethod()]
    public void IsSubnormalTest() // 値が非正規かどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsSubnormal(fp8);
            bool doubleResult = double.IsSubnormal((double)decimal_a);

            Assert.IsTrue(fp8Result == doubleResult);
        }
    }

    [TestMethod()]
    public void IsZeroTest() // 値が 0 かどうか
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;
            bool fp8Result = FixedPoint8.IsZero(fp8);

            if (fp8 == FixedPoint8.Zero)
            {
                Assert.IsTrue(fp8Result);
            }
            else
            {
                Assert.IsFalse(fp8Result);
            }
        }
    }


    [TestMethod()]
    public void MaxMagnitudeTest()
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                var fp8Result = FixedPoint8.MaxMagnitude(fp8_a, fp8_b);
                var decimalResult = decimal.MaxMagnitude(decimal_a, decimal_b);

                Assert.IsTrue(fp8Result == (FixedPoint8)decimalResult);
            }
        }
    }

    [TestMethod()]
    public void MaxMagnitudeNumberTest()
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                var fp8Result = FixedPoint8.MaxMagnitudeNumber(fp8_a, fp8_b);
                var decimalResult = decimal.MaxMagnitude(decimal_a, decimal_b);

                Assert.IsTrue(fp8Result == (FixedPoint8)decimalResult);
            }
        }
    }

    [TestMethod()]
    public void MinMagnitudeTest()
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                var fp8Result = FixedPoint8.MinMagnitude(fp8_a, fp8_b);
                var decimalResult = decimal.MinMagnitude(decimal_a, decimal_b);

                Assert.IsTrue(fp8Result == (FixedPoint8)decimalResult);
            }
        }
    }

    [TestMethod()]
    public void MinMagnitudeNumberTest()
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8_a = (FixedPoint8)decimal_a;

            foreach (var decimal_b in list)
            {
                var fp8_b = (FixedPoint8)decimal_b;

                var fp8Result = FixedPoint8.MinMagnitudeNumber(fp8_a, fp8_b);
                var decimalResult = decimal.MinMagnitude(decimal_a, decimal_b);

                Assert.IsTrue(fp8Result == (FixedPoint8)decimalResult);
            }
        }
    }


    [TestMethod()]
    public void Prase_TryParseTest()
    {
        var trueList = new List<(string fromString, decimal resultNum)>()
        {
            ("0",0m) ,
            ( "0.01",0.01m ),
            ( ".01",0.01m ),
            ("1",1m ),
            ( "1.0",1.0m ),
            ( "-0",-0m ),
            ( "-0.01",-0.01m ),
            ( "-.01",-0.01m ),
            ("-1",-1m ),
            ( "-1.0",-1.0m ),
            ("123.456",123.456m),
            ("-123.456",-123.456m),
            ("0.01234",0.01234m),
            ("-0.01234",-0.01234m),
        };

        var fp8OnlyList = new List<(string fromString, decimal resultNum)>()
        {
            ("1.2e3",1200m),
            ("1.2e+3",1200m),
            ("1.2e-3",0.0012m),
            ("1.2E3",1200m),
        };

        var falseList = new List<string>()
        {
            "--",
            "++",
            "a",
            "9a",
            "1.2.3",
            "-1.2.3",
            "1.2x",
            "5.0ea",
            "5.0e3b",
        };

        var list = GetDecimals();

        foreach (var item in list)
        {
            StringParseTest(item.ToString(), item, true, false);
        }

        foreach (var (fromString, resultNum) in trueList)
        {
            StringParseTest(fromString, resultNum, true, false);
        }

        foreach (var (fromString, resultNum) in fp8OnlyList)
        {
            StringParseTest(fromString, resultNum, true, true);
        }

        foreach (var item in falseList)
        {
            StringParseTest(item, 0m, false, false);
        }
    }

    public static void StringParseTest(string fromString, decimal resultNum, bool success, bool fp8Only)
    {
        FixedPoint8 value;
        bool result;

        if (success)
        {
            if (fp8Only == false)
            {
                // String
                result = FixedPoint8.TryParse(fromString, out value);
                Assert.IsTrue(result);
                Assert.IsTrue(value == (FixedPoint8)resultNum);

                value = FixedPoint8.Parse(fromString);
                Assert.IsTrue(value == (FixedPoint8)resultNum);

                // CharArray
                result = FixedPoint8.TryParse(fromString.ToCharArray(), out value);
                Assert.IsTrue(result);
                Assert.IsTrue(value == (FixedPoint8)resultNum);

                value = FixedPoint8.Parse(fromString.ToCharArray());
                Assert.IsTrue(value == (FixedPoint8)resultNum);
            }

            // Utf8
            byte[] fromUtf8 = Encoding.UTF8.GetBytes(fromString);

            result = FixedPoint8.TryParse(fromUtf8, out value);
            Assert.IsTrue(result);
            Assert.IsTrue(value == (FixedPoint8)resultNum);

            value = FixedPoint8.Parse(fromUtf8);
            Assert.IsTrue(value == (FixedPoint8)resultNum);
        }
        else
        {
            // String
            result = FixedPoint8.TryParse(fromString, out value);
            Assert.IsFalse(result);
            Assert.IsTrue(value == FixedPoint8.Zero);

            result = true;
            try
            {
                value = FixedPoint8.Parse(fromString);
            }
            catch
            {
                result = false;
            }
            Assert.IsFalse(result);

            // CharArray
            result = FixedPoint8.TryParse(fromString.ToCharArray(), out value);
            Assert.IsFalse(result);
            Assert.IsTrue(value == FixedPoint8.Zero);

            result = true;
            try
            {
                value = FixedPoint8.Parse(fromString.ToCharArray());
            }
            catch
            {
                result = false;
            }
            Assert.IsFalse(result);

            // Utf8
            result = FixedPoint8.TryParse(Encoding.UTF8.GetBytes(fromString), out value);
            Assert.IsFalse(result);
            Assert.IsTrue(value == FixedPoint8.Zero);

            result = true;
            try
            {
                value = FixedPoint8.Parse(Encoding.UTF8.GetBytes(fromString));
            }
            catch
            {
                result = false;
            }
            Assert.IsFalse(result);
        }
    }

    [TestMethod()]
    public void CastTest()
    {
        var list = GetDecimals();
        foreach (var decimal_a in list)
        {
            if (sbyte.MinValue <= decimal_a && decimal_a <= sbyte.MaxValue)
            {
                var fp8_a = (FixedPoint8)decimal_a;
                var decimalTo = (sbyte)decimal_a;
                var fp8To = (sbyte)fp8_a;
                Assert.IsTrue(decimalTo == fp8To);

                var toFp8 = (FixedPoint8)decimalTo;
                var toDecimal = (decimal)decimalTo;
                Assert.IsTrue(toFp8 == (FixedPoint8)toDecimal);
            }

            if (byte.MinValue <= decimal_a && decimal_a <= byte.MaxValue)
            {
                var fp8_a = (FixedPoint8)decimal_a;
                var decimalTo = (byte)decimal_a;
                var fp8To = (byte)fp8_a;
                Assert.IsTrue(decimalTo == fp8To);

                var toFp8 = (FixedPoint8)decimalTo;
                var toDecimal = (decimal)decimalTo;
                Assert.IsTrue(toFp8 == (FixedPoint8)toDecimal);
            }

            if (short.MinValue <= decimal_a && decimal_a <= short.MaxValue)
            {
                var fp8_a = (FixedPoint8)decimal_a;
                var decimalTo = (short)decimal_a;
                var fp8To = (short)fp8_a;
                Assert.IsTrue(decimalTo == fp8To);

                var toFp8 = (FixedPoint8)decimalTo;
                var toDecimal = (decimal)decimalTo;
                Assert.IsTrue(toFp8 == (FixedPoint8)toDecimal);
            }

            if (ushort.MinValue <= decimal_a && decimal_a <= ushort.MaxValue)
            {
                var fp8_a = (FixedPoint8)decimal_a;
                var decimalTo = (ushort)decimal_a;
                var fp8To = (ushort)fp8_a;
                Assert.IsTrue(decimalTo == fp8To);

                var toFp8 = (FixedPoint8)decimalTo;
                var toDecimal = (decimal)decimalTo;
                Assert.IsTrue(toFp8 == (FixedPoint8)toDecimal);
            }

            if (int.MinValue <= decimal_a && decimal_a <= int.MaxValue)
            {
                var fp8_a = (FixedPoint8)decimal_a;
                var decimalTo = (int)decimal_a;
                var fp8To = (int)fp8_a;
                Assert.IsTrue(decimalTo == fp8To);

                var toFp8 = (FixedPoint8)decimalTo;
                var toDecimal = (decimal)decimalTo;
                Assert.IsTrue(toFp8 == (FixedPoint8)toDecimal);
            }

            if (uint.MinValue <= decimal_a && decimal_a <= uint.MaxValue)
            {
                var fp8_a = (FixedPoint8)decimal_a;
                var decimalTo = (uint)decimal_a;
                var fp8To = (uint)fp8_a;
                Assert.IsTrue(decimalTo == fp8To);

                var toFp8 = (FixedPoint8)decimalTo;
                var toDecimal = (decimal)decimalTo;
                Assert.IsTrue(toFp8 == (FixedPoint8)toDecimal);
            }

            {
                var fp8_a = (FixedPoint8)decimal_a;
                var decimalTo = (long)decimal_a;
                var fp8To = (long)fp8_a;
                Assert.IsTrue(decimalTo == fp8To);

                var toFp8 = (FixedPoint8)decimalTo;
                var toDecimal = (decimal)decimalTo;
                Assert.IsTrue(toFp8 == (FixedPoint8)toDecimal);
            }

            if (0 < decimal_a)
            {
                var fp8_a = (FixedPoint8)decimal_a;
                var decimalTo = (ulong)decimal_a;
                var fp8To = (ulong)fp8_a;
                Assert.IsTrue(decimalTo == fp8To);

                var toFp8 = (FixedPoint8)decimalTo;
                var toDecimal = (decimal)decimalTo;
                Assert.IsTrue(toFp8 == (FixedPoint8)toDecimal);
            }

            if (decimal_a == 0)
            {
                var fp8_a = (FixedPoint8)decimal_a;
                var decimalTo = (float)decimal_a;
                var fp8To = (float)fp8_a;
                Assert.IsTrue(decimalTo == fp8To);

                var toFp8 = (FixedPoint8)decimalTo;
                var toDecimal = (decimal)decimalTo;
                Assert.IsTrue(toFp8 == (FixedPoint8)toDecimal);
            }
            else
            {
                if (-92233720367m < decimal_a && decimal_a < 92233720367m)
                {
                    var fp8_a = (FixedPoint8)decimal_a;
                    var decimalTo = (float)decimal_a;
                    var fp8To = (float)fp8_a;

                    double rate = fp8To / decimalTo;
                    Assert.IsTrue(0.999999 < rate && rate < 1.000001);

                    var toFp8 = (FixedPoint8)decimalTo;
                    var toDecimal = (decimal)decimalTo;
                    double rate2 = (double)toFp8 / (double)toDecimal;
                    Assert.IsTrue(0.999999 < rate2 && rate2 < 1.000001);
                }

            }

            if (decimal_a == 0)
            {
                var fp8_a = (FixedPoint8)decimal_a;
                var decimalTo = (double)decimal_a;
                var fp8To = (double)fp8_a;
                Assert.IsTrue(decimalTo == fp8To);

                var toFp8 = (FixedPoint8)decimalTo;
                var toDecimal = (decimal)decimalTo;
                Assert.IsTrue(toFp8 == (FixedPoint8)toDecimal);
            }
            else
            {
                if (-92233720367m < decimal_a && decimal_a < 92233720367m)
                {
                    var fp8_a = (FixedPoint8)decimal_a;
                    var decimalTo = (double)decimal_a;
                    var fp8To = (double)fp8_a;

                    double rate = fp8To / decimalTo;
                    Assert.IsTrue(0.99999999 < rate && rate < 1.00000001);

                    var toFp8 = (FixedPoint8)decimalTo;
                    var toDecimal = (decimal)decimalTo;
                    double rate2 = (double)toFp8 / (double)toDecimal;
                    Assert.IsTrue(0.99999999 < rate2 && rate2 < 1.00000001);
                }
            }

            {
                var fp8_a = (FixedPoint8)decimal_a;
                var decimalTo = (decimal)decimal_a;
                var fp8To = (decimal)fp8_a;
                Assert.IsTrue(decimalTo == fp8To);

                var toFp8 = (FixedPoint8)decimalTo;
                var toDecimal = (decimal)decimalTo;
                Assert.IsTrue(toFp8 == (FixedPoint8)toDecimal);
            }
        }
    }

    [TestMethod()]
    public void MathTest()
    {
        var list = GetDecimals();

        foreach (var decimal_a in list)
        {
            var fp8 = (FixedPoint8)decimal_a;

            if (-92233720368m <= decimal_a && decimal_a < 92233720368m)
            {
                //Round
                var mathRound = Math.Round(decimal_a);
                var fp8Round = fp8.Round();
                Assert.IsTrue((decimal)fp8Round == mathRound);

                for (int decimals = -10; decimals < 7; decimals++)
                {
                    decimal decimal_b = decimal_a;
                    decimal mathRound2;
                    if (decimals < 0)
                    {
                        for (int i = 0; i < -decimals; i++)
                        {
                            decimal_b /= 10;
                        }
                        mathRound2 = Math.Round(decimal_b);

                        for (int i = 0; i < -decimals; i++)
                        {
                            mathRound2 *= 10;
                        }
                    }
                    else
                    {
                        mathRound2 = Math.Round(decimal_a, decimals);
                    }
                    var fp8Round2 = fp8.Round(decimals);
                    Assert.IsTrue((decimal)fp8Round2 == mathRound2);
                }

                // Floor
                var mathFloor = Math.Floor(decimal_a);
                var fp8Floor = fp8.Floor();
                Assert.IsTrue((decimal)fp8Floor == mathFloor);

                for (int decimals = -10; decimals < 7; decimals++)
                {
                    decimal decimal_b = decimal_a;
                    decimal mathFloor2;
                    if (decimals < 0)
                    {
                        for (int i = 0; i < -decimals; i++)
                        {
                            decimal_b /= 10;
                        }
                        mathFloor2 = Math.Floor(decimal_b);

                        for (int i = 0; i < -decimals; i++)
                        {
                            mathFloor2 *= 10;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < decimals; i++)
                        {
                            decimal_b *= 10;
                        }
                        mathFloor2 = Math.Floor(decimal_b);

                        for (int i = 0; i < decimals; i++)
                        {
                            mathFloor2 /= 10;
                        }
                    }
                    var fp8Floor2 = fp8.Floor(decimals);
                    Assert.IsTrue((decimal)fp8Floor2 == mathFloor2);
                }

                // Truncate
                var mathTruncate = Math.Truncate(decimal_a);
                var fp8Truncate = fp8.Truncate();
                Assert.IsTrue((decimal)fp8Truncate == mathTruncate);

                for (int decimals = -10; decimals < 7; decimals++)
                {
                    decimal decimal_b = decimal_a;
                    decimal mathTruncate2;
                    if (decimals < 0)
                    {
                        for (int i = 0; i < -decimals; i++)
                        {
                            decimal_b /= 10;
                        }
                        mathTruncate2 = Math.Truncate(decimal_b);

                        for (int i = 0; i < -decimals; i++)
                        {
                            mathTruncate2 *= 10;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < decimals; i++)
                        {
                            decimal_b *= 10;
                        }
                        mathTruncate2 = Math.Truncate(decimal_b);

                        for (int i = 0; i < decimals; i++)
                        {
                            mathTruncate2 /= 10;
                        }
                    }
                    var fp8Truncate2 = fp8.Truncate(decimals);
                    Assert.IsTrue((decimal)fp8Truncate2 == mathTruncate2);
                }

                // Ceiling
                var mathCeiling = Math.Ceiling(decimal_a);
                var fp8Ceiling = fp8.Ceiling();
                Assert.IsTrue((decimal)fp8Ceiling == mathCeiling);

                for (int decimals = -10; decimals < 7; decimals++)
                {
                    decimal decimal_b = decimal_a;
                    decimal mathCeiling2;
                    if (decimals < 0)
                    {
                        for (int i = 0; i < -decimals; i++)
                        {
                            decimal_b /= 10;
                        }
                        mathCeiling2 = Math.Ceiling(decimal_b);

                        for (int i = 0; i < -decimals; i++)
                        {
                            mathCeiling2 *= 10;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < decimals; i++)
                        {
                            decimal_b *= 10;
                        }
                        mathCeiling2 = Math.Ceiling(decimal_b);

                        for (int i = 0; i < decimals; i++)
                        {
                            mathCeiling2 /= 10;
                        }
                    }
                    var fp8Ceiling2 = fp8.Ceiling(decimals);
                    Assert.IsTrue((decimal)fp8Ceiling2 == mathCeiling2);
                }
            }
        }
    }

    [TestMethod()]
    public void MinValueMathTest()
    {
        decimal value = -92233720368.54775808m;
        FixedPoint8 fp8Value = (FixedPoint8)value;

        // Round
        Assert.IsTrue(fp8Value.Round().ToString() == "92233720368"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Round(-10).ToString() == "-90000000000"); // 実行可能
        Assert.IsTrue(fp8Value.Round(-9).ToString() == "-92000000000"); // 実行可能
        Assert.IsTrue(fp8Value.Round(-8).ToString() == "-92200000000"); // 実行可能
        Assert.IsTrue(fp8Value.Round(-7).ToString() == "-92230000000"); // 実行可能
        Assert.IsTrue(fp8Value.Round(-6).ToString() == "92233440737.09551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Round(-5).ToString() == "-92233700000"); // 実行可能
        Assert.IsTrue(fp8Value.Round(-4).ToString() == "-92233720000"); // 実行可能
        Assert.IsTrue(fp8Value.Round(-3).ToString() == "-92233720000"); // 実行可能
        Assert.IsTrue(fp8Value.Round(-2).ToString() == "92233720337.09551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Round(-1).ToString() == "92233720367.09551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Round(1).ToString() == "-92233720368.5"); // 実行可能
        Assert.IsTrue(fp8Value.Round(2).ToString() == "92233720368.54551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Round(3).ToString() == "92233720368.54751616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Round(4).ToString() == "92233720368.54771616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Round(5).ToString() == "92233720368.54775616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Round(6).ToString() == "-92233720368.547758"); // 実行可能
        Assert.IsTrue(fp8Value.Round(7).ToString() == "92233720368.54775806"); // オーバーフローが発生する。Exceptionが発生しないこと

        //Floor
        Assert.IsTrue(fp8Value.Floor().ToString() == "92233720367");　// オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(-10).ToString() == "84467440737.09551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(-9).ToString() == "91467440737.09551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(-8).ToString() == "92167440737.09551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(-7).ToString() == "92227440737.09551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(-6).ToString() == "92233440737.09551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(-5).ToString() == "92233640737.09551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(-4).ToString() == "92233710737.09551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(-3).ToString() == "92233719737.09551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(-2).ToString() == "92233720337.09551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(-1).ToString() == "92233720367.09551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(1).ToString() == "92233720368.49551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(2).ToString() == "92233720368.54551616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(3).ToString() == "92233720368.54751616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(4).ToString() == "92233720368.54771616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(5).ToString() == "92233720368.54775616"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(6).ToString() == "92233720368.54775716"); // オーバーフローが発生する。Exceptionが発生しないこと
        Assert.IsTrue(fp8Value.Floor(7).ToString() == "92233720368.54775806"); // オーバーフローが発生する。Exceptionが発生しないこと

        //Truncate
        Assert.IsTrue(fp8Value.Truncate().ToString() == "-92233720368"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(-10).ToString() == "-90000000000"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(-9).ToString() == "-92000000000"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(-8).ToString() == "-92200000000"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(-7).ToString() == "-92230000000"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(-6).ToString() == "-92233000000"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(-5).ToString() == "-92233700000"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(-4).ToString() == "-92233720000"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(-3).ToString() == "-92233720000"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(-2).ToString() == "-92233720300"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(-1).ToString() == "-92233720360"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(1).ToString() == "-92233720368.5"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(2).ToString() == "-92233720368.54"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(3).ToString() == "-92233720368.547"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(4).ToString() == "-92233720368.5477"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(5).ToString() == "-92233720368.54775"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(6).ToString() == "-92233720368.547758"); // 実行可能
        Assert.IsTrue(fp8Value.Truncate(7).ToString() == "-92233720368.547758"); // 実行可能

        //Ceiling
        Assert.IsTrue(fp8Value.Ceiling().ToString() == "-92233720368"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(-10).ToString() == "-90000000000"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(-9).ToString() == "-92000000000"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(-8).ToString() == "-92200000000"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(-7).ToString() == "-92230000000"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(-6).ToString() == "-92233000000"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(-5).ToString() == "-92233700000"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(-4).ToString() == "-92233720000"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(-3).ToString() == "-92233720000"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(-2).ToString() == "-92233720300"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(-1).ToString() == "-92233720360"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(1).ToString() == "-92233720368.5"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(2).ToString() == "-92233720368.54"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(3).ToString() == "-92233720368.547"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(4).ToString() == "-92233720368.5477"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(5).ToString() == "-92233720368.54775"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(6).ToString() == "-92233720368.547758"); // 実行可能
        Assert.IsTrue(fp8Value.Ceiling(7).ToString() == "-92233720368.547758"); // 実行可能
    }


    [TestMethod()]
    public void WriteCharsTest()
    {
        var list = GetDecimals();

        foreach (var item in list)
        {
            var fp8 = (FixedPoint8)item;

            var fp8St = $"{fp8}";
            var decSt = $"{item}";

            // 小数点以下の右側の0を消す
            // "1.0200" → "1.02"
            if (decSt.Contains('.'))
            {
                while (decSt[^1..] == "0")
                {
                    decSt = decSt[..^1];
                }
                if (decSt[^1..] == ".")
                {
                    decSt = decSt[..^1];
                }
            }

            Assert.IsTrue(fp8St == decSt);
        }
    }

    [TestMethod()]
    public void WriteUtf8Test()
    {
        Span<byte> buffer = stackalloc byte[21];

        var list = GetDecimals();

        foreach (var item in list)
        {
            var fp8 = (FixedPoint8)item;

            int offset = 0;

            var result = fp8.TryWriteUtf8(buffer, out int charsWritten);
            Assert.IsTrue(result);

            var value = fp8.WriteUtf8(ref buffer);
            offset += value;

            var decimalString = item.ToString();

            // 小数点以下の右側の0を消す
            // "1.0200" → "1.02"
            if (decimalString.Contains('.'))
            {
                while (decimalString[^1..] == "0")
                {
                    decimalString = decimalString[..^1];
                }
                if (decimalString[^1..] == ".")
                {
                    decimalString = decimalString[..^1];
                }
            }

            var decimalUtf8 = Encoding.UTF8.GetBytes(decimalString);

            Assert.IsTrue(buffer[..offset].SequenceEqual(decimalUtf8));
        }
    }       

    [TestMethod()]
    public void ToStringTest()
    {
        var list = GetDecimals();

        foreach (var item in list)
        {
            var fp8 = (FixedPoint8)item;
            var result = fp8.ToString();

            var decimalString = item.ToString();

            // 小数点以下の右側の0を消す
            // "1.0200" → "1.02"
            if (decimalString.Contains('.'))
            {
                while (decimalString[^1..] == "0")
                {
                    decimalString = decimalString[..^1];
                }
                if (decimalString[^1..] == ".")
                {
                    decimalString = decimalString[..^1];
                }
            }
            Assert.IsTrue(result == decimalString);
        }
    }

    [TestMethod]
    public void WriteCharsIBufferWriterTest()
    {
        var writer = new System.Buffers.ArrayBufferWriter<char>();

        var list = GetDecimals();

        foreach (var item in list)
        {
            var fp8 = (FixedPoint8)item;

            fp8.WriteChars(writer);
            writer.Write(" ".AsSpan());
        }
        System.Diagnostics.Debug.WriteLine(new string(writer.WrittenSpan));
    }

    [TestMethod]
    public void WriteUtf8IBufferWriterTest()
    {
        var writer = new System.Buffers.ArrayBufferWriter<byte>();

        var list = GetDecimals();

        foreach (var item in list)
        {
            var fp8 = (FixedPoint8)item;

            fp8.WriteUtf8(writer);
            writer.Write(" "u8);
        }
        System.Diagnostics.Debug.WriteLine(Encoding.UTF8.GetString(writer.WrittenSpan));
    }
}
