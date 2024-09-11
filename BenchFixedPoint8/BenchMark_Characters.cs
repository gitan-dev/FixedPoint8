using BenchmarkDotNet.Attributes;
using System.Buffers;
using System.Text;

namespace Gitan.FixedPoint8;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<保留中>")]


public class BenchMark_Characters
{
    static readonly string[] _stringIntValues = new string[]
    {
        "1000000000",
        "123456789",
        "12345678",
        "1234567",
        "123456",
        "1234",
        "12",
        "1",
        "0",
        "-1000000000",
        "-123456789",
        "-12345678",
        "-1234567",
        "-123456",
        "-1234",
        "-12",
        "-1",
    };

    static readonly string[] _stringValues = new string[]
    {
        "12345678",
        "1234",
        "12",
        "1",
        "0",
        "0.1",
        "0.12",
        "0.1234",
        "0.12345678",
        "-12345678",
        "-1234",
        "-12",
        "-1",
        "-0.1",
        "-0.12",
        "-0.1234",
        "-0.12345678",
    };

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
        //123456789m,
        //1234567890m,
        //12345678901m,
        1234m,
        12m,
        1m,
        0m,
        0.1m,
        0.12m,
        0.1234m,
        0.12345678m,
        -12345678m,
        //-123456789m,
        //-1234567890m,
        //-12345678901m,
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


    /////////////////////////////////////// ROS Parse

    static readonly List<byte[]> _utf8List = GetUtf8List();
    static List<byte[]> GetUtf8List()
    {
        var list = new List<byte[]>();
        foreach (var value in _decimalValues)
        {
            list.Add(Encoding.UTF8.GetBytes($"""{value}"""));
        }
        return list;
    }

    [Benchmark]
    public int StringToInt()
    {
        int result = 0;
        foreach (var value in _stringIntValues)
        {
            result = int.Parse(value);
        }
        return result;
    }

    [Benchmark]
    public double StringToDouble()
    {
        double result = 0.0;
        foreach (var value in _stringValues)
        {
            result = double.Parse(value);
        }
        return result;
    }

    [Benchmark]
    public decimal StringToDecimal()
    {
        decimal result = 0m;
        foreach (var value in _stringValues)
        {
            result = decimal.Parse(value);
        }
        return result;
    }

    [Benchmark]
    public FixedPoint8 StringToFixedPoint8()
    {
        FixedPoint8 result = FixedPoint8.Zero;
        foreach (var value in _stringValues)
        {
            result = FixedPoint8.Parse(value);
        }
        return result;
    }

    [Benchmark]
    public FixedPoint8 Utf8ToFixedPoint8()
    {
        FixedPoint8 result = FixedPoint8.Zero;
        foreach (var value in _utf8List)
        {
            result = FixedPoint8.Parse(value);
        }
        return result;
    }

    [Benchmark]
    public FixedPoint8 CharArrayToFixedPoint8()
    {
        FixedPoint8 result = FixedPoint8.Zero;
        foreach (var value in _stringValues)
        {
            result = FixedPoint8.Parse(value.AsSpan());
        }
        return result;
    }

    ///////////////////////////////////////// GetUtf8

    [Benchmark]
    public int IntToString()
    {
        int sum = 0;
        foreach (var value in _intValues)
        {
            var result = value.ToString();
            sum += result.Length;
        }
        return sum;
    }

    [Benchmark]
    public int DoubleToString()
    {
        int sum = 0;
        foreach (var value in _doubleValues)
        {
            var result = value.ToString();
            sum += result.Length;
        }
        return sum;
    }

    [Benchmark]
    public int DecimalToString()
    {
        int sum = 0;
        foreach (var value in _decimalValues)
        {
            var result = value.ToString();
            sum += result.Length;
        }
        return sum;
    }

    [Benchmark]
    public int FixedPoint8ToString()
    {
        int sum = 0;
        foreach (var value in _fp8Values)
        {
            var result = value.ToString();
            sum += result.Length;
        }
        return sum;
    }

    [Benchmark]
    public int FixedPoint8ToUtf8()
    {
        int sum = 0;
        foreach (var value in _fp8Values)
        {
            var result = value.ToUtf8();
            sum += result.Length;
        }
        return sum;
    }


    /////////////////////////////////////// WriteCharsUtf8

