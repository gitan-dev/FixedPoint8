<<<<<<< HEAD
﻿using System;
using Utf8Json;

namespace Gitan.FixedPoint8;

public readonly struct Utf8Json_FixedPoint8
{
    public const int InnerPower = 100_000_000;
}

public sealed class FixedPoint8Formatter : IJsonFormatter<FixedPoint8>
{
    public static readonly FixedPoint8Formatter Default = new();

    public void Serialize(ref JsonWriter writer, FixedPoint8 value, IJsonFormatterResolver formatterResolver)
    {
        writer.WriteFixedPoint8(value);
    }

    public FixedPoint8 Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
        return reader.ReadFixedPoint8();
    }
}

public static class WriterExtensions
{
    public static void WriteQuotedFixedPoint8(ref this JsonWriter writer, FixedPoint8 value)
    {
        writer.WriteQuotation();
        writer.WriteFixedPoint8(value);
        writer.WriteQuotation();
    }

    public static void WriteFixedPoint8(ref this JsonWriter writer, FixedPoint8 value)
    {
        writer.EnsureCapacity(21);

        uint num1, num2, num3, div;
        ulong valueA, valueB;


        if (value.InnerValue < 0)
        {
            if (value.InnerValue == long.MinValue)
            {
                ReadOnlySpan<byte> minValue = "-92233720368.54775808"u8;
                foreach (var b in minValue)
                {
                    writer.WriteRawUnsafe(b);
                }
                return;
            }

            writer.WriteRawUnsafe((byte)'-');
            valueB = (ulong)(unchecked(-value.InnerValue));
        }
        else
        {
            valueB = (ulong)value.InnerValue;
        }

        valueA = valueB / Utf8Json_FixedPoint8.InnerPower;

        uint underPoint = (uint)(valueB - (valueA * Utf8Json_FixedPoint8.InnerPower));

        if (valueA < 10000)
        {
            num1 = (uint)valueA;
            if (num1 < 10) { goto L1; }
            if (num1 < 100) { goto L2; }
            if (num1 < 1000) { goto L3; }
            goto L4;
        }
        else
        {
            valueB = valueA / 10000;
            num1 = (uint)(valueA - valueB * 10000);
            if (valueB < 10000)
            {
                num2 = (uint)valueB;
                if (num2 < 100)
                {
                    if (num2 < 10) { goto L5; }
                    goto L6;
                }
                if (num2 < 1000) { goto L7; }
                goto L8;
            }
            else
            {
                valueA = valueB / 10000;
                num2 = (uint)(valueB - valueA * 10000);
                {
                    num3 = (uint)valueA;
                    if (num3 < 100)
                    {
                        if (num3 < 10) { goto L9; }
                        goto L10;
                    }
                    if (num3 < 1000) { goto L11; }
                    goto L12;

                L12:
                    writer.WriteRawUnsafe((byte)('0' + (div = Div1000(num3))));
                    num3 -= div * 1000;
                L11:
                    writer.WriteRawUnsafe((byte)('0' + (div = Div100(num3))));
                    num3 -= div * 100;
                L10:
                    writer.WriteRawUnsafe((byte)('0' + (div = Div10(num3))));
                    num3 -= div * 10;
                L9:
                    writer.WriteRawUnsafe((byte)('0' + num3));
                }
            }
        L8:
            writer.WriteRawUnsafe((byte)('0' + (div = Div1000(num2))));
            num2 -= div * 1000;
        L7:
            writer.WriteRawUnsafe((byte)('0' + (div = Div100(num2))));
            num2 -= div * 100;
        L6:
            writer.WriteRawUnsafe((byte)('0' + (div = Div10(num2))));
            num2 -= div * 10;
        L5:
            writer.WriteRawUnsafe((byte)('0' + num2));

        }

    L4:
        writer.WriteRawUnsafe((byte)('0' + (div = Div1000(num1))));
        num1 -= div * 1000;
    L3:
        writer.WriteRawUnsafe((byte)('0' + (div = Div100(num1))));
        num1 -= div * 100;
    L2:
        writer.WriteRawUnsafe((byte)('0' + (div = Div10(num1))));
        num1 -= div * 10;
    L1:
        writer.WriteRawUnsafe((byte)('0' + num1));


        if (underPoint > 0)
        {

            writer.WriteRawUnsafe((byte)'.');

            while (underPoint > 0)
            {
                var num = underPoint / 10_000_000;

                writer.WriteRawUnsafe((byte)('0' + num));

                underPoint = underPoint * 10 - num * Utf8Json_FixedPoint8.InnerPower;
            }
        }
        return;
    }

