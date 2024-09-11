using BenchmarkDotNet.Attributes;

namespace Gitan.FixedPoint8;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<保留中>")]

public class BenchMark_TryFormat
{
    static readonly char[] _charBuffer = new char[100];
    static readonly byte[] _byteBuffer = new byte[100];

    static int i = 1234567890;
    static long l = 1234567890;
    static double d = 1234.56789;
    static decimal dec = 1234567890;
    static FixedPoint8 fp8 = FixedPoint8.FromInnerValue(12_3456_7890_0000_0000);
    static FixedPoint8 fp8Double = FixedPoint8.FromInnerValue(1234_5678_9000);

    [Benchmark]
    public int IntTryFormat()
    {
        Span<char> buffer = _charBuffer;

        i.TryFormat(buffer, out int written);
        return written;
    }

    [Benchmark]
    public int LongTryFormat()
    {
        Span<char> buffer = _charBuffer;

        l.TryFormat(buffer, out int written);
        return written;
    }

    [Benchmark]
    public int DoubleTryFormat()
    {
        Span<char> buffer = _charBuffer;

        d.TryFormat(buffer, out int written);
        return written;
    }

    [Benchmark]
    public int DecimalTryFormat()
    {
        Span<char> buffer = _charBuffer;

        dec.TryFormat(buffer, out int written);
        return written;
    }

    [Benchmark]
    public int FixedPoint8TryFormat()
    {
        Span<char> buffer = _charBuffer;

        fp8.TryFormat(buffer, out int written);
        return written;
    }

    [Benchmark]
    public int FixedPoint8DoubleTryFormat()
    {
        Span<char> buffer = _charBuffer;

        fp8Double.TryFormat(buffer, out int written);
        return written;
    }

#if NET8_0
    [Benchmark]
    public int IntUtf8TryFormat()
    {
        Span<byte> buffer = _byteBuffer;

        i.TryFormat(buffer, out int written);
        return written;
    }

    [Benchmark]
    public long LongUtf8TryFormat()
    {
        Span<byte> buffer = _byteBuffer;

        l.TryFormat(buffer, out int written);
        return written;
    }

    [Benchmark]
    public int DoubleUtf8TryFormat()
    {
        Span<byte> buffer = _byteBuffer;

        d.TryFormat(buffer, out int written);
        return written;
    }

    [Benchmark]
    public int DecimalUtf8TryFormat()
    {
        Span<byte> buffer = _byteBuffer;

        dec.TryFormat(buffer, out int written);
        return written;
    }

    [Benchmark]
    public int FixedPoint8Utf8TryFormat()
    {
        Span<byte> buffer = _byteBuffer;

        fp8.TryFormat(buffer, out int written);
        return written;
    }

    [Benchmark]
    public int FixedPoint8DoubleUtf8TryFormat()
    {
        Span<byte> buffer = _byteBuffer;

        fp8Double.TryFormat(buffer, out int written);
        return written;
    }

    [Benchmark]
    public int FixedPoint8DoubleWriteUtf8()
    {
        Span<byte> buffer = _byteBuffer;

        return fp8Double.WriteUtf8Unsafe(ref buffer);
    }

    [Benchmark]
    public int IntUtf8TryWrite()
    {
        Span<byte> buffer = _byteBuffer;

        System.Text.Unicode.Utf8.TryWrite(buffer, $"{i}", out int charsWritten);
        return charsWritten;
    }  
    
    [Benchmark]
    public int Fp8Utf8TryWrite()
    {
        Span<byte> buffer = _byteBuffer;

        System.Text.Unicode.Utf8.TryWrite(buffer, $"{fp8}", out int charsWritten);
        return charsWritten;
    }

#endif

}


//| Method                         | Mean       | Error     | StdDev    |
//|------------------------------- |-----------:|----------:|----------:|
//| IntTryFormat                   |   5.012 ns | 0.1013 ns | 0.0898 ns |
//| LongTryFormat                  |   5.897 ns | 0.1386 ns | 0.1296 ns |
//| DoubleTryFormat                | 100.651 ns | 1.4182 ns | 1.3266 ns |
//| DecimalTryFormat               |  40.083 ns | 0.8000 ns | 1.0118 ns |
//| FixedPoint8TryFormat           |   9.303 ns | 0.2071 ns | 0.3162 ns |
//| FixedPoint8DoubleTryFormat     |   9.896 ns | 0.1996 ns | 0.4078 ns |

//| IntUtf8TryFormat               |   5.523 ns | 0.1352 ns | 0.2732 ns |
//| LongUtf8TryFormat              |   6.718 ns | 0.1591 ns | 0.3492 ns |
//| DoubleUtf8TryFormat            | 105.853 ns | 2.0746 ns | 3.3501 ns |
//| DecimalUtf8TryFormat           |  42.257 ns | 0.8554 ns | 1.2803 ns |
//| FixedPoint8Utf8TryFormat       |   9.227 ns | 0.1962 ns | 0.1927 ns |
//| FixedPoint8DoubleUtf8TryFormat |   9.467 ns | 0.2030 ns | 0.2085 ns |
//| FixedPoint8DoubleWriteUtf8     |   9.198 ns | 0.2013 ns | 0.2951 ns |


//| Method          | Mean      | Error     | StdDev    |
//|---------------- |----------:|----------:|----------:|
//| IntUtf8TryWrite |  7.714 ns | 0.1784 ns | 0.2725 ns |
//| Fp8Utf8TryWrite | 10.481 ns | 0.2288 ns | 0.2894 ns |