■ **Gitan.FixedPoint8とは**

Gitan.FixedPoint8は、固定小数点で-92233720368.54775808～92233720368.54775807までの数字を扱うことができます。
内部にInt64をもつstructで、10進数の小数点を誤差なく扱うことができます。
実行速度が速いことに重点を置いてUTF8との親和性が高いです。

プロジェクトURL : [https://github.com/gitan-dev/FixedPoint8](https://github.com/gitan-dev/FixedPoint8)

■ **仕様**

・ Gitan.FixedPoint8はuncheckedで動きます、オーバーフローが発生する値でエラーは発生しませんのでご注意ください。

・ 機能のいくつかはdecimalを使用した実装になっているため、十分な速度がでません。(速度最適化未実施と記載のある機能が該当します)

・ ver1.0.5の変更にてWriteUtf8,WriteCharsの破壊的変更があります。


■ **使用方法**

NuGetパッケージ : Gitan.FixedPoint8

NuGetを使用してGitan.FixedPoint8パッケージをインストールします。

FixedPoint8を使用する方法を以下に記載します。

    using Gitan.FixedPoint8;

    public void fp8Test()       
    {
        var v1 = (FixedPoint8)12.34;        
        var v2 = (FixedPoint8)23.45;
        //v1は内部的にInt64の1234000000となる。

        var add = v1 + v2;
        var sub = v1 - v2;
        var mul = v1 * v2;
        var div = v1 / v2;
    }


NuGetパッケージ : Gitan.Utf8Json_FixedPoint8

NuGetを使用してGitan.Utf8Json_FixedPoint8をインストールします。　※依存関係で[Utf8Json](https://github.com/neuecc/Utf8Json)がインストールされます。

Gitan.Utf8Json_FixedPoint8はJsonとFixedPoint8をRead,Writeします。

ReadFixedPoint8,WriteFixedPoint8の処理は[Utf8Json](https://github.com/neuecc/Utf8Json/tree/master/src/Utf8Json)の[NumberConverter](https://github.com/neuecc/Utf8Json/blob/master/src/Utf8Json/Internal/NumberConverter.cs)を部分引用しています。

Utf8Jsonを使用してFixedPoint8シリアライズ/デシリアライズする方法を以下に記載します。

    using Gitan.FixedPoint8;

    static readonly byte[] _json = """{"Value":-12.34}"""u8.ToArray();

    public FixedPoint8 DeserializeFixedPoint8()
    {
        var obj = JsonSerializer.Deserialize<FixedPoint8Class>(_json);
        return obj.Value;
    }

    public byte[] SerializeFixedPoint8()
    {
        var test = FixedPoint8Class.GetSample();
        var result = JsonSerializer.Serialize<FixedPoint8Class>(test);
        return result;
    }

    public class FixedPoint8Class
    {
        [JsonFormatter(typeof(FixedPoint8Formatter))]
        public FixedPoint8 Value { get; set; }
        public static FixedPoint8Class GetSample()
        {
            var item = new FixedPoint8Class()
            {
                Value = new FixedPoint8(-1_234_000_000),
            };
            return item;
        }
    }

Reader.Writerで読み書きする方法を以下に記載します。

    using Gitan.FixedPoint8;

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
            var reader = new Utf8Json.JsonReader(json.ToArray());
            var result = reader.ReadFixedPoint8();
            Assert.IsTrue(result.ToString() == resultString);

            var writer = new Utf8Json.JsonWriter(sharedBuffer);
            writer.WriteFixedPoint8(result);

            var result2 = Encoding.UTF8.GetString(writer.GetBuffer().ToArray());
            Assert.IsTrue(result2 == resultString);
        }

        readonly byte[] sharedBuffer = new byte[65535];

■ **パフォーマンス**
   
**FixedPoint8**

    static readonly FixedPoint8 fixedPoint8Value = FixedPoint8.FromInnerValue(-12_3400_0000);
    static readonly FixedPoint8 v2 = FixedPoint8.FromInnerValue(2_0000_0000);
    static readonly FixedPoint8 v10 = FixedPoint8.FromInnerValue(10_0000_0000);
    static readonly int intValue = -1234;
    static readonly double doubleValue = -12.34;
    static readonly decimal decimalValue = -12.34m;

    static readonly int[] _intValues = new int[]
    {
        1000000000,
        123456789,
        12345678,
        1234567,
        123456,
        1234,
        12,
        1,
        0,
        -1000000000,
        -123456789,
        -12345678,
        -1234567,
        -123456,
        -1234,
        -12,
        -1,
    };

    static readonly decimal[] _decimalValues = new decimal[]
    {
        12345678m,
        1234m,
        12m,
        1m,
        0m,
        0.1m,
        0.12m,
        0.1234m,
        0.12345678m,
        -12345678m,
        -1234m,
        -12m,
        -1m,
        -0.1m,
        -0.12m,
        -0.1234m,
        -0.12345678m,
    };

    static readonly double[] _doubleValues = _decimalValues.Select(x => (double)x).ToArray();
    static readonly FixedPoint8[] _fp8Values = _decimalValues.Select(x => (FixedPoint8)x).ToArray();

    [Benchmark]
    public FixedPoint8 MulInt2FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach(var value in _fp8Values)
            {
                sum += value * 2;
            }
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 Mul2FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _fp8Values)
            {
                sum += value * v2;
            }
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 MulInt10FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _fp8Values)
            {
                sum += value * 10;
            }
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 Mul10FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _fp8Values)
            {
                sum += value * v10;
            }
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 Add10FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _fp8Values)
            {
                sum += value + v10;
            }
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 Sub10FixedPoint8()
    {
        var sum = FixedPoint8.Zero;
        for (var i = 0; i < 1000; i++)
        {
            foreach (var value in _fp8Values)
            {
                sum += value - v10;
            }
        }
        return sum;
    }