    static uint Div1000(uint value) => value * 8389 >> 23;
    static uint Div100(uint value) => value * 5243 >> 19;
    static uint Div10(uint value) => value * 6554 >> 16;

}

public static class ReaderExtensions
{
    public static FixedPoint8 ReadQuotedFixedPoint8(ref this JsonReader reader)
    {
        reader.SkipWhiteSpace();
#if DEBUG
        if (reader.GetBufferUnsafe()[reader.GetCurrentOffsetUnsafe()] != (byte)'"') throw new Exception("QuotedFixedPoint8はJsonが「\"」で始まる必要があります。");
#endif
        reader.AdvanceOffset(1);
        FixedPoint8 value = reader.ReadFixedPoint8();
        reader.AdvanceOffset(1);
        return value;
    }

    public static FixedPoint8 ReadFixedPoint8(ref this JsonReader reader)
    {
        reader.SkipWhiteSpace();

        var buffer = reader.GetBufferUnsafe();
        int offset = reader.GetCurrentOffsetUnsafe();
        //double result = reader.ReadDouble(); 
        int sign = 1; // 1 = + ,-1 = -
        long overPoint = 0;
        long underPoint = 0;

        // + - の処理
        if (offset >= buffer.Length) { throw new Exception("noData"); }
        if (buffer[offset] == (byte)'-')
        {
            offset++;
            sign = -1;
        }
        else if (buffer[offset] == (byte)'+')
        {
            offset++;
        }

        //　0始まりの時
        if (offset >= buffer.Length) { throw new Exception("noData"); }
        if (buffer[offset] == (byte)'0')
        {
            offset++;
            if (offset >= buffer.Length) { throw new Exception("noData"); }
            if (buffer[offset] != (byte)'.')
            {
                while (offset < buffer.Length)
                {
                    if (buffer[offset] != (byte)'0') { break; }
                    offset++;
                }
                reader.AdvanceOffset(offset - reader.GetCurrentOffsetUnsafe());
                return FixedPoint8.Zero;
            }
        }

        // 0～9の処理
        while (offset < buffer.Length && buffer[offset] >= (byte)'0' && buffer[offset] <= (byte)'9')
        {
            int digit = (int)(buffer[offset] - (byte)'0');
            offset++;
            overPoint = overPoint * 10 + digit;
        }

        // .の処理
        if (offset >= buffer.Length)
        {
            return new FixedPoint8(overPoint * Utf8Json_FixedPoint8.InnerPower + underPoint);
        }
        if (buffer[offset] == (byte)'.')
        {
            offset++;

            long power = 100_000_000_000_000_000;
            while (offset < buffer.Length && buffer[offset] >= (byte)'0' && buffer[offset] <= (byte)'9')
            {
                long digit = (long)(buffer[offset] - (byte)'0');
                offset++;

                underPoint += digit * power;
                power /= 10;
            }
        }

        // e,Eじゃなかったら
        if (buffer[offset] != (byte)'e' && buffer[offset] != (byte)'E')
        {
            reader.AdvanceOffset(offset - reader.GetCurrentOffsetUnsafe());
            return new FixedPoint8((overPoint * Utf8Json_FixedPoint8.InnerPower + underPoint / 10_000_000_000) * sign);
        }

        // e,Eだったら
        int powerSignPower = 1;
        int powerNum = 0;
        offset++;

        if (buffer[offset] == (byte)'-')
        {
            offset++;
            powerSignPower = -1;
        }
        else if (buffer[offset] == (byte)'+')
        {
            offset++;
        }

        while (offset < buffer.Length && buffer[offset] >= (byte)'0' && buffer[offset] <= (byte)'9')
        {
            int digit = (int)(buffer[offset] - (byte)'0');
            offset++;
            powerNum = powerNum * 10 + digit;
        }

        powerNum *= powerSignPower;

        var powerNumOver = powerNum + 8;
        long calcOver;
        if (powerNumOver >= 0)
        {
            calcOver = overPoint * powerArray[powerNumOver];
        }
        else
        {
            calcOver = overPoint / powerArray[-powerNumOver];
        }

        var powerNumUnder = powerNum - 10;
        long calcUnder;
        if (powerNumUnder >= 0)
        {
            calcUnder = underPoint * powerArray[powerNumUnder];
        }
        else
        {
            if (powerNumUnder < -18)
            {
                calcUnder = 0;
            }
            else
            {
                calcUnder = underPoint / powerArray[-powerNumUnder];
            }
        }

        reader.AdvanceOffset(offset - reader.GetCurrentOffsetUnsafe());
        return new FixedPoint8((calcOver + calcUnder) * sign);
    }

