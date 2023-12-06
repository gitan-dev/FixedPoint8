using Utf8Json;
using BenchmarkDotNet.Attributes;
using System.Text;

namespace Gitan.FixedPoint8;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<保留中>")]

public class BenchMark_Utf8Json
{
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
    ///


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

//|                 Method |       Mean |    Error |   StdDev |
//|----------------------- |-----------:|---------:|---------:|
//|                ReadInt |   396.8 ns |  7.52 ns |  8.66 ns |
//|               ReadLong |   400.5 ns |  7.89 ns | 15.00 ns |
//|             ReadDouble | 1,793.0 ns | 34.33 ns | 30.43 ns |
//|        ReadFixedPoint8 |   446.5 ns |  8.78 ns | 12.59 ns |

//|         DeserializeInt |   816.0 ns | 16.06 ns | 24.52 ns |
//|        DeserializeLong |   831.9 ns | 16.53 ns | 23.70 ns |
//|      DeserializeDouble | 2,187.1 ns | 43.16 ns | 56.12 ns |
//|     DeserializeDecimal | 2,360.5 ns | 45.31 ns | 53.94 ns |
//| DeserializeFixedPoint8 |   824.5 ns | 15.96 ns | 14.93 ns |

//|               WriteInt |   172.4 ns |  3.46 ns |  8.88 ns |
//|              WriteLong |   163.5 ns |  3.29 ns |  5.93 ns |
//|            WriteDouble | 1,301.8 ns | 25.96 ns | 41.93 ns |
//|       WriteFixedPoint8 |   151.5 ns |  2.06 ns |  1.83 ns |

//|           SerializeInt |   630.3 ns | 12.45 ns | 13.84 ns |
//|          SerializeLong |   646.4 ns | 12.66 ns | 21.49 ns |
//|        SerializeDouble | 1,916.9 ns | 37.70 ns | 40.33 ns |
//|       SerializeDecimal | 1,793.2 ns | 29.51 ns | 24.64 ns |
//|   SerializeFixedPoint8 |   667.8 ns | 11.17 ns | 10.45 ns |
}