加算、減算はdecimalと比べて90%～95%速いがDoubleの約1/2の速度です。
FixedPoint8との乗算は遅いので使用を推奨しない


| Method               | Runtime  | Mean            | Error          | StdDev         | Median          | Ratio | RatioSD |
|--------------------- |--------- |----------------:|---------------:|---------------:|----------------:|------:|--------:|
| Mul2Int              | .NET 7.0 |   4,572.7710 ns |     90.3135 ns |    167.4018 ns |   4,547.3579 ns |  1.00 |    0.00 |
| Mul2Int              | .NET 8.0 |   4,293.8047 ns |     85.4494 ns |     98.4037 ns |   4,321.8761 ns |  0.94 |    0.05 |
| Mul2Double           | .NET 7.0 |  11,539.7450 ns |     83.0773 ns |     73.6459 ns |  11,530.6412 ns |  1.00 |    0.00 |
| Mul2Double           | .NET 8.0 |  11,670.8632 ns |    208.7207 ns |    185.0254 ns |  11,607.7080 ns |  1.01 |    0.02 |
| Mul2Decimal          | .NET 7.0 | 173,784.9714 ns |  1,794.6919 ns |  1,590.9477 ns | 173,015.0757 ns |  1.00 |    0.00 |
| Mul2Decimal          | .NET 8.0 | 174,499.1804 ns |  2,715.2168 ns |  2,406.9690 ns | 174,061.7065 ns |  1.00 |    0.02 |
| MulInt2FixedPoint8   | .NET 7.0 |   5,914.9793 ns |    378.1387 ns |  1,114.9504 ns |   5,873.1972 ns |  1.00 |    0.00 |
| MulInt2FixedPoint8   | .NET 8.0 |   6,387.5701 ns |    318.3312 ns |    938.6067 ns |   6,996.3493 ns |  1.12 |    0.26 |
| Mul2FixedPoint8      | .NET 7.0 | 747,730.3516 ns |  7,881.4972 ns |  7,372.3574 ns | 746,037.6953 ns |  1.00 |    0.00 |
| Mul2FixedPoint8      | .NET 8.0 | 826,037.5698 ns |  8,583.3541 ns |  7,608.9198 ns | 827,765.5273 ns |  1.11 |    0.01 |
| Mul10Int             | .NET 7.0 |   4,246.9570 ns |     81.2063 ns |     99.7287 ns |   4,233.6533 ns |  1.00 |    0.00 |
| Mul10Int             | .NET 8.0 |   4,161.1268 ns |     71.3323 ns |     63.2342 ns |   4,158.4396 ns |  0.98 |    0.03 |
| Mul10Double          | .NET 7.0 |  11,548.6852 ns |     85.2441 ns |     75.5667 ns |  11,531.9984 ns |  1.00 |    0.00 |
| Mul10Double          | .NET 8.0 |  11,569.2242 ns |     55.8982 ns |     49.5523 ns |  11,564.2693 ns |  1.00 |    0.01 |
| Mul10Decimal         | .NET 7.0 | 176,852.5651 ns |  3,412.0240 ns |  3,191.6094 ns | 176,322.6074 ns |  1.00 |    0.00 |
| Mul10Decimal         | .NET 8.0 | 179,393.4745 ns |  3,239.9466 ns |  3,856.9260 ns | 178,678.8818 ns |  1.02 |    0.03 |
| MulInt10FixedPoint8  | .NET 7.0 |   6,617.2033 ns |    131.7600 ns |    166.6342 ns |   6,628.3234 ns |  1.00 |    0.00 |
| MulInt10FixedPoint8  | .NET 8.0 |   6,653.7935 ns |    132.8328 ns |    206.8046 ns |   6,731.1947 ns |  1.01 |    0.04 |
| Mul10FixedPoint8     | .NET 7.0 | 741,841.2956 ns |  7,668.2957 ns |  7,172.9285 ns | 741,695.3125 ns |  1.00 |    0.00 |
| Mul10FixedPoint8     | .NET 8.0 | 820,721.6471 ns | 12,885.8423 ns | 12,053.4248 ns | 822,087.6953 ns |  1.11 |    0.02 |
| Add2Int              | .NET 7.0 |   5,654.9791 ns |    304.2097 ns |    896.9693 ns |   5,794.1940 ns |  1.00 |    0.00 |
| Add2Int              | .NET 8.0 |   5,847.5261 ns |    275.2209 ns |    811.4950 ns |   6,399.6235 ns |  1.06 |    0.24 |
| Add2Double           | .NET 7.0 |  11,531.5492 ns |     65.1783 ns |     54.4268 ns |  11,533.6807 ns |  1.00 |    0.00 |
| Add2Double           | .NET 8.0 |  11,577.4798 ns |    107.3107 ns |    100.3785 ns |  11,564.0594 ns |  1.01 |    0.01 |
| Add2Decimal          | .NET 7.0 | 228,013.4294 ns |  3,250.2661 ns |  3,040.3009 ns | 226,790.5029 ns |  1.00 |    0.00 |
| Add2Decimal          | .NET 8.0 | 229,288.5045 ns |  2,639.3008 ns |  2,339.6714 ns | 228,635.6445 ns |  1.00 |    0.02 |
| Add2FixedPoint8      | .NET 7.0 |   8,069.9836 ns |    121.3589 ns |    134.8901 ns |   8,040.7883 ns |  1.00 |    0.00 |
| Add2FixedPoint8      | .NET 8.0 |   6,243.1613 ns |    352.3323 ns |  1,038.8596 ns |   7,010.6266 ns |  0.80 |    0.09 |
| Add10Int             | .NET 7.0 |   5,613.1340 ns |    296.8058 ns |    875.1386 ns |   5,715.4602 ns |  1.00 |    0.00 |
| Add10Int             | .NET 8.0 |   5,869.4012 ns |    269.3940 ns |    794.3142 ns |   6,390.8222 ns |  1.07 |    0.23 |
| Add10Double          | .NET 7.0 |  11,514.3157 ns |     49.0943 ns |     38.3296 ns |  11,526.5663 ns |  1.00 |    0.00 |
| Add10Double          | .NET 8.0 |  11,532.3924 ns |     74.6589 ns |     69.8360 ns |  11,525.8194 ns |  1.00 |    0.01 |
| Add10Decimal         | .NET 7.0 | 229,111.6804 ns |  2,872.6561 ns |  2,546.5348 ns | 228,920.4956 ns |  1.00 |    0.00 |
| Add10Decimal         | .NET 8.0 | 231,055.1585 ns |  4,489.8852 ns |  4,610.7815 ns | 228,706.2988 ns |  1.01 |    0.02 |
| Add10FixedPoint8     | .NET 7.0 |   8,078.0655 ns |    132.7905 ns |    124.2123 ns |   8,017.0425 ns |  1.00 |    0.00 |
| Add10FixedPoint8     | .NET 8.0 |   6,213.6527 ns |    359.4805 ns |  1,059.9362 ns |   6,766.8495 ns |  0.78 |    0.12 |
| Sub2Int              | .NET 7.0 |   5,853.7722 ns |    302.1611 ns |    890.9289 ns |   6,233.7879 ns |  1.00 |    0.00 |
| Sub2Int              | .NET 8.0 |   5,701.3345 ns |    288.3404 ns |    850.1781 ns |   6,156.5395 ns |  1.00 |    0.23 |
| Sub2Double           | .NET 7.0 |  11,532.9719 ns |     82.2149 ns |     76.9039 ns |  11,506.2347 ns |  1.00 |    0.00 |
| Sub2Double           | .NET 8.0 |  11,664.9715 ns |    172.6358 ns |    161.4836 ns |  11,589.2929 ns |  1.01 |    0.01 |
| Sub2Decimal          | .NET 7.0 | 232,999.6338 ns |  1,680.1366 ns |  1,571.6008 ns | 233,268.7012 ns |  1.00 |    0.00 |
| Sub2Decimal          | .NET 8.0 | 229,782.5858 ns |  1,778.4805 ns |  1,576.5767 ns | 229,143.6890 ns |  0.99 |    0.01 |
| Sub2FixedPoint8      | .NET 7.0 |   8,114.4645 ns |    154.9499 ns |    144.9402 ns |   8,119.3253 ns |  1.00 |    0.00 |
| Sub2FixedPoint8      | .NET 8.0 |   5,958.9486 ns |    308.3495 ns |    909.1755 ns |   5,993.1816 ns |  0.71 |    0.13 |
| Sub10Int             | .NET 7.0 |   5,750.3535 ns |    293.1852 ns |    864.4632 ns |   6,066.8659 ns |  1.00 |    0.00 |
| Sub10Int             | .NET 8.0 |   5,845.1612 ns |    258.9527 ns |    763.5280 ns |   6,373.4074 ns |  1.04 |    0.23 |
| Sub10Double          | .NET 7.0 |  11,493.1770 ns |     47.7358 ns |     39.8616 ns |  11,492.4103 ns |  1.00 |    0.00 |
| Sub10Double          | .NET 8.0 |  11,622.6581 ns |    141.6178 ns |    132.4694 ns |  11,631.8588 ns |  1.01 |    0.01 |
| Sub10Decimal         | .NET 7.0 | 228,927.8621 ns |  2,460.0853 ns |  2,301.1653 ns | 227,765.6372 ns |  1.00 |    0.00 |
| Sub10Decimal         | .NET 8.0 | 227,867.2433 ns |  1,990.4841 ns |  1,764.5123 ns | 226,994.4458 ns |  1.00 |    0.01 |
| Sub10FixedPoint8     | .NET 7.0 |   7,999.6249 ns |     38.1571 ns |     31.8629 ns |   8,004.5624 ns |  1.00 |    0.00 |
| Sub10FixedPoint8     | .NET 8.0 |   6,044.5880 ns |    331.1392 ns |    976.3713 ns |   6,734.5013 ns |  0.70 |    0.14 |