    static readonly long[] powerArray = new long[] {
    1,
    10,
    100,
    1_000,
    10_000,
    100_000,
    1_000_000,
    10_000_000,
    100_000_000,
    1_000_000_000,
    10_000_000_000,
    100_000_000_000,
    1_000_000_000_000,
    10_000_000_000_000,
    100_000_000_000_000,
    1_000_000_000_000_000,
    10_000_000_000_000_000,
    100_000_000_000_000_000,
    1_000_000_000_000_000_000
    };
=======
﻿using System;
using Utf8Json;

namespace Gitan.FixedPoint8;

public readonly struct Utf8Json_FixedPoint8
{
    public const int InnerPower = 100_000_000;
}

public sealed class FixedPoint8Formatter : IJsonFormatter<FixedPoint8>
{
    public static readonly FixedPoint8Formatter Default = new();

    public void Serialize(ref JsonWriter writer, FixedPoint8 value, IJsonFormatterResolver formatterResolver)
    {
        writer.WriteFixedPoint8(value);
    }

    public FixedPoint8 Deserialize(ref JsonReader reader, IJsonFormatterResolver formatterResolver)
    {
        return reader.ReadFixedPoint8();
    }
}

public static class WriterExtensions
{
    public static void WriteQuotedFixedPoint8(ref this JsonWriter writer, FixedPoint8 value)
    {
        writer.WriteQuotation();
        writer.WriteFixedPoint8(value);
        writer.WriteQuotation();
    }