    static readonly char[] _charBuffer = new char[100];
    static readonly byte[] _byteBuffer = new byte[100];
    static readonly System.Buffers.ArrayBufferWriter<char> _charBufferIBuffer = new(100);
    static readonly System.Buffers.ArrayBufferWriter<byte> _byteBufferIBuffer = new(100);

    //Span<char>
    [Benchmark]
    public (char[] buffer, int offset) FixedPoint8_TryWriteChars()
    {
        int offset = 0;

        foreach (var value in _fp8Values)
        {
            var result = value.TryWriteChars(_charBuffer.AsSpan(offset), out var charsWritten);
            if (result == false) { throw new Exception(""); }
            offset += charsWritten;
        }
        return (_charBuffer, offset);
    }

    [Benchmark]
    public (char[] buffer, int offset) FixedPoint8_WriteChars()
    {
        int offset = 0;

        foreach (var value in _fp8Values)
        {
            var dest = _charBuffer.AsSpan(offset);
            var charsWritten = value.WriteCharsUnsafe(ref dest);
            offset += charsWritten;
        }
        return (_charBuffer, offset);
    }

    [Benchmark]
    public ArrayBufferWriter<char> FixedPoint8_WriteCharsIBufferWriter()
    {
        _charBufferIBuffer.Clear();

        foreach (var value in _fp8Values)
        {
            value.WriteChars(_charBufferIBuffer);
        }
        return _charBufferIBuffer;
    }

    // Span<byte>
    [Benchmark]
    public (byte[] buffer, int offset) FixedPoint8_TryWriteUtf8()
    {
        int offset = 0;

        foreach (var value in _fp8Values)
        {
            var result = value.TryWriteUtf8(_byteBuffer.AsSpan(offset), out var charsWritten);
            if (result == false) { throw new Exception(""); }
            offset += charsWritten;
        }
        return (_byteBuffer, offset);
    }

    [Benchmark]
    public (byte[] buffer, int offset) FixedPoint8_WriteUtf8() // destinationに直接書き込み
    {
        int offset = 0;

        foreach (var value in _fp8Values)
        {
            var dest = _byteBuffer.AsSpan(offset);
            var charsWritten = value.WriteUtf8Unsafe(ref dest);
            offset += charsWritten;
        }

        return (_byteBuffer, offset);
    } 
    
    [Benchmark]
    public ArrayBufferWriter<byte> FixedPoint8_WriteUtf8IBufferWriter()
    {
        _byteBufferIBuffer.Clear();

        foreach (var value in _fp8Values)
        {
            value.WriteUtf8(_byteBufferIBuffer);
        }
        return _byteBufferIBuffer;
    }
}


//| Method                              | Mean        | Error     | StdDev   |
//|------------------------------------ |------------:|----------:|---------:|
//| StringToInt                         |   137.62 ns |  2.035 ns | 1.804 ns |
//| StringToDouble                      |   593.97 ns |  4.739 ns | 3.957 ns |
//| StringToDecimal                     |   604.21 ns |  6.515 ns | 5.775 ns |
//| StringToFixedPoint8                 |    73.09 ns |  0.897 ns | 0.839 ns |
//| Utf8ToFixedPoint8                   |    89.19 ns |  0.523 ns | 0.464 ns |
//| CharArrayToFixedPoint8              |    73.02 ns |  0.697 ns | 0.652 ns |

//| IntToString                         |   113.76 ns |  2.301 ns | 6.415 ns |
//| DoubleToString                      | 1,320.87 ns | 10.928 ns | 9.687 ns |
//| DecimalToString                     |   613.09 ns |  8.874 ns | 8.300 ns |
//| FixedPoint8ToString                 |   211.38 ns |  3.677 ns | 4.909 ns |
//| FixedPoint8ToUtf8                   |   175.68 ns |  3.250 ns | 4.764 ns |

//| FixedPoint8_TryWriteChars           |    96.74 ns |  0.312 ns | 0.277 ns |
//| FixedPoint8_WriteChars              |    82.84 ns |  0.619 ns | 0.517 ns |
//| FixedPoint8_WriteCharsIBufferWriter |    94.68 ns |  0.612 ns | 0.511 ns |
//| FixedPoint8_TryWriteUtf8            |    92.46 ns |  0.513 ns | 0.429 ns |
//| FixedPoint8_WriteUtf8               |    80.93 ns |  0.681 ns | 0.637 ns |
//| FixedPoint8_WriteUtf8IBufferWriter  |    97.89 ns |  1.350 ns | 1.127 ns |