**Utf8JsonFixedPoint8**


    /////////////////////////////////////// Reader
    ///
    static readonly byte[][] _sourceInt = new string[]
    {
        "1000000000,",
        "123456789,",
        "12345678,",
        "1234567,",
        "123456,",
        "1234,",
        "12,",
        "1,",
        "0,",
        "-1000000000,",
        "-123456789,",
        "-12345678,",
        "-1234567,",
        "-123456,",
        "-1234,",
        "-12,",
        "-1,",
    }.Select(x=>Encoding.UTF8.GetBytes(x)).ToArray();

    static readonly byte[][] _sourceDouble = new string[]
    {
        "12345678,",
        "1234,",
        "12,",
        "1,",
        "0,",
        "0.1,",
        "0.12,",
        "0.1234,",
        "0.12345678,",
        "-12345678,",
        "-1234,",
        "-12,",
        "-1,",
        "-0.1,",
        "-0.12,",
        "-0.1234,",
        "-0.12345678,",
    }.Select(x => Encoding.UTF8.GetBytes(x)).ToArray();

    static readonly int[] _intValues = new int[]
    {
        1000000000,
        123456789,
        12345678,
        1234567,
        123456,
        1234,
        12,
        1,
        0,
        -1000000000,
        -123456789,
        -12345678,
        -1234567,
        -123456,
        -1234,
        -12,
        -1,
    };

    static readonly decimal[] _decimalValues = new decimal[]
    {
        12345678m,
        1234m,
        12m,
        1m,
        0m,
        0.1m,
        0.12m,
        0.1234m,
        0.12345678m,
        -12345678m,
        -1234m,
        -12m,
        -1m,
        -0.1m,
        -0.12m,
        -0.1234m,
        -0.12345678m,
    };

    static readonly double[] _doubleValues = _decimalValues.Select(x => (double)x).ToArray();
    static readonly FixedPoint8[] _fp8Values = _decimalValues.Select(x => (FixedPoint8)x).ToArray();


    [Benchmark]
    public int ReadInt()
    {
        int sum = 0;
        foreach (var item in _sourceInt) 
        {
            JsonReader reader = new(item);
            sum += reader.ReadInt32();
        }
        return sum;
    }

    [Benchmark]
    public long ReadLong()
    {
        long sum = 0;
        foreach (var item in _sourceInt)
        {
            JsonReader reader = new(item);
            sum += reader.ReadInt64();
        }
        return sum;
    }

    [Benchmark]
    public double ReadDouble()
    {
        double sum = 0.0;
        foreach (var item in _sourceDouble)
        {
            JsonReader reader = new(item);
            sum += reader.ReadDouble();
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 ReadFixedPoint8()
    {
        FixedPoint8 sum = FixedPoint8.Zero;
        foreach (var item in _sourceDouble)
        {
            JsonReader reader = new(item);
            sum += reader.ReadFixedPoint8();
        }
        return sum;
    }

    /////////////////////////////////////// Deserialize
    ///

    static readonly byte[][] _intJsonList = GetIntJsonList();
    static byte[][] GetIntJsonList()
    {
        var list = new List<byte[]>();
        foreach(var value in _intValues)
        {
            list.Add(Encoding.UTF8.GetBytes($$"""{"Value":{{value}}}"""));
        }
        return list.ToArray();
    }  
    
    static readonly byte[][] _decimalJsonList = GetDecimalJsonList();
    static byte[][] GetDecimalJsonList()
    {
        var list = new List<byte[]>();
        foreach(var value in _decimalValues)
        {
            list.Add(Encoding.UTF8.GetBytes($$"""{"Value":{{value}}}"""));
        }
        return list.ToArray();
    }
    

    [Benchmark]
    public int DeserializeInt()
    {
        int sum = 0;
        foreach(var json in _intJsonList)
        {
            var obj = JsonSerializer.Deserialize<IntClass>(json);
            sum += obj.Value;
        }
        return sum;
    }

    [Benchmark]
    public long DeserializeLong()
    {
        long sum = 0;
        foreach (var json in _intJsonList)
        {
            var obj = JsonSerializer.Deserialize<LongClass>(json);
            sum += obj.Value;
        }
        return sum;
    }

    [Benchmark]
    public double DeserializeDouble()
    {
        double sum = 0.0;
        foreach (var json in _decimalJsonList)
        {
            var obj = JsonSerializer.Deserialize<DoubleClass>(json);
            sum += obj.Value;
        }
        return sum;
    }

    [Benchmark]
    public decimal DeserializeDecimal()
    {
        decimal sum = 0m;
        foreach (var json in _decimalJsonList)
        {
            var obj = JsonSerializer.Deserialize<DecimalClass>(json);
            sum += obj.Value;
        }
        return sum;
    }

    [Benchmark]
    public FixedPoint8 DeserializeFixedPoint8()
    {
        FixedPoint8 sum = FixedPoint8.Zero;
        foreach (var json in _decimalJsonList)
        {
            var obj = JsonSerializer.Deserialize<FixedPoint8Class>(json);
            sum += obj.Value;
        }
        return sum;
    }

    /////////////////////////////////////// Writer

    readonly byte[] sharedBuffer = new byte[65535];

    [Benchmark]
    public int WriteInt()
    {
        var writer = new JsonWriter(sharedBuffer);
        foreach (var value in _intValues)
        {
            writer.WriteInt32(value);
        }
        return writer.CurrentOffset;
    }

    [Benchmark]
    public long WriteLong()
    {
        var writer = new JsonWriter(sharedBuffer);
        foreach (var value in _intValues)
        {
            writer.WriteInt64(value);
        }
        return writer.CurrentOffset;
    }

    [Benchmark]
    public int WriteDouble()
    {
        var writer = new JsonWriter(sharedBuffer);
        foreach (var value in _doubleValues)
        {
            writer.WriteDouble(value);
        }
        return writer.CurrentOffset;
    }

    [Benchmark]
    public int WriteFixedPoint8()
    {
        var writer = new JsonWriter(sharedBuffer);
        foreach (var value in _fp8Values)
        {
            writer.WriteFixedPoint8(value);
        }
        return writer.CurrentOffset;
    }

    /////////////////////////////////////// Serialize

     [Benchmark]
    public int SerializeInt()
    {
        var test = IntClass.GetSample();
        int sum = 0;
        foreach (var value in _intValues)
        {
            test.Value = value;
            var result = JsonSerializer.Serialize<IntClass>(test);
            sum += result.Length;
        }
        return sum;
    }

    [Benchmark]
    public int SerializeLong()
    {
        var test = LongClass.GetSample();
        int sum = 0;
        foreach (var value in _intValues)
        {
            test.Value = value;
            var result = JsonSerializer.Serialize<LongClass>(test);
            sum += result.Length;
        }
        return sum;
    }

    [Benchmark]
    public int SerializeDouble()
    {
        var test = DoubleClass.GetSample();
        int sum = 0;
        foreach (var value in _doubleValues)
        {
            test.Value = value;
            var result = JsonSerializer.Serialize<DoubleClass>(test);
            sum += result.Length;
        }
        return sum;
    }

    [Benchmark]
    public int SerializeDecimal()
    {
        var test = DecimalClass.GetSample();
        int sum = 0;
        foreach (var value in _decimalValues)
        {
            test.Value = value;
            var result = JsonSerializer.Serialize<DecimalClass>(test);
            sum += result.Length;
        }
        return sum;
    }

    [Benchmark]
    public int SerializeFixedPoint8()
    {
        var test = FixedPoint8Class.GetSample();
        int sum = 0;
        foreach (var value in _fp8Values)
        {
            test.Value = value;
            var result = JsonSerializer.Serialize<FixedPoint8Class>(test);
            sum += result.Length;
        }
        return sum;
    }

    public class IntClass
    {
        public int Value { get; set; }
        public static IntClass GetSample()
        {
            var item = new IntClass()
            {
                Value = -1234,
            };
            return item;
        }
    }  
    
    public class LongClass
    {
        public long Value { get; set; }
        public static LongClass GetSample()
        {
            var item = new LongClass()
            {
                Value = -1234,
            };
            return item;
        }
    }

    public class DoubleClass
    {
        public double Value { get; set; }
        public static DoubleClass GetSample()
        {
            var item = new DoubleClass()
            {
                Value = -12.34,
            };
            return item;
        }
    }

    public class DecimalClass
    {
        public decimal Value { get; set; }
        public static DecimalClass GetSample()
        {
            var item = new DecimalClass()
            {
                Value = -12.34m,
            };
            return item;
        }
    }

    public class FixedPoint8Class
    {
        [JsonFormatter(typeof(FixedPoint8Formatter))]
        public FixedPoint8 Value { get; set; }
        public static FixedPoint8Class GetSample()
        {
            var item = new FixedPoint8Class()
            {
                Value = new FixedPoint8(-12_3400_0000),
            };
            return item;
        }
    } 

Reader,WriterはDoubleと比較して90%速い
Deserialize,SerializeはDouble,Decimalと比較して55%～60%速い
Int32,Int64と比べても差はほとんどない。


byte[]でReader,Writer,Deserialize,Serializeの比較

| Method                 | Runtime  | Mean       | Error    | StdDev   | Median     | Ratio | RatioSD |
|----------------------- |--------- |-----------:|---------:|---------:|-----------:|------:|--------:|
| ReadInt                | .NET 7.0 |   130.2 ns |  2.71 ns |  7.98 ns |   134.1 ns |  1.00 |    0.00 |
| ReadInt                | .NET 8.0 |   113.0 ns |  2.96 ns |  8.72 ns |   118.0 ns |  0.87 |    0.09 |
| ReadLong               | .NET 7.0 |   126.2 ns |  2.54 ns |  6.37 ns |   128.9 ns |  1.00 |    0.00 |
| ReadLong               | .NET 8.0 |   106.7 ns |  2.22 ns |  6.50 ns |   108.5 ns |  0.85 |    0.07 |
| ReadDouble             | .NET 7.0 | 1,475.4 ns | 29.43 ns | 27.52 ns | 1,474.1 ns |  1.00 |    0.00 |
| ReadDouble             | .NET 8.0 |   645.2 ns | 10.73 ns | 10.04 ns |   644.3 ns |  0.44 |    0.01 |
| ReadFixedPoint8        | .NET 7.0 |   162.8 ns |  3.27 ns |  7.63 ns |   165.4 ns |  1.00 |    0.00 |
| ReadFixedPoint8        | .NET 8.0 |   157.8 ns |  3.15 ns |  5.08 ns |   158.8 ns |  0.96 |    0.05 |
| DeserializeInt         | .NET 7.0 |   834.3 ns | 16.55 ns | 30.68 ns |   830.0 ns |  1.00 |    0.00 |
| DeserializeInt         | .NET 8.0 |   702.8 ns | 10.28 ns |  9.61 ns |   703.3 ns |  0.84 |    0.04 |
| DeserializeLong        | .NET 7.0 |   795.9 ns | 15.73 ns | 25.40 ns |   798.6 ns |  1.00 |    0.00 |
| DeserializeLong        | .NET 8.0 |   710.5 ns | 13.43 ns | 13.79 ns |   703.8 ns |  0.89 |    0.03 |
| DeserializeDouble      | .NET 7.0 | 2,414.8 ns | 45.77 ns | 61.11 ns | 2,410.4 ns |  1.00 |    0.00 |
| DeserializeDouble      | .NET 8.0 | 1,302.8 ns | 16.91 ns | 15.81 ns | 1,298.2 ns |  0.54 |    0.01 |
| DeserializeDecimal     | .NET 7.0 | 2,337.6 ns | 46.32 ns | 76.11 ns | 2,341.6 ns |  1.00 |    0.00 |
| DeserializeDecimal     | .NET 8.0 | 2,052.8 ns | 39.57 ns | 50.04 ns | 2,069.0 ns |  0.89 |    0.03 |
| DeserializeFixedPoint8 | .NET 7.0 |   834.8 ns | 16.03 ns | 19.08 ns |   833.5 ns |  1.00 |    0.00 |
| DeserializeFixedPoint8 | .NET 8.0 |   763.5 ns | 14.82 ns | 13.14 ns |   764.6 ns |  0.92 |    0.03 |
| WriteInt               | .NET 7.0 |   159.3 ns |  2.76 ns |  3.39 ns |   158.3 ns |  1.00 |    0.00 |
| WriteInt               | .NET 8.0 |   151.4 ns |  1.78 ns |  1.58 ns |   151.1 ns |  0.95 |    0.03 |
| WriteLong              | .NET 7.0 |   165.8 ns |  3.35 ns |  9.66 ns |   161.0 ns |  1.00 |    0.00 |
| WriteLong              | .NET 8.0 |   187.2 ns |  1.12 ns |  0.99 ns |   187.2 ns |  1.14 |    0.07 |
| WriteDouble            | .NET 7.0 | 1,347.3 ns | 25.72 ns | 35.21 ns | 1,360.8 ns |  1.00 |    0.00 |
| WriteDouble            | .NET 8.0 |   817.6 ns | 16.04 ns | 16.48 ns |   814.1 ns |  0.61 |    0.02 |
| WriteFixedPoint8       | .NET 7.0 |   153.1 ns |  2.25 ns |  2.11 ns |   152.9 ns |  1.00 |    0.00 |
| WriteFixedPoint8       | .NET 8.0 |   150.6 ns |  0.74 ns |  0.61 ns |   150.7 ns |  0.98 |    0.01 |
| SerializeInt           | .NET 7.0 |   679.6 ns | 13.43 ns | 19.27 ns |   679.0 ns |  1.00 |    0.00 |
| SerializeInt           | .NET 8.0 |   610.7 ns | 12.06 ns | 18.06 ns |   615.6 ns |  0.90 |    0.04 |
| SerializeLong          | .NET 7.0 |   657.1 ns | 13.10 ns | 23.62 ns |   652.0 ns |  1.00 |    0.00 |
| SerializeLong          | .NET 8.0 |   689.1 ns | 12.00 ns | 11.22 ns |   688.9 ns |  1.03 |    0.04 |
| SerializeDouble        | .NET 7.0 | 1,897.7 ns | 27.49 ns | 22.95 ns | 1,906.6 ns |  1.00 |    0.00 |
| SerializeDouble        | .NET 8.0 | 1,364.1 ns | 27.09 ns | 40.54 ns | 1,368.9 ns |  0.72 |    0.02 |
| SerializeDecimal       | .NET 7.0 | 1,838.4 ns | 36.08 ns | 46.92 ns | 1,846.1 ns |  1.00 |    0.00 |
| SerializeDecimal       | .NET 8.0 | 1,589.5 ns | 31.23 ns | 32.07 ns | 1,589.3 ns |  0.87 |    0.03 |
| SerializeFixedPoint8   | .NET 7.0 |   719.8 ns | 14.06 ns | 15.63 ns |   716.9 ns |  1.00 |    0.00 |
| SerializeFixedPoint8   | .NET 8.0 |   646.3 ns | 12.40 ns | 10.99 ns |   643.2 ns |  0.90 |    0.02 |

■ **Api定義**

|                 プロパティ|                                 説明|
| ---------------------- | ----------------------------------- |
|MaxValue                |FixedPoint8の最大値(92233720368.54775807)を返します|
|MinValue                |FixedPoint8の最小値(-92233720368.54775808)を返します|
|Zero                    |FixedPoint8の0を返します|
|One                     |FixedPoint8の1を返します|
|Radix                   |基数として10を返します|
|AdditiveIdentity        |FixedPoint8の0を返します|
|MultiplicativeIdentity  |FixedPoint8の1を返します|


|キャスト (FixedPoint8⇔各数値型)| 
| -------------------------- |
|sbyte                       |
|byte                        |
|short                       |
|ushort                      |
|int                         |
|uint                        |
|long                        |
|ulong                       |
|float                       |
|double                      |
|decimal                     |


|                    オペレーター| 説明|
| -------------------------- | -------------------------- |
|+(FixedPoint8,FixedPoint8)  | |
|-(FixedPoint8,FixedPoint8)  | |
|*(FixedPoint8,long)         | |
|*(FixedPoint8,ulong)        | |
|*(FixedPoint8,FixedPoint8)  |※速度最適化未実施|
|/(FixedPoint8,long)         | |
|/(FixedPoint8,ulong)        | |
|/(FixedPoint8,FixedPoint8)  |※速度最適化未実施|
|==(FixedPoint8,FixedPoint8) | |
|!=(FixedPoint8,FixedPoint8) | |
|<(FixedPoint8,FixedPoint8)  | |
|<=(FixedPoint8,FixedPoint8) | |
|>(FixedPoint8,FixedPoint8)  | |
|>=(FixedPoint8,FixedPoint8) | |
|%(FixedPoint8,FixedPoint8)  | |
|++(FixedPoint8)             | |
|--(FixedPoint8)             | |
|+(FixedPoint8)              | |
|-(FixedPoint8)              | |


|メソッド|説明|
| -------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ |
|FromInnerValue(long)|引数で渡した内部の値(long)からFixedPoint8の値を作って返します|
|Parse(string)|文字列をFixedPoint8に変換します|
|Parse(ReadOnlySpan&lt;char&gt;)|ReadOnlySpan&lt;char&gt;からFixedPoint8に変換します|
|Parse(ReadOnlySpan&lt;byte&gt;)|ReadOnlySpan&lt;byte&gt;からFixedPoint8に変換します|
|Parse(string, IFormatProvider?)|stringをdecimalに変換後、FixedPoint8にキャストします※速度最適化未実施|
|Parse(ReadOnlySpan&lt;char&gt;, IFormatProvider?)|ReadOnlySpan&lt;char&gt;をdecimalに変換後、FixedPoint8にキャストします※速度最適化未実施|
|Parse(string, NumberStyles, IFormatProvider?)|stringからFixedPoint8に変換します　※速度最適化未実施|
|Parse(ReadOnlySpan&lt;char&gt;, NumberStyles, IFormatProvider?)|ReadOnlySpan&lt;char&gt;からFixedPoint8に変換します　※速度最適化未実施|
|TryParse([NotNullWhen(true)] string?, out FixedPoint8)|文字列をFixedPoint8に変換を試みます。変換失敗時はfalseを返します|
|TryParse(ReadOnlySpan&lt;char&gt;, out FixedPoint8)|ReadOnlySpan&lt;char&gt;からFixedPoint8に変換を試みます。変換失敗時はfalseが返ります|
|TryParse(ReadOnlySpan&lt;byte&gt;, out FixedPoint8)|ReadOnlySpan&lt;byte&gt;からFixedPoint8に変換を試みます。変換失敗時はfalseが返ります|
|TryParse([NotNullWhen(true)] string?, IFormatProvider?, [MaybeNullWhen(false)] out FixedPoint8)|stringをdecimalに変換後、FixedPoint8にキャストします。失敗時はfalseを返します ※ 速度最適化未実施|
|TryParse(ReadOnlySpan&lt;char&gt;, IFormatProvider?, [MaybeNullWhen(false)] out FixedPoint8)|ReadOnlySpan&lt;char&gt;をdecimalに変換後、FixedPoint8にキャストします。失敗時はfalseを返します ※ 速度最適化未実施|
|TryParse([NotNullWhen(true)] string?, NumberStyles, IFormatProvider?, [MaybeNullWhen(false)] out FixedPoint8)|stringからFixedPoint8に変換を試みます。変換失敗時はfalseが返ります　※速度最適化未実施|
|TryParse(ReadOnlySpan&lt;char&gt;, NumberStyles, IFormatProvider?, [MaybeNullWhen(false)] out FixedPoint8)|ReadOnlySpan&lt;char&gt;からFixedPoint8に変換を試みます。変換失敗時はfalseが返ります　※速度最適化未実施|
|ToString()|FixedPoint8からstringに変換します|
|ToString(string?, IFormatProvider?)|FixedPoint8からstringに変換します ※速度最適化未実施|
|ToUtf8()|FixedPoint8からUTF8(byte[])に変換します|
|TryFormat(Span&lt;char&gt; , out int, ReadOnlySpan&lt;char&gt;, IFormatProvider?)|ReadOnlySpan&lt;char&gt;が0以外の時はdecimal.TryFormatを使用し、0の時はWriteChars,TryWriteCharsを使用します。失敗時はfalseを返します|
|WriteChars(ref Span&lt;char&gt;)|Span&lt;char&gt;のLengthを返します|
|WriteChars(IBufferWriter&lt;char&gt;)|IBufferWriterに対してUtf16で書き込みを行います|
|WriteUtf8(ref Span&lt;byte&gt;)|Span&lt;byte&gt;のLengthを返します|
|WriteUtf8(IBufferWriter&lt;byte&gt;)|IBufferWriterに対してUtf8で書き込みを行います|
|TryWriteChars(Span&lt;char&gt; ,out int)|引数の値をstringに書き換えます。失敗時はfalseを返します|
|TryWriteUtf8(Span&lt;byte&gt; ,out int)|引数の値をutf8に書き換えます。失敗時はfalseを返します|
|Equals(object?)|自分自身とobjectが等しいかどうかを返します|
|Equals(FixedPoint8)|自分自身と引数の値が等しいかどうかを返します|
|GetHashCode()|ハッシュコードを返します|
|CompareTo(object?)|objectよりも小さければ-1,同じなら0,大きければ1を返します|
|CompareTo(FixedPoint8)|FixedPoint8よりも小さければ-1,同じなら0,大きければ1を返します|
|Abs(FixedPoint8)|FixedPoint8の絶対値を返します|
|IsCanonical(FixedPoint8)|trueを返します|
|IsComplexNumber(FixedPoint8)|falseを返します|
|IsEvenInteger(FixedPoint8)|偶数の整数かどうかを判断します。違う場合はfalseを返します|
|IsFinite(FixedPoint8)|trueを返します|
|IsImaginaryNumber(FixedPoint8)|falseを返します|
|IsInfinity(FixedPoint8)|falseを返します|
|IsInteger(FixedPoint8)|整数かどうかを判断します。違う場合はfalseを返します|
|IsNaN(FixedPoint8)|falseを返します|
|IsNegative(FixedPoint8)|負かどうかを返します|
|IsNegativeInfinity(FixedPoint8)|falseを返します|
|IsNormal(FixedPoint8)|0ならfalseを返します。違う場合はtrueを返します|
|IsOddInteger(FixedPoint8)|奇数の整数かどうかを判断します。違う場合はfalseを返します|
|IsPositive(FixedPoint8)|正かどうかを返します|
|IsPositiveInfinity(FixedPoint8)|falseを返します|
|IsRealNumber(FixedPoint8)|trueを返します|
|IsSubnormal(FixedPoint8)|falseを返します|
|IsZero(FixedPoint8)|0かどうかを返します|
|MaxMagnitude(FixedPoint8, FixedPoint8)|比較して大きい方の値を返します|
|MaxMagnitudeNumber(FixedPoint8, FixedPoint8)|比較して大きい方の値を返します|
|MinMagnitude(FixedPoint8, FixedPoint8)|比較して小さい方の値を返します|
|MinMagnitudeNumber(FixedPoint8, FixedPoint8)|比較して小さい方の値を返します|
|Round()|整数に丸めます。丸め方法は四捨五入で0.5の時はは1つ上の桁が偶数になるように丸めます(銀行丸め)|
|Round(int)|小数点以下を指定した桁数に丸めます。丸め方法は四捨五入で0.5の時はは1つ上の桁が偶数になるように丸めます(銀行丸め)|
|Floor()|整数に丸めます。丸め方法は負の最大値に近づくように丸めます|
|Floor(int)|小数点以下を指定した桁数に丸めます。丸め方法は負の最大値に近づくように丸めます|
|Truncate()|整数に丸めます。丸め方法は0に近づくように丸めます|
|Truncate(int)|小数点以下を指定した桁数に丸めます。丸め方法は0に近づくように丸めます|
|Ceiling()|整数に丸めます。丸め方法は正の最大値に近づくように丸めます|
|Ceiling(int)|小数点以下を指定した桁数に丸めます。丸め方法は正の最大値に近づくように丸めます|


■ 実装説明

・ FixedPointとUtf8の相互変換はatoi/itoaを使用、ソースコードは[Utf8Json](https://github.com/neuecc/Utf8Json)からの移植を行いました。

・ 内部の計算はできる限りuint,ulongを使うように実装しています。(int,longより乗算、除算が速いため)

・ 10,100,1000・・・で乗算、除算する場合、ループではなく配列を使用しています。

・ 偶数、奇数判定は速度に重点を置くため％を使用せずに実装しています。