    public static void WriteFixedPoint8(ref this JsonWriter writer, FixedPoint8 value)
    {
        writer.EnsureCapacity(21);

        uint num1, num2, num3, div;
        ulong valueA, valueB;


        if (value.InnerValue < 0)
        {
            if (value.InnerValue == long.MinValue)
            {
                ReadOnlySpan<byte> minValue = "-92233720368.54775808"u8;
                foreach (var b in minValue)
                {
                    writer.WriteRawUnsafe(b);
                }
                return;
            }

            writer.WriteRawUnsafe((byte)'-');
            valueB = (ulong)(unchecked(-value.InnerValue));
        }
        else
        {
            valueB = (ulong)value.InnerValue;
        }

        valueA = valueB / Utf8Json_FixedPoint8.InnerPower;

        uint underPoint = (uint)(valueB - (valueA * Utf8Json_FixedPoint8.InnerPower));

        if (valueA < 10000)
        {
            num1 = (uint)valueA;
            if (num1 < 10) { goto L1; }
            if (num1 < 100) { goto L2; }
            if (num1 < 1000) { goto L3; }
            goto L4;
        }
        else
        {
            valueB = valueA / 10000;
            num1 = (uint)(valueA - valueB * 10000);
            if (valueB < 10000)
            {
                num2 = (uint)valueB;
                if (num2 < 100)
                {
                    if (num2 < 10) { goto L5; }
                    goto L6;
                }
                if (num2 < 1000) { goto L7; }
                goto L8;
            }
            else
            {
                valueA = valueB / 10000;
                num2 = (uint)(valueB - valueA * 10000);
                {
                    num3 = (uint)valueA;
                    if (num3 < 100)
                    {
                        if (num3 < 10) { goto L9; }
                        goto L10;
                    }
                    if (num3 < 1000) { goto L11; }
                    goto L12;

                L12:
                    writer.WriteRawUnsafe((byte)('0' + (div = Div1000(num3))));
                    num3 -= div * 1000;
                L11:
                    writer.WriteRawUnsafe((byte)('0' + (div = Div100(num3))));
                    num3 -= div * 100;
                L10:
                    writer.WriteRawUnsafe((byte)('0' + (div = Div10(num3))));
                    num3 -= div * 10;
                L9:
                    writer.WriteRawUnsafe((byte)('0' + num3));
                }
            }
        L8:
            writer.WriteRawUnsafe((byte)('0' + (div = Div1000(num2))));
            num2 -= div * 1000;
        L7:
            writer.WriteRawUnsafe((byte)('0' + (div = Div100(num2))));
            num2 -= div * 100;
        L6:
            writer.WriteRawUnsafe((byte)('0' + (div = Div10(num2))));
            num2 -= div * 10;
        L5:
            writer.WriteRawUnsafe((byte)('0' + num2));

        }

    L4:
        writer.WriteRawUnsafe((byte)('0' + (div = Div1000(num1))));
        num1 -= div * 1000;
    L3:
        writer.WriteRawUnsafe((byte)('0' + (div = Div100(num1))));
        num1 -= div * 100;
    L2:
        writer.WriteRawUnsafe((byte)('0' + (div = Div10(num1))));
        num1 -= div * 10;
    L1:
        writer.WriteRawUnsafe((byte)('0' + num1));


        if (underPoint > 0)
        {

            writer.WriteRawUnsafe((byte)'.');

            while (underPoint > 0)
            {
                var num = underPoint / 10_000_000;

                writer.WriteRawUnsafe((byte)('0' + num));

                underPoint = underPoint * 10 - num * Utf8Json_FixedPoint8.InnerPower;
            }
        }
        return;
    }

    static uint Div1000(uint value) => value * 8389 >> 23;
    static uint Div100(uint value) => value * 5243 >> 19;
    static uint Div10(uint value) => value * 6554 >> 16;

}

public static class ReaderExtensions
{
    public static FixedPoint8 ReadQuotedFixedPoint8(ref this JsonReader reader)
    {
        reader.SkipWhiteSpace();
#if DEBUG
        if (reader.GetBufferUnsafe()[reader.GetCurrentOffsetUnsafe()] != (byte)'"') throw new Exception("QuotedFixedPoint8はJsonが「\"」で始まる必要があります。");
#endif
        reader.AdvanceOffset(1);
        FixedPoint8 value = reader.ReadFixedPoint8();
        reader.AdvanceOffset(1);
        return value;
    }

