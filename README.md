■ **Gitan.FixedPoint8とは**

Gitan.FixedPoint8は、固定小数点で-92233720368.54775808～92233720368.54775807までの数字を扱うことができます。
内部にInt64をもつstructで、10進数の小数点を誤差なく扱うことができます。
実行速度が速いことに重点を置いてUTF8との親和性が高いです。

プロジェクトURL : [https://github.com/gitan-dev/FixedPoint8](https://github.com/gitan-dev/FixedPoint8)

■ **仕様**

・ Gitan.FixedPoint8はuncheckedで動きます、オーバーフローが発生する値でエラーは発生しませんのでご注意ください。

・ FixedPoint8同士の乗算、除算（速度最適化未実施）


■ **使用方法**

NuGetパッケージ : Gitan.FixedPoint8

NuGetを使用してGitan.FixedPoint8パッケージをインストールします。

FixedPoint8を使用する方法を以下に記載します。

    using Gitan.FixedPoint8;

    public void fp8Test()       
    {
        var v1 = FixedPoint8.FromDouble(12.34);        
        var v2 = FixedPoint8.FromDecimal(12.34m);
        //内部的には、Int64の1234000000となる。

        var add = v1 + v2;
        var sub = v1 - v2;
        var mul = v1 * v2;
        var div = v1 / v2;
    }


NuGetパッケージ : Gitan.Utf8Json_FixedPoint8

NuGetを使用してGitan.Utf8Json_FixedPoint8をインストールします。　※依存関係で[Utf8Json](https://github.com/neuecc/Utf8Json)がインストールされます。

Gitan.Utf8Json_FixedPoint8はJsonとFixedPoint8をRead,Writeします。

ReadFixedPoint8,WriteFixedPoint8の処理は[Utf8Json](https://github.com/neuecc/Utf8Json/tree/master/src/Utf8Json)の[NumberConverter](https://github.com/neuecc/Utf8Json/blob/master/src/Utf8Json/Internal/NumberConverter.cs)を部分引用しています。

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


Utf8Jsonを使用してFixedPoint8シリアライズ/デシリアライズする方法を以下に記載します。

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


|               Method |            Mean |          Error |         StdDev |          Median |
|--------------------- |----------------:|---------------:|---------------:|----------------:|
|              Mul2Int |   4,405.6419 ns |     75.6055 ns |     67.0223 ns |   4,378.1532 ns |
|           Mul2Double |  11,597.1852 ns |    110.5118 ns |     97.9658 ns |  11,550.6607 ns |
|          Mul2Decimal | 171,681.9318 ns |  2,222.0380 ns |  1,969.7788 ns | 171,415.0513 ns |
|   MulInt2FixedPoint8 |   5,095.5557 ns |    229.8811 ns |    652.1343 ns |   5,063.8119 ns |
|      Mul2FixedPoint8 | 740,576.5320 ns | 14,567.0630 ns | 14,306.8037 ns | 736,481.2988 ns |
|             Mul10Int |   4,198.6275 ns |     59.8123 ns |     55.9485 ns |   4,207.6736 ns |
|          Mul10Double |  11,566.6072 ns |     85.9625 ns |     80.4094 ns |  11,562.5351 ns |
|         Mul10Decimal | 182,478.7274 ns |  3,625.5590 ns |  6,444.4275 ns | 181,066.9189 ns |
|  MulInt10FixedPoint8 |   7,960.9773 ns |    153.5037 ns |    188.5164 ns |   7,924.3217 ns |
|     Mul10FixedPoint8 | 737,801.1133 ns | 13,234.3068 ns | 12,379.3788 ns | 739,840.6250 ns |
|              Add2Int |   5,130.1053 ns |    206.7801 ns |    603.1874 ns |   5,062.9269 ns |
|           Add2Double |  11,804.3303 ns |    129.1374 ns |    114.4769 ns |  11,841.0278 ns |
|          Add2Decimal | 229,580.8140 ns |  3,002.1355 ns |  2,661.3149 ns | 229,196.7773 ns |
|      Add2FixedPoint8 |   8,053.5049 ns |     95.3321 ns |     84.5094 ns |   8,045.7703 ns |
|             Add10Int |   5,179.5364 ns |    199.3422 ns |    581.4905 ns |   5,021.6770 ns |
|          Add10Double |  11,711.3880 ns |    170.7424 ns |    159.7126 ns |  11,681.6528 ns |
|         Add10Decimal | 227,311.7928 ns |  3,070.6181 ns |  2,872.2581 ns | 226,703.4302 ns |
|     Add10FixedPoint8 |   8,009.1789 ns |     74.9214 ns |     66.4159 ns |   7,994.0407 ns |
|              Sub2Int |   5,222.6691 ns |    258.3863 ns |    761.8579 ns |   4,992.6422 ns |
|           Sub2Double |  11,577.0315 ns |     98.2322 ns |     87.0803 ns |  11,558.0177 ns |
|          Sub2Decimal | 236,974.9669 ns |  3,245.1285 ns |  2,876.7219 ns | 236,256.2500 ns |
|      Sub2FixedPoint8 |   8,148.7481 ns |    118.4674 ns |     98.9257 ns |   8,159.6352 ns |
|             Sub10Int |   5,108.3281 ns |    219.5566 ns |    640.4570 ns |   4,884.5806 ns |
|          Sub10Double |  11,752.1292 ns |    231.5493 ns |    237.7841 ns |  11,619.3405 ns |
|         Sub10Decimal | 229,858.3089 ns |  2,906.6454 ns |  2,718.8779 ns | 228,971.4355 ns |
|     Sub10FixedPoint8 |   8,629.7839 ns |    163.8589 ns |    323.4414 ns |   8,561.5463 ns |


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

|                 Method |       Mean |    Error |   StdDev |
|----------------------- |-----------:|---------:|---------:|
|                ReadInt |   396.8 ns |  7.52 ns |  8.66 ns |
|               ReadLong |   400.5 ns |  7.89 ns | 15.00 ns |
|             ReadDouble | 1,793.0 ns | 34.33 ns | 30.43 ns |
|        ReadFixedPoint8 |   446.5 ns |  8.78 ns | 12.59 ns |
|         DeserializeInt |   816.0 ns | 16.06 ns | 24.52 ns |
|        DeserializeLong |   831.9 ns | 16.53 ns | 23.70 ns |
|      DeserializeDouble | 2,187.1 ns | 43.16 ns | 56.12 ns |
|     DeserializeDecimal | 2,360.5 ns | 45.31 ns | 53.94 ns |
| DeserializeFixedPoint8 |   824.5 ns | 15.96 ns | 14.93 ns |
|               WriteInt |   172.4 ns |  3.46 ns |  8.88 ns |
|              WriteLong |   163.5 ns |  3.29 ns |  5.93 ns |
|            WriteDouble | 1,301.8 ns | 25.96 ns | 41.93 ns |
|       WriteFixedPoint8 |   151.5 ns |  2.06 ns |  1.83 ns |
|           SerializeInt |   630.3 ns | 12.45 ns | 13.84 ns |
|          SerializeLong |   646.4 ns | 12.66 ns | 21.49 ns |
|        SerializeDouble | 1,916.9 ns | 37.70 ns | 40.33 ns |
|       SerializeDecimal | 1,793.2 ns | 29.51 ns | 24.64 ns |
|   SerializeFixedPoint8 |   667.8 ns | 11.17 ns | 10.45 ns |

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


|                    オペレーター| 
| -------------------------- |
|+(FixedPoint8,FixedPoint8)  |
|-(FixedPoint8,FixedPoint8)  |
|*(FixedPoint8,long)         |
|*(FixedPoint8,ulong)        |
|*(FixedPoint8,FixedPoint8)  |
|/(FixedPoint8,long)         |
|/(FixedPoint8,ulong)        |
|/(FixedPoint8,FixedPoint8)  |
|==(FixedPoint8,FixedPoint8) |
|!=(FixedPoint8,FixedPoint8) |
|<(FixedPoint8,FixedPoint8)  |
|<=(FixedPoint8,FixedPoint8) |
|>(FixedPoint8,FixedPoint8)  |
|>=(FixedPoint8,FixedPoint8) |
|%(FixedPoint8,FixedPoint8)  |
|++(FixedPoint8)             |
|--(FixedPoint8)             |
|+(FixedPoint8)              |
|-(FixedPoint8)              |


|メソッド|説明|
| -------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------ |
|FromInnerValue(long)|longをFixedPoint8の値で返します|
|FromDouble(double)|doubleをFixedPoint8の値で返します|
|FromDecimal(decimal)|decimalのFixedPoint8の値で返します|
|Parse(string)|文字列をFixedPoint8に変換しようとします|
|Parse(ReadOnlySpan<char>)|読み取り専用のcharを、FixedPoint8に変換しようとします|
|Parse(ReadOnlySpan<byte>)|読み取り専用のbyteを、FixedPoint8に変換しようとします|
|TryParse([NotNullWhen(true)] string?, out FixedPoint8)|文字列をFixedPoint8に変換しようとします。戻り値は、変換が成功したか失敗したかを示します|
|TryParse(ReadOnlySpan<char>, out FixedPoint8)|読み取り専用のcharを、FixedPoint8に変換しようとします。戻り値は、変換が成功したか失敗したかを示します|
|TryParse(ReadOnlySpan<byte>, out FixedPoint8)|読み取り専用のbyteを、FixedPoint8に変換しようとします。 戻り値は、変換が成功したか失敗したかを示します|
|ToString()|このインスタンスの数値を同等の文字列表現に変換します|
|ToUtf8()|このインスタンスの数値をUTF8に変換します|
|Equals(object?)|このインスタンスが指定されたオブジェクトと等しいかどうかを示す値を返します|
|Equals(FixedPoint8)|このインスタンスが指定されたFixedPoint8と等しいかどうかを示す値を返します|
|GetHashCode()|このインスタンスのハッシュ コードを返します|
|CompareTo(object?)|このインスタンスを指定されたオブジェクトと比較し、それらの相対値の指示を返します|
|CompareTo(FixedPoint8)|このインスタンスを指定されたFixedPoint8と比較し、それらの相対値の指示を返します|
|Abs(FixedPoint8)|FixedPoint8の絶対値を返します|
|IsCanonical(FixedPoint8)|trueを返します|
|IsComplexNumber(FixedPoint8)|falseを返します|
|IsEvenInteger(FixedPoint8)|値が偶数の整数を表すかどうかを判断します|
|IsFinite(FixedPoint8)|trueを返します|
|IsImaginaryNumber(FixedPoint8)|falseを返します|
|IsInfinity(FixedPoint8)|falseを返します|
|IsInteger(FixedPoint8)|longの可能な最小値を返します|
|IsNaN(FixedPoint8)|falseを返します|
|IsNegative(FixedPoint8)|値が負かどうかを判断します|
|IsNegativeInfinity(FixedPoint8)|falseを返します|
|IsNormal(FixedPoint8)|trueを返します|
|IsOddInteger(FixedPoint8)|値が奇数の整数を表すかどうかを判断します|
|IsPositive(FixedPoint8)|値が正かどうかを判断します|
|IsPositiveInfinity(FixedPoint8)|falseを返します|
|IsRealNumber(FixedPoint8)|trueを返します|
|IsSubnormal(FixedPoint8)|falseを返します|
|IsZero(FixedPoint8)|値が0かどうかを判断します|
|MaxMagnitude(FixedPoint8, FixedPoint8)|値を比較して大きい方の値を返します|
|MaxMagnitudeNumber(FixedPoint8, FixedPoint8)|値を比較して大きい方の値を返します|
|MinMagnitude(FixedPoint8, FixedPoint8)|値を比較して小さい方の値を返します|
|MinMagnitudeNumber(FixedPoint8, FixedPoint8)|値を比較して小さい方の値を返します|
|Parse(ReadOnlySpan<char>, NumberStyles, IFormatProvider?)|未実装|
|Parse(string, NumberStyles, IFormatProvider?)|未実装|
|TryParse(ReadOnlySpan<char>, NumberStyles, IFormatProvider?, [MaybeNullWhen(false)] out FixedPoint8)|未実装|
|TryParse([NotNullWhen(true)] string?, NumberStyles, IFormatProvider?, [MaybeNullWhen(false)] out FixedPoint8)|未実装|
|TryFormat(Span<char> , out int, ReadOnlySpan<char>, IFormatProvider?)|ReadOnlySpan<char>が0以外の時はdecima.TryFormat、0の時はSpan<char>のLengthを見てWriteChars,TryWriteChars。返り値は変換が成功したか失敗したかを示します|
|TryWriteChars(Span<char> ,out int)|現在の整数値インスタンスの値の、指定したcharスパンへの書式設定を試みます。戻り値は、変換が成功したか失敗したかを示します|
|WriteChars(ref Span<char>)|現在の整数値インスタンスの値の、指定したcharスパンへの書式設定を試みます|
|TryWriteChars(Span<byte> ,out int)|現在の整数値インスタンスの値の、指定したbyteスパンへの書式設定を試みます。戻り値は、変換が成功したか失敗したかを示します|
|WriteChars(ref Span<byte>)|現在の整数値インスタンスの値の、指定したbyteスパンへの書式設定を試みます|
|ToString(string?, IFormatProvider?)|※速度最適化未実施|
|Parse(ReadOnlySpan<char>, IFormatProvider?)|読み取り専用のcharを、decimalに変換後、FixedPoint8にキャストします※速度最適化未実施|
|TryParse(ReadOnlySpan<char>, IFormatProvider?, [MaybeNullWhen(false)] out FixedPoint8)|読み取り専用のcharを、decimalに変換後、FixedPoint8にキャストします。戻り値は、変換が成功したか失敗したかを示します|
|Parse(string, IFormatProvider?)|文字列をdecimalに変換後、FixedPoint8にキャストします※速度最適化未実施|
|TryParse([NotNullWhen(true)] string?, IFormatProvider?, [MaybeNullWhen(false)] out FixedPoint8)|文字列をdecimalに変換後、FixedPoint8にキャストします。戻り値は、変換が成功したか失敗したかを示します|
|Round()|最も近い整数に値を丸めます|
|Round(int)|指定した小数点以下の桁数に値を丸めます|
|Floor()|指定した倍精度浮動小数点数以下の数のうち、最大の整数値を返します|
|Floor(int)|指定した小数点以下の桁数に最大の値を返します|
|Truncate()|指定した10進数の整数部を計算します|
|Truncate(int)|指定した小数点以下の桁数に値を計算します|
|Ceiling()|指定した 10進数以上の数のうち、最小の整数値を返します|
|Ceiling(int)|指定した倍精度浮動小数点数以上の数のうち、最小の値を返します|


■ 実装説明

・ 偶数、奇数判定は速度に重点を置くため％を使用せずに実装しています。

・ FixedPointとUtf8の相互変換はatoi/itoaを使用、ソースコードは[Utf8Json](https://github.com/neuecc/Utf8Json)からの移植を行いました。

・ //powerArrayの話