    public static FixedPoint8 ReadFixedPoint8(ref this JsonReader reader)
    {
        reader.SkipWhiteSpace();

        var buffer = reader.GetBufferUnsafe();
        int offset = reader.GetCurrentOffsetUnsafe();
        //double result = reader.ReadDouble(); 
        int sign = 1; // 1 = + ,-1 = -
        long overPoint = 0;
        long underPoint = 0;

        // + - の処理
        if (offset >= buffer.Length) { throw new Exception("noData"); }
        if (buffer[offset] == (byte)'-')
        {
            offset++;
            sign = -1;
        }
        else if (buffer[offset] == (byte)'+')
        {
            offset++;
        }

        //　0始まりの時
        if (offset >= buffer.Length) { throw new Exception("noData"); }
        if (buffer[offset] == (byte)'0')
        {
            offset++;
            if (offset >= buffer.Length) { throw new Exception("noData"); }
            if (buffer[offset] != (byte)'.')
            {
                while (offset < buffer.Length)
                {
                    if (buffer[offset] != (byte)'0') { break; }
                    offset++;
                }
                reader.AdvanceOffset(offset - reader.GetCurrentOffsetUnsafe());
                return FixedPoint8.Zero;
            }
        }

        // 0～9の処理
        while (offset < buffer.Length && buffer[offset] >= (byte)'0' && buffer[offset] <= (byte)'9')
        {
            int digit = (int)(buffer[offset] - (byte)'0');
            offset++;
            overPoint = overPoint * 10 + digit;
        }

        // .の処理
        if (offset >= buffer.Length)
        {
            return new FixedPoint8(overPoint * Utf8Json_FixedPoint8.InnerPower + underPoint);
        }
        if (buffer[offset] == (byte)'.')
        {
            offset++;

            long power = 100_000_000_000_000_000;
            while (offset < buffer.Length && buffer[offset] >= (byte)'0' && buffer[offset] <= (byte)'9')
            {
                long digit = (long)(buffer[offset] - (byte)'0');
                offset++;

                underPoint += digit * power;
                power /= 10;
            }
        }

        // e,Eじゃなかったら
        if (buffer[offset] != (byte)'e' && buffer[offset] != (byte)'E')
        {
            reader.AdvanceOffset(offset - reader.GetCurrentOffsetUnsafe());
            return new FixedPoint8((overPoint * Utf8Json_FixedPoint8.InnerPower + underPoint / 10_000_000_000) * sign);
        }

        // e,Eだったら
        int powerSignPower = 1;
        int powerNum = 0;
        offset++;

        if (buffer[offset] == (byte)'-')
        {
            offset++;
            powerSignPower = -1;
        }
        else if (buffer[offset] == (byte)'+')
        {
            offset++;
        }

        while (offset < buffer.Length && buffer[offset] >= (byte)'0' && buffer[offset] <= (byte)'9')
        {
            int digit = (int)(buffer[offset] - (byte)'0');
            offset++;
            powerNum = powerNum * 10 + digit;
        }

        powerNum *= powerSignPower;

        var powerNumOver = powerNum + 8;
        long calcOver;
        if (powerNumOver >= 0)
        {
            calcOver = overPoint * powerArray[powerNumOver];
        }
        else
        {
            calcOver = overPoint / powerArray[-powerNumOver];
        }

        var powerNumUnder = powerNum - 10;
        long calcUnder;
        if (powerNumUnder >= 0)
        {
            calcUnder = underPoint * powerArray[powerNumUnder];
        }
        else
        {
            if (powerNumUnder < -18)
            {
                calcUnder = 0;
            }
            else
            {
                calcUnder = underPoint / powerArray[-powerNumUnder];
            }
        }

        reader.AdvanceOffset(offset - reader.GetCurrentOffsetUnsafe());
        return new FixedPoint8((calcOver + calcUnder) * sign);
    }

    static readonly long[] powerArray = new long[] {
    1,
    10,
    100,
    1_000,
    10_000,
    100_000,
    1_000_000,
    10_000_000,
    100_000_000,
    1_000_000_000,
    10_000_000_000,
    100_000_000_000,
    1_000_000_000_000,
    10_000_000_000_000,
    100_000_000_000_000,
    1_000_000_000_000_000,
    10_000_000_000_000_000,
    100_000_000_000_000_000,
    1_000_000_000_000_000_000
    };
>>>>>>> 1c034ee14a2fed2c550bfe840c3700753e00637a
}