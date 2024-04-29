using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Gitan.FixedPoint8;

public readonly struct FixedPoint8 : INumber<FixedPoint8>, IMinMaxValue<FixedPoint8>,ISignedNumber<FixedPoint8>
{
    public static FixedPoint8 MaxValue { get; } = new FixedPoint8(long.MaxValue);
    public static FixedPoint8 MinValue { get; } = new FixedPoint8(long.MinValue);

    public const int InnerPower = 100_000_000;

    public long InnerValue { get; init; }

    public static FixedPoint8 Zero { get; } = new FixedPoint8(0);

    public static FixedPoint8 One { get; } = new FixedPoint8(InnerPower);

    public static int Radix => 10;

    public static FixedPoint8 AdditiveIdentity => Zero;

    public static FixedPoint8 MultiplicativeIdentity => One;

    public static FixedPoint8 NegativeOne => new (-InnerPower);

    public FixedPoint8(long innerValue)
    {
        InnerValue = innerValue;
    }

    // ****************************************
    // FixedPoint8への変換(数値系)
    // **************************************** 
    public static FixedPoint8 FromInnerValue(long value)
    {
        return new FixedPoint8(value);
    }

    private static FixedPoint8 FromDouble(double value)
    {
        return new FixedPoint8((long)(value * InnerPower + (value > 0 ? 0.5 : -0.5)));
    }

    private static FixedPoint8 FromDecimal(decimal value)
    {
        return new FixedPoint8((long)(value * InnerPower + (value > 0m ? 0.5m : -0.5m)));
    }

    // ****************************************
    // FixedPoint8への変換(文字系)
    // ***************************************
    public static void ThrowFormatException(string value)
    {
        throw new FormatException(value);
    }

    public static FixedPoint8 Parse(string s)
    {
        if (TryParse(s.AsSpan(), out var result))
        {
            return result;
        }
        ThrowFormatException($"The input string '{s}' was not in a correct format.");
        return Zero;
    }

    public static FixedPoint8 Parse(ReadOnlySpan<char> s)
    {
        if (TryParse(s, out var result))
        {
            return result;
        }
        ThrowFormatException($"The input ReadOnlySpan<char> '{s}' was not in a correct format.");
        return Zero;
    }

    public static FixedPoint8 Parse(ReadOnlySpan<byte> utf8)
    {
        if (TryParse(utf8, out var result))
        {
            return result;
        }
        ThrowFormatException($"The input ReadOnlySpan<byte> '{utf8.ToString()}' was not in a correct format.");
        return Zero;
    }  

    // 速度最適化未実施
    public static FixedPoint8 Parse(string s, IFormatProvider? provider)
    {
        var decimalValue = decimal.Parse(s, provider);
        return (FixedPoint8)decimalValue;
    }

    // 速度最適化未実施
    public static FixedPoint8 Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        decimal decimalValue = decimal.Parse(s, provider);
        return (FixedPoint8)decimalValue;
    }

    // 速度最適化未実施
    public static FixedPoint8 Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        decimal value = decimal.Parse(s, style, provider);
        return (FixedPoint8)value;
    }

    // 速度最適化未実施
    public static FixedPoint8 Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        decimal value = decimal.Parse(s, style, provider);
        return (FixedPoint8)value;
    }

    // 速度最適化未実
    public static bool TryParse([NotNullWhen(true)] string? s, out FixedPoint8 result)
    {
        return TryParse(s.AsSpan(), out result);
    }

    public static bool TryParse(ReadOnlySpan<char> s, out FixedPoint8 result)
    {
        int offset = 0;
        int sign = 1; // 1 = + ,-1 = -
        ulong overPoint = 0;
        ulong underPoint = 0;
        uint digit;

        if (s.Length == 0) { goto returnFalse; }

        // + - の処理
        if (s[0] == (char)'-')
        {
            offset++;
            sign = -1;
        }
        else if (s[0] == (char)'+')
        {
            offset++;
        }

        // 0～9の処理
        while (true)
        {
            if (offset >= s.Length) { goto returnTrue; }

            digit = (uint)(s[offset] - '0');

            if (digit <= 9)
            {
                offset++;
                overPoint = overPoint * 10 + digit;
            }
            else
            {
                break;
            }
        }

        // .の処理
        if (s[offset] == (char)'.')
        {
            offset++;

            ulong power = 100_000_000_000_000_000;

            while (true)
            {
                if (offset >= s.Length) { goto returnTrue; }

                digit = (uint)(s[offset] - '0');

                if (digit <= 9)
                {
                    offset++;

                    underPoint += digit * power;
                    power /= 10;
                }
                else
                {
                    break;
                }
            }
        }

        // e,Eじゃなかったら
        if (s[offset] != (char)'e' && s[offset] != (char)'E')
        {
            goto returnFalse;
        }

        // e,Eだったら
        int powerSignPower = 1;
        int powerNum = 0;
        offset++;

        if (s[offset] == (char)'-')
        {
            offset++;
            powerSignPower = -1;
        }
        else if (s[offset] == (char)'+')
        {
            offset++;
        }

        while (true)
        {
            if (offset >= s.Length) { break; }

            digit = (uint)(s[offset] - '0');

            if (digit <= 9)
            {
                offset++;
                powerNum = powerNum * 10 + (int)digit;
            }
            else
            {
                goto returnFalse;
            }
        }

        powerNum *= powerSignPower;

        var powerNumOver = powerNum + 8;
        ulong calcOver;
        if (powerNumOver >= 0)
        {
            calcOver = overPoint * powerArray[powerNumOver];
        }
        else
        {
            calcOver = overPoint / powerArray[-powerNumOver];
        }

        var powerNumUnder = powerNum - 10;
        ulong calcUnder;
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

        result = new FixedPoint8((long)(calcOver + calcUnder) * sign);
        return true;


    returnTrue:
        result = new FixedPoint8((long)(overPoint * InnerPower + underPoint / 10_000_000_000) * sign);
        return true;

    returnFalse:
        result = FixedPoint8.Zero;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<byte> utf8, out FixedPoint8 result)
    {
        int offset = 0;
        int sign = 1; // 1 = + ,-1 = -
        ulong overPoint = 0;
        ulong underPoint = 0;
        uint digit;

        if (utf8.Length == 0) { goto returnFalse; }

        // + - の処理
        if (utf8[0] == (byte)'-')
        {
            offset++;
            sign = -1;
        }
        else if (utf8[0] == (byte)'+')
        {
            offset++;
        }

        // 0～9の処理
        while (true)
        {
            if (offset >= utf8.Length) { goto returnTrue; }

            digit = (uint)(utf8[offset] - '0');

            if (digit <= 9)
            {
                offset++;
                overPoint = overPoint * 10 + digit;
            }
            else
            {
                break;
            }
        }

        // .の処理
        if (utf8[offset] == (byte)'.')
        {
            offset++;

            ulong power = 100_000_000_000_000_000;

            while (true)
            {
                if (offset >= utf8.Length) { goto returnTrue; }

                digit = (uint)(utf8[offset] - '0');

                if (digit <= 9)
                {
                    offset++;

                    underPoint += digit * power;
                    power /= 10;
                }
                else
                {
                    break;
                }
            }
        }

        // e,Eじゃなかったら
        if (utf8[offset] != (byte)'e' && utf8[offset] != (byte)'E')
        {
            goto returnFalse;
        }

        // e,Eだったら
        int powerSignPower = 1;
        int powerNum = 0;
        offset++;

        if (utf8[offset] == (byte)'-')
        {
            offset++;
            powerSignPower = -1;
        }
        else if (utf8[offset] == (byte)'+')
        {
            offset++;
        }

        while (true)
        {
            if (offset >= utf8.Length) { break; }

            digit = (uint)(utf8[offset] - '0');

            if (digit <= 9)
            {
                offset++;
                powerNum = powerNum * 10 + (int)digit;
            }
            else
            {
                goto returnFalse;
            }
        }

        powerNum *= powerSignPower;

        var powerNumOver = powerNum + 8;
        ulong calcOver;
        if (powerNumOver >= 0)
        {
            calcOver = overPoint * powerArray[powerNumOver];
        }
        else
        {
            calcOver = overPoint / powerArray[-powerNumOver];
        }

        var powerNumUnder = powerNum - 10;
        ulong calcUnder;
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

        result = new FixedPoint8((long)(calcOver + calcUnder) * sign);
        return true;


    returnTrue:
        result = new FixedPoint8((long)(overPoint * InnerPower + underPoint / 10_000_000_000) * sign);
        return true;

    returnFalse:
        result = FixedPoint8.Zero;
        return false;
    }


    // 速度最適化未実施
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out FixedPoint8 result)
    {
        var success = decimal.TryParse(s, provider, out var decimalResult);
        if (success)
        {
            result = (FixedPoint8)decimalResult;
            return true;
        }
        else
        {
            result = FixedPoint8.Zero;
            return false;
        }
    }

    // 速度最適化未実施
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out FixedPoint8 result)
    {
        var success = decimal.TryParse(s, provider, out var decimalResult);
        if (success)
        {
            result = (FixedPoint8)decimalResult;
            return true;
        }
        else
        {
            result = FixedPoint8.Zero;
            return false;
        }
    }

    // 速度最適化未実施
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out FixedPoint8 result)
    {
        var success = decimal.TryParse(s, style, provider, out var decimalResult);
        if (success)
        {
            result = (FixedPoint8)decimalResult;
            return true;
        }
        else
        {
            result = FixedPoint8.Zero;
            return false;
        }
    }

    // 速度最適化未実施
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out FixedPoint8 result)
    {
        var success = decimal.TryParse(s, style, provider, out var decimalResult);
        if (success)
        {
            result = (FixedPoint8)decimalResult;
            return true;
        }
        else
        {
            result = FixedPoint8.Zero;
            return false;
        }
    }

    // ****************************************
    // FixedPoint8からの変換(文字系)
    // ****************************************

    static readonly byte[] _tableUtf8 = GetTableUtf8();

    static byte[] GetTableUtf8()
    {
        var table = new byte[100 * 2];

        for (int i = 0; i < 100; i++)
        {
            var a = i / 10;
            var b = i - a * 10;
            table[i * 2] = (byte)('0' + a);
            table[i * 2 + 1] = (byte)('0' + b);
        }
        return table;
    }  
    
    static readonly char[] _tableChar = GetTableChar();

    static char[] GetTableChar()
    {
        var table = new char[100 * 2];

        for (int i = 0; i < 100; i++)
        {
            var a = i / 10;
            var b = i - a * 10;
            table[i * 2] = (char)('0' + a);
            table[i * 2 + 1] = (char)('0' + b);
        }
        return table;
    }

    public override string ToString()
    {
        Span<char> buffer = stackalloc char[21];

        int charsWritten = WriteChars(ref buffer);
        return new string(buffer[..charsWritten]);
    }   

    // 速度最適化未実施
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        decimal decimalValue = (decimal)this;
        return decimalValue.ToString(format, formatProvider);
    }

    public byte[] ToUtf8()
    {
        Span<byte> buffer = stackalloc byte[21];

        int charsWritten = WriteUtf8(ref buffer);
        return buffer[..charsWritten].ToArray();
    }

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        if (format.Length != 0)
        {
            decimal dec = (decimal)this;
            return dec.TryFormat(destination, out charsWritten, format, provider);
        }
        if (destination.Length >= 21)
        {
            charsWritten = WriteChars(ref destination);
            return true;
        }
        else
        {
            return TryWriteChars(destination, out charsWritten);
        }
    } 
    
#if NET8_0
    public bool TryFormat(Span<byte> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
    {
        if (format.Length != 0)
        {
            decimal dec = (decimal)this;
            return dec.TryFormat(destination, out charsWritten, format, provider);
        }
        if (destination.Length >= 21)
        {
            charsWritten = WriteUtf8(ref destination);
            return true;
        }
        else
        {
            return TryWriteUtf8(destination, out charsWritten);
        }
    }
#endif


    public int WriteChars(ref Span<char> destination)
    {
        int offset = 0;

        var value = InnerValue;

        uint num1, num2, num3, div;
        ulong valueA, valueB;

        unsafe
        {
            fixed (char* pNumTable = _tableChar)
            fixed (char* pTo = destination)
            {
                var pToCurrent = pTo;

                if (value < 0)
                {
                    if (InnerValue == long.MinValue)
                    {
                        string minValue = "-92233720368.54775808";

                        minValue.CopyTo(destination);
                        return minValue.Length;
                    }

                    *pToCurrent = (char)'-';
                    pToCurrent++;
                    valueB = (ulong)(unchecked(-value));
                }
                else
                {
                    valueB = (ulong)value;
                }

                valueA = valueB / InnerPower;
                uint underPoint = (uint)(valueB - (valueA * InnerPower));


                if (valueA < 10000)
                {
                    num1 = (uint)valueA;
                    if (num1 < 10)
                    {
                        *(pToCurrent++) = (char)('0' + num1);
                        goto L0;
                    }
                    if (num1 < 100) { goto L2; }
                    if (num1 < 1000)
                    {
                        *(pToCurrent++) = (char)('0' + (div = Div100(num1)));
                        num1 -= div * 100;
                        goto L2;
                    }
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
                            if (num2 < 10)
                            {
                                *(pToCurrent++) = (char)('0' + num2);
                                goto L4;
                            }
                            goto L6;
                        }
                        if (num2 < 1000)
                        {
                            *(pToCurrent++) = (char)('0' + (div = Div100(num2)));
                            num2 -= div * 100;
                            goto L6;
                        }
                        goto L8;
                    }
                    else
                    {
                        valueA = valueB / 10000;
                        num2 = (uint)(valueB - valueA * 10000);

                        num3 = (uint)valueA;
                        if (num3 < 100)
                        {
                            if (num3 < 10)
                            {
                                *(pToCurrent++) = (char)('0' + num3);
                                goto L8;
                            }
                            goto L10;
                        }
                        if (num3 < 1000)
                        {
                            *(pToCurrent++) = (char)('0' + (div = Div100(num3)));
                            num3 -= div * 100;
                            goto L10;
                        }
                        goto L12;
                    L12:
                        *(int*)(pToCurrent) = *(int*)(pNumTable + (div = Div100(num3)) * 2);
                        pToCurrent += 2;
                        num3 -= div * 100;
                    L10:
                        *(int*)(pToCurrent) = *(int*)(pNumTable + num3 * 2);
                        pToCurrent += 2;
                    }
                L8:
                    *(int*)(pToCurrent) = *(int*)(pNumTable + (div = Div100(num2)) * 2);
                    pToCurrent += 2;
                    num2 -= div * 100;
                L6:
                    *(int*)(pToCurrent) = *(int*)(pNumTable + num2 * 2);
                    pToCurrent += 2;
                }
            L4:
                // num1は0～9999
                // num1を2桁ずつ、div , num1に分解
                *(int*)(pToCurrent) = *(int*)(pNumTable + (div = Div100(num1)) * 2);
                pToCurrent += 2;
                num1 -= div * 100;
            L2:
                *(int*)(pToCurrent) = *(int*)(pNumTable + num1 * 2);
                pToCurrent += 2;
            L0:
                offset = (int)(pToCurrent - pTo);

                // 小数点以下を最終的に、2桁ずつに分解する。
                // num1:1～2桁 num2:3～4桁 num3:5～6桁 num4:7～8桁
                // 最後に2桁を分解 例、 34→div=3,rem=4
                uint num4;
                uint rem;

                if (underPoint > 0)
                {
                    num4 = underPoint;

                    num4 -= (num2 = num4 / 10000) * 10000;
                    num2 -= (num1 = Div100(num2)) * 100;

                    if (num4 > 0) // 5～8桁を評価
                    {
                        // 小数点以下出力は、5桁以上
                        num4 -= (num3 = Div100(num4)) * 100;
                        if (num4 > 0) // 7～8桁を評価
                        {
                            // 小数点以下出力は、7or8桁
                            rem = num4 - (div = Div10(num4)) * 10;

                            if (rem > 0) // 8桁を評価
                            {
                                // 小数点以下出力は、8桁
                                pToCurrent += 7;
                                offset += 9;
                                goto U7;
                            }
                            else
                            {
                                // 小数点以下出力は、7桁
                                pToCurrent += 7;
                                offset += 8;
                                *(pToCurrent) = (char)('0' + div);
                                pToCurrent -= 2;
                                goto U5;
                            }
                        }
                        else
                        {
                            // 小数点以下出力は、5or6桁
                            rem = num3 - (div = Div10(num3)) * 10;

                            if (rem > 0) // 6桁を評価
                            {
                                // 小数点以下出力は、6桁
                                pToCurrent += 5;
                                offset += 7;
                                goto U5;
                            }
                            else
                            {
                                // 小数点以下出力は、5桁
                                pToCurrent += 5;
                                offset += 6;
                                *(pToCurrent) = (char)('0' + div);
                                pToCurrent -= 2;
                                goto U3;
                            }
                        }
                    }
                    else
                    {
                        // 小数点以下出力は、4桁以下
                        if (num2 > 0) // 3～4桁を評価
                        {
                            // 小数点以下出力は、3or4桁
                            rem = num2 - (div = Div10(num2)) * 10;

                            if (rem > 0) // 4桁を評価
                            {
                                // 小数点以下出力は、4桁
                                pToCurrent += 3;
                                offset += 5;
                                goto U3;
                            }
                            else
                            {
                                // 小数点以下出力は、3桁
                                pToCurrent += 3;
                                offset += 4;
                                *pToCurrent = (char)('0' + div);
                                pToCurrent -= 2;
                                goto U1;
                            }
                        }
                        else
                        {
                            // 小数点以下出力は、1or2桁
                            rem = num1 - (div = Div10(num1)) * 10;

                            if (rem > 0) // 2桁を評価
                            {
                                // 小数点以下出力は、2桁
                                pToCurrent += 1;
                                offset += 3;
                                goto U1;
                            }
                            else
                            {
                                // 小数点以下出力は、1桁
                                pToCurrent += 1;
                                offset += 2;
                                *pToCurrent = (char)('0' + div);
                                pToCurrent -= 1;
                                goto U0;
                            }
                        }
                    }

                U7: // 小数点以下 7～8桁を出力
                    *(int*)(pToCurrent) = *(int*)(pNumTable + num4 * 2);
                    pToCurrent -= 2;
                U5: // 小数点以下 5～6桁を出力
                    *(int*)(pToCurrent) = *(int*)(pNumTable + num3 * 2);
                    pToCurrent -= 2;
                U3: // 小数点以下 3～4桁を出力
                    *(int*)(pToCurrent) = *(int*)(pNumTable + num2 * 2);
                    pToCurrent -= 2;
                U1: // 小数点以下 1～2桁を出力
                    *(int*)(pToCurrent) = *(int*)(pNumTable + num1 * 2);
                    pToCurrent--;
                U0:
                    *pToCurrent = (char)'.';
                }
            }
        }

        return offset;
    }

    public void WriteChars(IBufferWriter<char> writer)
    {
        var buffer = writer.GetSpan(21);

        int charsWritten = WriteChars(ref buffer);
        writer.Advance(charsWritten);
    }

    public int WriteUtf8(ref Span<byte> destination)
    {
        int offset = 0;

        var value = InnerValue;

        uint num1, num2, num3, div;
        ulong valueA, valueB;

        unsafe
        {
            fixed (byte* pNumTable = _tableUtf8)
            fixed (byte* pTo = destination)
            {
                var pToCurrent = pTo;

                if (value < 0)
                {
                    if (InnerValue == long.MinValue)
                    {
                        ReadOnlySpan<byte> minValue = "-92233720368.54775808"u8;

                        minValue.CopyTo(destination);
                        return minValue.Length;
                    }

                    *pToCurrent = (byte)'-';
                    pToCurrent++;
                    valueB = (ulong)(unchecked(-value));
                }
                else
                {
                    valueB = (ulong)value;
                }

                valueA = valueB / InnerPower;
                uint underPoint = (uint)(valueB - (valueA * InnerPower));


                if (valueA < 10000)
                {
                    num1 = (uint)valueA;
                    if (num1 < 10)
                    {
                        *(pToCurrent++) = (byte)('0' + num1);
                        goto L0;
                    }
                    if (num1 < 100) { goto L2; }
                    if (num1 < 1000)
                    {
                        *(pToCurrent++) = (byte)('0' + (div = Div100(num1)));
                        num1 -= div * 100;
                        goto L2;
                    }
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
                            if (num2 < 10)
                            {
                                *(pToCurrent++) = (byte)('0' + num2);
                                goto L4;
                            }
                            goto L6;
                        }
                        if (num2 < 1000)
                        {
                            *(pToCurrent++) = (byte)('0' + (div = Div100(num2)));
                            num2 -= div * 100;
                            goto L6;
                        }
                        goto L8;
                    }
                    else
                    {
                        valueA = valueB / 10000;
                        num2 = (uint)(valueB - valueA * 10000);

                        num3 = (uint)valueA;
                        if (num3 < 100)
                        {
                            if (num3 < 10)
                            {
                                *(pToCurrent++) = (byte)('0' + num3);
                                goto L8;
                            }
                            goto L10;
                        }
                        if (num3 < 1000)
                        {
                            *(pToCurrent++) = (byte)('0' + (div = Div100(num3)));
                            num3 -= div * 100;
                            goto L10;
                        }
                        goto L12;
                    L12:
                        *(short*)(pToCurrent) = *(short*)(pNumTable + (div = Div100(num3)) * 2);
                        pToCurrent += 2;
                        num3 -= div * 100;
                    L10:
                        *(short*)(pToCurrent) = *(short*)(pNumTable + num3 * 2);
                        pToCurrent += 2;
                    }
                L8:
                    *(short*)(pToCurrent) = *(short*)(pNumTable + (div = Div100(num2)) * 2);
                    pToCurrent += 2;
                    num2 -= div * 100;
                L6:
                    *(short*)(pToCurrent) = *(short*)(pNumTable + num2 * 2);
                    pToCurrent += 2;
                }
            L4:
                // num1は0～9999
                // num1を2桁ずつ、div , num1に分解
                *(short*)(pToCurrent) = *(short*)(pNumTable + (div = Div100(num1)) * 2);
                pToCurrent += 2;
                num1 -= div * 100;
            L2:
                *(short*)(pToCurrent) = *(short*)(pNumTable + num1 * 2);
                pToCurrent += 2;
            L0:
                offset = (int)(pToCurrent - pTo);

                // 小数点以下を最終的に、2桁ずつに分解する。
                // num1:1～2桁 num2:3～4桁 num3:5～6桁 num4:7～8桁
                // 最後に2桁を分解 例、 34→div=3,rem=4
                uint num4;
                uint rem;

                if (underPoint > 0)
                {
                    num4 = underPoint;

                    num4 -= (num2 = num4 / 10000) * 10000;
                    num2 -= (num1 = Div100(num2)) * 100;

                    if (num4 > 0) // 5～8桁を評価
                    {
                        // 小数点以下出力は、5桁以上
                        num4 -= (num3 = Div100(num4)) * 100;
                        if (num4 > 0) // 7～8桁を評価
                        {
                            // 小数点以下出力は、7or8桁
                            rem = num4 - (div = Div10(num4)) * 10;

                            if (rem > 0) // 8桁を評価
                            {
                                // 小数点以下出力は、8桁
                                pToCurrent += 7;
                                offset += 9;
                                goto U7;
                            }
                            else
                            {
                                // 小数点以下出力は、7桁
                                pToCurrent += 7;
                                offset += 8;
                                *(pToCurrent) = (byte)('0' + div);
                                pToCurrent -= 2;
                                goto U5;
                            }
                        }
                        else
                        {
                            // 小数点以下出力は、5or6桁
                            rem = num3 - (div = Div10(num3)) * 10;

                            if (rem > 0) // 6桁を評価
                            {
                                // 小数点以下出力は、6桁
                                pToCurrent += 5;
                                offset += 7;
                                goto U5;
                            }
                            else
                            {
                                // 小数点以下出力は、5桁
                                pToCurrent += 5;
                                offset += 6;
                                *(pToCurrent) = (byte)('0' + div);
                                pToCurrent -= 2;
                                goto U3;
                            }
                        }
                    }
                    else
                    {
                        // 小数点以下出力は、4桁以下
                        if (num2 > 0) // 3～4桁を評価
                        {
                            // 小数点以下出力は、3or4桁
                            rem = num2 - (div = Div10(num2)) * 10;

                            if (rem > 0) // 4桁を評価
                            {
                                // 小数点以下出力は、4桁
                                pToCurrent += 3;
                                offset += 5;
                                goto U3;
                            }
                            else
                            {
                                // 小数点以下出力は、3桁
                                pToCurrent += 3;
                                offset += 4;
                                *pToCurrent = (byte)('0' + div);
                                pToCurrent -= 2;
                                goto U1;
                            }
                        }
                        else
                        {
                            // 小数点以下出力は、1or2桁
                            rem = num1 - (div = Div10(num1)) * 10;

                            if (rem > 0) // 2桁を評価
                            {
                                // 小数点以下出力は、2桁
                                pToCurrent += 1;
                                offset += 3;
                                goto U1;
                            }
                            else
                            {
                                // 小数点以下出力は、1桁
                                pToCurrent += 1;
                                offset += 2;
                                *pToCurrent = (byte)('0' + div);
                                pToCurrent -= 1;
                                goto U0;
                            }
                        }
                    }

                U7: // 小数点以下 7～8桁を出力
                    *(short*)(pToCurrent) = *(short*)(pNumTable + num4 * 2);
                    pToCurrent -= 2;
                U5: // 小数点以下 5～6桁を出力
                    *(short*)(pToCurrent) = *(short*)(pNumTable + num3 * 2);
                    pToCurrent -= 2;
                U3: // 小数点以下 3～4桁を出力
                    *(short*)(pToCurrent) = *(short*)(pNumTable + num2 * 2);
                    pToCurrent -= 2;
                U1: // 小数点以下 1～2桁を出力
                    *(short*)(pToCurrent) = *(short*)(pNumTable + num1 * 2);
                    pToCurrent--;
                U0:
                    *pToCurrent = (byte)'.';
                }
            }
        }

        return offset;
    }

    public void WriteUtf8(IBufferWriter<byte> writer)
    {
        var buffer = writer.GetSpan(21);

        int charsWritten = WriteUtf8(ref buffer);
        writer.Advance(charsWritten);
    }

    public bool TryWriteChars(Span<char> destination, out int charsWritten)
    {
        int offset = 0;
        charsWritten = 0;

        var value = InnerValue;

        uint num1, num2, num3, div;
        ulong valueA, valueB;

        unsafe
        {
            fixed (char* pNumTable = _tableChar)
            fixed (char* pTo = destination)
             {
                var pToCurrent = pTo;

                if (value < 0)
                {
                    if (InnerValue == long.MinValue)
                    {
                        string minValue = "-92233720368.54775808";
                        return minValue.TryCopyTo(destination);
                    }

                    if (destination.Length < 2) { return false; }
                    *pToCurrent = '-';
                    pToCurrent++;
                    offset++;
                    valueB = (ulong)(unchecked(-value));
                }
                else
                {
                    valueB = (ulong)value;
                }

                valueA = valueB / InnerPower;
                uint underPoint = (uint)(valueB - (valueA * InnerPower));


                if (valueA < 10000)
                { 
                    num1 = (uint)valueA;
                    if (num1 < 10)
                    {
                        if (destination.Length < offset + 1) { return false; }
                        *(pToCurrent++) = (char)('0' + num1);
                        goto L0;
                    }
                    if (num1 < 100) 
                    {
                        if (destination.Length < offset + 2) { return false; }
                        goto L2;
                    }
                    if (num1 < 1000)
                    {
                        if (destination.Length < offset + 3) { return false; }
                        *(pToCurrent++) = (char)('0' + (div = Div100(num1)));
                        num1 -= div * 100;
                        goto L2;
                    }
                    if (destination.Length < offset + 4) { return false; }
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
                            if (num2 < 10)
                            {
                                if (destination.Length < offset + 5) { return false; }
                                *(pToCurrent++) = (char)('0' + num2);
                                goto L4;
                            }
                            if (destination.Length < offset + 6) { return false; }
                            goto L6;
                        }
                        if (num2 < 1000)
                        {
                            if (destination.Length < offset + 7) { return false; }
                            *(pToCurrent++) = (char)('0' + (div = Div100(num2)));
                            num2 -= div * 100;
                            goto L6;
                        }
                        if (destination.Length < offset + 8) { return false; }
                        goto L8;
                    }
                    else
                    {
                        valueA = valueB / 10000;
                        num2 = (uint)(valueB - valueA * 10000);

                        num3 = (uint)valueA;
                        if (num3 < 100)
                        {
                            if (num3 < 10)
                            {
                                if (destination.Length < offset + 9) { return false; }
                                *(pToCurrent++) = (char)('0' + num3);
                                goto L8;
                            }
                            if (destination.Length < offset + 10) { return false; }
                            goto L10;
                        }
                        if (num3 < 1000)
                        {
                            if (destination.Length < offset + 11) { return false; }
                            *(pToCurrent++) = (char)('0' + (div = Div100(num3)));
                            num3 -= div * 100;
                            goto L10;
                        }
                        if (destination.Length < offset + 12) { return false; }
                        goto L12;
                    L12:
                        *(int*)(pToCurrent) = *(int*)(pNumTable + (div = Div100(num3)) * 2);
                        pToCurrent += 2;
                        num3 -= div * 100;
                    L10:
                        *(int*)(pToCurrent) = *(int*)(pNumTable + num3 * 2);
                        pToCurrent += 2;
                    }
                L8:
                    *(int*)(pToCurrent) = *(int*)(pNumTable + (div = Div100(num2)) * 2);
                    pToCurrent += 2;
                    num2 -= div * 100;
                L6:
                    *(int*)(pToCurrent) = *(int*)(pNumTable + num2 * 2);
                    pToCurrent += 2;
                }
            L4:
                // num1は0～9999
                // num1を2桁ずつ、div , num1に分解
                *(int*)(pToCurrent) = *(int*)(pNumTable + (div = Div100(num1)) * 2);
                pToCurrent += 2;
                num1 -= div * 100;
            L2:
                *(int*)(pToCurrent) = *(int*)(pNumTable + num1 * 2);
                pToCurrent += 2;
            L0:
                offset = (int)(pToCurrent - pTo);

                // 小数点以下を最終的に、2桁ずつに分解する。
                // num1:1～2桁 num2:3～4桁 num3:5～6桁 num4:7～8桁
                // 最後に2桁を分解 例、 34→div=3,rem=4
                uint num4;
                uint rem;

                if (underPoint > 0)
                {
                    num4 = underPoint;

                    num4 -= (num2 = num4 / 10000) * 10000;
                    num2 -= (num1 = Div100(num2)) * 100;

                    if (num4 > 0) // 5～8桁を評価
                    {
                        // 小数点以下出力は、5桁以上
                        num4 -= (num3 = Div100(num4)) * 100;
                        if (num4 > 0) // 7～8桁を評価
                        {
                            // 小数点以下出力は、7or8桁
                            rem = num4 - (div = Div10(num4)) * 10;

                            if (rem > 0) // 8桁を評価
                            {
                                // 小数点以下出力は、8桁
                                pToCurrent += 7;
                                offset += 9;
                                if (destination.Length < offset) { return false; }
                                goto U7;
                            }
                            else
                            {
                                // 小数点以下出力は、7桁
                                pToCurrent += 7;
                                offset += 8;
                                if (destination.Length < offset) { return false; }
                                *(pToCurrent) = (char)('0' + div);
                                pToCurrent -= 2;
                                goto U5;
                            }
                        }
                        else
                        {
                            // 小数点以下出力は、5or6桁
                            rem = num3 - (div = Div10(num3)) * 10;

                            if (rem > 0) // 6桁を評価
                            {
                                // 小数点以下出力は、6桁
                                pToCurrent += 5;
                                offset += 7;
                                if (destination.Length < offset) { return false; }
                                goto U5;
                            }
                            else
                            {
                                // 小数点以下出力は、5桁
                                pToCurrent += 5;
                                offset += 6;
                                if (destination.Length < offset) { return false; }
                                *(pToCurrent) = (char)('0' + div);
                                pToCurrent -= 2;
                                goto U3;
                            }
                        }
                    }
                    else
                    {
                        // 小数点以下出力は、4桁以下
                        if (num2 > 0) // 3～4桁を評価
                        {
                            // 小数点以下出力は、3or4桁
                            rem = num2 - (div = Div10(num2)) * 10;

                            if (rem > 0) // 4桁を評価
                            {
                                // 小数点以下出力は、4桁
                                pToCurrent += 3;
                                offset += 5;
                                if (destination.Length < offset) { return false; }
                                goto U3;
                            }
                            else
                            {
                                // 小数点以下出力は、3桁
                                pToCurrent += 3;
                                offset += 4;
                                if (destination.Length < offset) { return false; }
                                *pToCurrent = (char)('0' + div);
                                pToCurrent -= 2;
                                goto U1;
                            }
                        }
                        else
                        {
                            // 小数点以下出力は、1or2桁
                            rem = num1 - (div = Div10(num1)) * 10;

                            if (rem > 0) // 2桁を評価
                            {
                                // 小数点以下出力は、2桁
                                pToCurrent += 1;
                                offset += 3;
                                if (destination.Length < offset) { return false; }
                                goto U1;
                            }
                            else
                            {
                                // 小数点以下出力は、1桁
                                pToCurrent += 1;
                                offset += 2;
                                if (destination.Length < offset) { return false; }
                                *pToCurrent = (char)('0' + div);
                                pToCurrent -= 1;
                                goto U0;
                            }
                        }
                    }

                U7: // 小数点以下 7～8桁を出力
                    *(int*)(pToCurrent) = *(int*)(pNumTable + num4 * 2);
                    pToCurrent -= 2;
                U5: // 小数点以下 5～6桁を出力
                    *(int*)(pToCurrent) = *(int*)(pNumTable + num3 * 2);
                    pToCurrent -= 2;
                U3: // 小数点以下 3～4桁を出力
                    *(int*)(pToCurrent) = *(int*)(pNumTable + num2 * 2);
                    pToCurrent -= 2;
                U1: // 小数点以下 1～2桁を出力
                    *(int*)(pToCurrent) = *(int*)(pNumTable + num1 * 2);
                    pToCurrent--;
                U0:
                    *pToCurrent = '.';
                }
            }
        }
        charsWritten = offset;
        return true;
    }

    public bool TryWriteUtf8(Span<byte> destination, out int charsWritten)
    {
        int offset = 0;
        charsWritten = 0;

        var value = InnerValue;

        uint num1, num2, num3, div;
        ulong valueA, valueB;

        unsafe
        {
            fixed (byte* pNumTable = _tableUtf8)
            fixed (byte* pTo = destination)
            {
                var pToCurrent = pTo;

                if (value < 0)
                {
                    if (InnerValue == long.MinValue)
                    {
                        ReadOnlySpan<byte> minValue = "-92233720368.54775808"u8;
                        return minValue.TryCopyTo(destination);
                    }

                    if (destination.Length < 2) { return false; }
                    *pToCurrent = (byte)'-';
                    pToCurrent++;
                    offset++;
                    valueB = (ulong)(unchecked(-value));
                }
                else
                {
                    valueB = (ulong)value;
                }

                valueA = valueB / InnerPower;
                uint underPoint = (uint)(valueB - (valueA * InnerPower));


                if (valueA < 10000)
                {
                    num1 = (uint)valueA;
                    if (num1 < 10)
                    {
                        if (destination.Length < offset + 1) { return false; }
                        *(pToCurrent++) = (byte)('0' + num1);
                        goto L0;
                    }
                    if (num1 < 100)
                    {
                        if (destination.Length < offset + 2) { return false; }
                        goto L2;
                    }
                    if (num1 < 1000)
                    {
                        if (destination.Length < offset + 3) { return false; }
                        *(pToCurrent++) = (byte)('0' + (div = Div100(num1)));
                        num1 -= div * 100;
                        goto L2;
                    }
                    if (destination.Length < offset + 4) { return false; }
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
                            if (num2 < 10)
                            {
                                if (destination.Length < offset + 5) { return false; }
                                *(pToCurrent++) = (byte)('0' + num2);
                                goto L4;
                            }
                            if (destination.Length < offset + 6) { return false; }
                            goto L6;
                        }
                        if (num2 < 1000)
                        {
                            if (destination.Length < offset + 7) { return false; }
                            *(pToCurrent++) = (byte)('0' + (div = Div100(num2)));
                            num2 -= div * 100;
                            goto L6;
                        }
                        if (destination.Length < offset + 8) { return false; }
                        goto L8;
                    }
                    else
                    {
                        valueA = valueB / 10000;
                        num2 = (uint)(valueB - valueA * 10000);

                        num3 = (uint)valueA;
                        if (num3 < 100)
                        {
                            if (num3 < 10)
                            {
                                if (destination.Length < offset + 9) { return false; }
                                *(pToCurrent++) = (byte)('0' + num3);
                                goto L8;
                            }
                            if (destination.Length < offset + 10) { return false; }
                            goto L10;
                        }
                        if (num3 < 1000)
                        {
                            if (destination.Length < offset + 11) { return false; }
                            *(pToCurrent++) = (byte)('0' + (div = Div100(num3)));
                            num3 -= div * 100;
                            goto L10;
                        }
                        if (destination.Length < offset + 12) { return false; }
                        goto L12;
                    L12:
                        *(short*)(pToCurrent) = *(short*)(pNumTable + (div = Div100(num3)) * 2);
                        pToCurrent += 2;
                        num3 -= div * 100;
                    L10:
                        *(short*)(pToCurrent) = *(short*)(pNumTable + num3 * 2);
                        pToCurrent += 2;
                    }
                L8:
                    *(short*)(pToCurrent) = *(short*)(pNumTable + (div = Div100(num2)) * 2);
                    pToCurrent += 2;
                    num2 -= div * 100;
                L6:
                    *(short*)(pToCurrent) = *(short*)(pNumTable + num2 * 2);
                    pToCurrent += 2;
                }
            L4:
                // num1は0～9999
                // num1を2桁ずつ、div , num1に分解
                *(short*)(pToCurrent) = *(short*)(pNumTable + (div = Div100(num1)) * 2);
                pToCurrent += 2;
                num1 -= div * 100;
            L2:
                *(short*)(pToCurrent) = *(short*)(pNumTable + num1 * 2);
                pToCurrent += 2;
            L0:
                offset = (int)(pToCurrent - pTo);

                // 小数点以下を最終的に、2桁ずつに分解する。
                // num1:1～2桁 num2:3～4桁 num3:5～6桁 num4:7～8桁
                // 最後に2桁を分解 例、 34→div=3,rem=4
                uint num4;
                uint rem;

                if (underPoint > 0)
                {
                    num4 = underPoint;

                    num4 -= (num2 = num4 / 10000) * 10000;
                    num2 -= (num1 = Div100(num2)) * 100;

                    if (num4 > 0) // 5～8桁を評価
                    {
                        // 小数点以下出力は、5桁以上
                        num4 -= (num3 = Div100(num4)) * 100;
                        if (num4 > 0) // 7～8桁を評価
                        {
                            // 小数点以下出力は、7or8桁
                            rem = num4 - (div = Div10(num4)) * 10;

                            if (rem > 0) // 8桁を評価
                            {
                                // 小数点以下出力は、8桁
                                pToCurrent += 7;
                                offset += 9;
                                if (destination.Length < offset) { return false; }
                                goto U7;
                            }
                            else
                            {
                                // 小数点以下出力は、7桁
                                pToCurrent += 7;
                                offset += 8;
                                if (destination.Length < offset) { return false; }
                                *(pToCurrent) = (byte)('0' + div);
                                pToCurrent -= 2;
                                goto U5;
                            }
                        }
                        else
                        {
                            // 小数点以下出力は、5or6桁
                            rem = num3 - (div = Div10(num3)) * 10;

                            if (rem > 0) // 6桁を評価
                            {
                                // 小数点以下出力は、6桁
                                pToCurrent += 5;
                                offset += 7;
                                if (destination.Length < offset) { return false; }
                                goto U5;
                            }
                            else
                            {
                                // 小数点以下出力は、5桁
                                pToCurrent += 5;
                                offset += 6;
                                if (destination.Length < offset) { return false; }
                                *(pToCurrent) = (byte)('0' + div);
                                pToCurrent -= 2;
                                goto U3;
                            }
                        }
                    }
                    else
                    {
                        // 小数点以下出力は、4桁以下
                        if (num2 > 0) // 3～4桁を評価
                        {
                            // 小数点以下出力は、3or4桁
                            rem = num2 - (div = Div10(num2)) * 10;

                            if (rem > 0) // 4桁を評価
                            {
                                // 小数点以下出力は、4桁
                                pToCurrent += 3;
                                offset += 5;
                                if (destination.Length < offset) { return false; }
                                goto U3;
                            }
                            else
                            {
                                // 小数点以下出力は、3桁
                                pToCurrent += 3;
                                offset += 4;
                                if (destination.Length < offset) { return false; }
                                *pToCurrent = (byte)('0' + div);
                                pToCurrent -= 2;
                                goto U1;
                            }
                        }
                        else
                        {
                            // 小数点以下出力は、1or2桁
                            rem = num1 - (div = Div10(num1)) * 10;

                            if (rem > 0) // 2桁を評価
                            {
                                // 小数点以下出力は、2桁
                                pToCurrent += 1;
                                offset += 3;
                                if (destination.Length < offset) { return false; }
                                goto U1;
                            }
                            else
                            {
                                // 小数点以下出力は、1桁
                                pToCurrent += 1;
                                offset += 2;
                                if (destination.Length < offset) { return false; }
                                *pToCurrent = (byte)('0' + div);
                                pToCurrent -= 1;
                                goto U0;
                            }
                        }
                    }

                U7: // 小数点以下 7～8桁を出力
                    *(short*)(pToCurrent) = *(short*)(pNumTable + num4 * 2);
                    pToCurrent -= 2;
                U5: // 小数点以下 5～6桁を出力
                    *(short*)(pToCurrent) = *(short*)(pNumTable + num3 * 2);
                    pToCurrent -= 2;
                U3: // 小数点以下 3～4桁を出力
                    *(short*)(pToCurrent) = *(short*)(pNumTable + num2 * 2);
                    pToCurrent -= 2;
                U1: // 小数点以下 1～2桁を出力
                    *(short*)(pToCurrent) = *(short*)(pNumTable + num1 * 2);
                    pToCurrent--;
                U0:
                    *pToCurrent = (byte)'.';
                }
            }
        }
        charsWritten = offset;
        return true;
    }

    // ****************************************
    // FixedPoint8への変換(cast)
    // ****************************************

    public static explicit operator FixedPoint8(sbyte value)
    {
        return new FixedPoint8((long)value * (long)InnerPower);
    }

    public static explicit operator FixedPoint8(byte value)
    {
        return new FixedPoint8((long)value * (long)InnerPower);
    }

    public static explicit operator FixedPoint8(short value)
    {
        return new FixedPoint8((long)value * (long)InnerPower);
    }

    public static explicit operator FixedPoint8(ushort value)
    {
        return new FixedPoint8((long)value * (long)InnerPower);
    }

    public static explicit operator FixedPoint8(int value)
    {
        return new FixedPoint8((long)value * (long)InnerPower);
    }

    public static explicit operator FixedPoint8(uint value)
    {
        return new FixedPoint8((long)value * (long)InnerPower);
    }

    public static explicit operator FixedPoint8(long value)
    {
        return new FixedPoint8(value * (long)InnerPower);
    }

    public static explicit operator FixedPoint8(ulong value)
    {
        return new FixedPoint8((long)value * (long)InnerPower);
    }

    public static explicit operator FixedPoint8(float value)
    {
        return new FixedPoint8((long)(value * InnerPower));
    }

    public static explicit operator FixedPoint8(double value)
    {
        return FromDouble(value);
    }

    public static explicit operator FixedPoint8(decimal value)
    {
        return FromDecimal(value);
    }

    // ****************************************
    // FixedPoint8からの変換(cast)
    // ****************************************

    public static explicit operator sbyte(FixedPoint8 value)
    {
        return (sbyte)(value.InnerValue / InnerPower);
    }

    public static explicit operator byte(FixedPoint8 value)
    {
        return (byte)(value.InnerValue / InnerPower);
    }

    public static explicit operator short(FixedPoint8 value)
    {
        return (short)(value.InnerValue / InnerPower);
    }

    public static explicit operator ushort(FixedPoint8 value)
    {
        return (ushort)(value.InnerValue / InnerPower);
    }

    public static explicit operator int(FixedPoint8 value)
    {
        return (int)(value.InnerValue / InnerPower);
    }

    public static explicit operator uint(FixedPoint8 value)
    {
        return (uint)(value.InnerValue / InnerPower);
    }

    public static explicit operator long(FixedPoint8 value)
    {
        return value.InnerValue / InnerPower;
    }

    public static explicit operator ulong(FixedPoint8 value)
    {
        return (ulong)(value.InnerValue / InnerPower);
    }

    public static explicit operator float(FixedPoint8 value)
    {
        return ((float)(value.InnerValue)) / InnerPower;
    }

    public static explicit operator double(FixedPoint8 value)
    {
        return ((double)(value.InnerValue)) / InnerPower;
    }

    public static explicit operator decimal(FixedPoint8 value)
    {
        return ((decimal)(value.InnerValue)) / InnerPower;
    }

    // ****************************************
    // operator
    // ****************************************

    public static FixedPoint8 operator +(FixedPoint8 left, FixedPoint8 right)
    {
        return new FixedPoint8(left.InnerValue + right.InnerValue);
    }

    public static FixedPoint8 operator -(FixedPoint8 left, FixedPoint8 right)
    {
        return new FixedPoint8(left.InnerValue - right.InnerValue);
    }


    public static FixedPoint8 operator *(FixedPoint8 left, long right)
    {
        return new FixedPoint8(left.InnerValue * right);
    }

    public static FixedPoint8 operator *(FixedPoint8 left, ulong right)
    {
        return new FixedPoint8(left.InnerValue * (long)right);
    }

    //速度が出ないので使用は推奨しない
    public static FixedPoint8 operator *(FixedPoint8 left, FixedPoint8 right)
    {
        var decimalLeft = (decimal)(left.InnerValue) / InnerPower;
        var decimalRight = (decimal)(right.InnerValue) / InnerPower;
        return FixedPoint8.FromDecimal(decimalLeft * decimalRight);
    }

    public static FixedPoint8 operator /(FixedPoint8 left, long right)
    {
        return new FixedPoint8(left.InnerValue / right);
    }

    public static FixedPoint8 operator /(FixedPoint8 left, ulong right)
    {
        return new FixedPoint8(left.InnerValue / (long)right);
    }

    //速度が出ないので使用は推奨しない
    public static FixedPoint8 operator /(FixedPoint8 left, FixedPoint8 right)
    {
        var decimalLeft = (decimal)(left.InnerValue) / InnerPower;
        var decimalRight = (decimal)(right.InnerValue) / InnerPower;
        return FixedPoint8.FromDecimal(decimalLeft / decimalRight);
    }

    public static bool operator ==(FixedPoint8 left, FixedPoint8 right)
    {
        return left.InnerValue == right.InnerValue;
    }

    public static bool operator !=(FixedPoint8 left, FixedPoint8 right)
    {
        return left.InnerValue != right.InnerValue;
    }

    public static bool operator <(FixedPoint8 left, FixedPoint8 right)
    {
        return left.InnerValue < right.InnerValue;
    }
    public static bool operator <=(FixedPoint8 left, FixedPoint8 right)
    {
        return left.InnerValue <= right.InnerValue;
    }

    public static bool operator >(FixedPoint8 left, FixedPoint8 right)
    {
        return left.InnerValue > right.InnerValue;
    }

    public static bool operator >=(FixedPoint8 left, FixedPoint8 right)
    {
        return left.InnerValue >= right.InnerValue;
    }

    public static FixedPoint8 operator %(FixedPoint8 left, FixedPoint8 right)
    {
        return new FixedPoint8(left.InnerValue % right.InnerValue);
    }

    public static FixedPoint8 operator ++(FixedPoint8 value)
    {
        return value + One;
    }

    public static FixedPoint8 operator --(FixedPoint8 value)
    {
        return value - One;
    }

    public static FixedPoint8 operator +(FixedPoint8 value)
    {
        return value;
    }

    public static FixedPoint8 operator -(FixedPoint8 value)
    {
        return new FixedPoint8(-value.InnerValue);
    }


    // ****************************************
    // その他
    // ****************************************

    public override bool Equals(object? obj)
    {
        return obj is FixedPoint8 fixedPoint8 &&
               InnerValue == fixedPoint8.InnerValue;
    }

    public bool Equals(FixedPoint8 value)
    {
        return value.InnerValue == InnerValue;
    }

    public override int GetHashCode()
    {
        return InnerValue.GetHashCode();
    }

    public int CompareTo(object? obj)
    {
        return obj is FixedPoint8 fixedPoint8 ? InnerValue.CompareTo(fixedPoint8.InnerValue) : throw new ArgumentException("obj is not FixedPoint8.");
    }

    public int CompareTo(FixedPoint8 other)
    {
        return InnerValue.CompareTo(other.InnerValue);
    }

    public static FixedPoint8 Abs(FixedPoint8 value)
    {
        if (value.InnerValue < 0)
        {
            return new FixedPoint8(-value.InnerValue);
        }
        return value;
    }

    public static bool IsCanonical(FixedPoint8 value)
    {
        return true;
    }

    public static bool IsComplexNumber(FixedPoint8 value)
    {
        return false;
    }

    public static bool IsEvenInteger(FixedPoint8 value)
    {
        long overPoint = value.InnerValue / 200_000_000;
        long underPoint = value.InnerValue - overPoint * 200_000_000;
        if (underPoint == 0)
        {
            return true;
        }
        return false;
    }

    public static bool IsFinite(FixedPoint8 value)
    {
        return true;
    }

    public static bool IsImaginaryNumber(FixedPoint8 value)
    {
        return false;
    }

    public static bool IsInfinity(FixedPoint8 value)
    {
        return false;
    }

    public static bool IsInteger(FixedPoint8 value)
    {
        long overPoint = value.InnerValue / InnerPower;
        long underPoint = value.InnerValue - overPoint * InnerPower;
        if (underPoint == 0)
        {
            return true;
        }
        return false;
    }

    public static bool IsNaN(FixedPoint8 value)
    {
        return false;
    }

    public static bool IsNegative(FixedPoint8 value)
    {
        return value.InnerValue < 0;
    }

    public static bool IsNegativeInfinity(FixedPoint8 value)
    {
        return false;
    }

    public static bool IsNormal(FixedPoint8 value)
    {
        if (value == FixedPoint8.Zero)
        {
            return false;
        }
        return true;
    }

    public static bool IsOddInteger(FixedPoint8 value)
    {
        long overPoint = value.InnerValue / 200_000_000;
        long underPoint = value.InnerValue - overPoint * 200_000_000;
        if (underPoint == InnerPower || underPoint == -InnerPower)
        {
            return true;
        }
        return false;
    }

    public static bool IsPositive(FixedPoint8 value)
    {
        return (value.InnerValue >= 0);
    }

    public static bool IsPositiveInfinity(FixedPoint8 value)
    {
        return false;
    }

    public static bool IsRealNumber(FixedPoint8 value)
    {
        return true;
    }

    public static bool IsSubnormal(FixedPoint8 value)
    {
        return false;
    }

    public static bool IsZero(FixedPoint8 value)
    {
        return (value.InnerValue == 0);
    }

    public static FixedPoint8 MaxMagnitude(FixedPoint8 x, FixedPoint8 y)
    {
        long absX = x.InnerValue;

        if (absX < 0)
        {
            absX = -absX;

            if (absX < 0)
            {
                return x;
            }
        }

        long absY = y.InnerValue;

        if (absY < 0)
        {
            absY = -absY;

            if (absY < 0)
            {
                return y;
            }
        }

        if (absX > absY)
        {
            return x;
        }

        if (absX == absY)
        {
            return IsNegative(x) ? y : x;
        }

        return y;
    }

    public static FixedPoint8 MaxMagnitudeNumber(FixedPoint8 x, FixedPoint8 y)
    {
        return MaxMagnitude(x, y);
    }

    public static FixedPoint8 MinMagnitude(FixedPoint8 x, FixedPoint8 y)
    {
        long absX = x.InnerValue;

        if (absX < 0)
        {
            absX = -absX;

            if (absX < 0)
            {
                return y;
            }
        }

        long absY = y.InnerValue;

        if (absY < 0)
        {
            absY = -absY;

            if (absY < 0)
            {
                return x;
            }
        }

        if (absX < absY)
        {
            return x;
        }

        if (absX == absY)
        {
            return IsNegative(x) ? x : y;
        }

        return y;
    }

    public static FixedPoint8 MinMagnitudeNumber(FixedPoint8 x, FixedPoint8 y)
    {
        return MinMagnitude(x, y);
    }

    static bool INumberBase<FixedPoint8>.TryConvertFromChecked<TOther>(TOther value, out FixedPoint8 result)
    {
        throw new NotImplementedException();
    }

    static bool INumberBase<FixedPoint8>.TryConvertFromSaturating<TOther>(TOther value, out FixedPoint8 result)
    {
        throw new NotImplementedException();
    }

    static bool INumberBase<FixedPoint8>.TryConvertFromTruncating<TOther>(TOther value, out FixedPoint8 result)
    {
        throw new NotImplementedException();
    }

    static bool INumberBase<FixedPoint8>.TryConvertToChecked<TOther>(FixedPoint8 value, out TOther result)
    {
        throw new NotImplementedException();
    }

    static bool INumberBase<FixedPoint8>.TryConvertToSaturating<TOther>(FixedPoint8 value, out TOther result)
    {
        throw new NotImplementedException();
    }

    static bool INumberBase<FixedPoint8>.TryConvertToTruncating<TOther>(FixedPoint8 value, out TOther result)
    {
        throw new NotImplementedException();
    }

    public FixedPoint8 Round()
    {
        long absValue;
        int sign; // 1 = + ,-1 = -
        long value;

        if (InnerValue < 0)
        {
            if (InnerValue == long.MinValue)
            {
                // ((FixedPoint8)(-92233720368.1)).Round() と同じ値を返す
                return ((FixedPoint8)(-92233720368.54775807m)).Round();
            }

            absValue = -InnerValue;
            sign = -1;

            value = InnerValue - 50000000;
        }
        else
        {
            absValue = InnerValue;
            sign = 1;

            value = InnerValue + 50000000;
        }

        var absOverPoint = absValue / InnerPower;
        var absUnderPoint = absValue - absOverPoint * InnerPower;

        if (absUnderPoint == 50000000)
        {
            if ((absOverPoint & 1) == 0)
            {
                return new FixedPoint8(absOverPoint * InnerPower * sign);
            }
            else
            {
                return new FixedPoint8((1 + absOverPoint) * InnerPower * sign);
            }
        }

        var overPoint = value / InnerPower;

        return new FixedPoint8(overPoint * InnerPower);
    }

    //decimals = 2　→　小数点２桁になるように丸める
    //decimals = -2　→　100の単位になるように丸める

    //1234.56780000 , -2 → 1200.00000000
    //  12 , 345678000000000000

    //1234.56780000 , -3 → 1000.00000000
    //   1 , 234567800000000000

    //1500.00000000 , -3 → 2000.00000000
    //   1 , 500000000000000000

    static readonly string _decimalsOutOfRange = "decimalは-10～7の間で設定してください";

    public FixedPoint8 Round(int decimals)
    {
        if (decimals < -10 || decimals > 7)
        {
            throw new ArgumentOutOfRangeException(_decimalsOutOfRange);
        }

        ulong absValue;
        int sign; // 1 = + ,-1 = -

        if (InnerValue < 0)
        {
            if (InnerValue == long.MinValue)
            {
                return ((FixedPoint8)(-92233720368.54775807m)).Round(decimals);
            }

            absValue = (ulong)(-InnerValue);
            sign = -1;
        }
        else
        {
            absValue = (ulong)InnerValue;
            sign = 1;
        }

        var overDiv = powerArray[-decimals + 8];
        var absOverRound = absValue / overDiv;

        var underPower = powerArray[10 + decimals];
        var absUnderRound = (absValue - absOverRound * overDiv) * underPower;

        if (absUnderRound == 500000000000000000)
        {
            if ((absOverRound & 1) == 0)
            {
                return new FixedPoint8((long)(absOverRound * overDiv) * sign);
            }
            return new FixedPoint8((long)((absOverRound + 1) * overDiv) * sign);
        }

        var mRoundOff = 5 * overDiv / 10;
        return new FixedPoint8((long)((absValue + mRoundOff) / overDiv * overDiv) * sign);
    }

    public FixedPoint8 Floor()
    {
        long absValue;

        if (InnerValue < 0)
        {
            if (InnerValue == long.MinValue)
            {
                // ((FixedPoint8)(-92233720368.1)).Floor() と同じ値を返す
                return ((FixedPoint8)(-92233720368.54775807m)).Floor();
            }

            absValue = -InnerValue;
            var overPoint = (absValue + 9999_9999) / InnerPower;
            return new FixedPoint8(-overPoint * InnerPower);
        }
        else
        {
            absValue = InnerValue;

            var overPoint = absValue / InnerPower;

            return new FixedPoint8(overPoint * InnerPower);
        }
    }

    public FixedPoint8 Floor(int decimals)
    {
        if (decimals < -10 || decimals > 7)
        {
            throw new ArgumentOutOfRangeException(_decimalsOutOfRange);
        }

        ulong absValue;

        if (InnerValue < 0)
        {
            if (InnerValue == long.MinValue)
            {
                return ((FixedPoint8)(-92233720368.54775807m)).Floor(decimals);
            }

            absValue = (ulong)(-InnerValue);

            var overDiv = powerArray[-decimals + 8];
            var absOverRound = absValue / overDiv;

            var underPower = powerArray[10 + decimals];
            var absUnderRound = (absValue - absOverRound * overDiv) * underPower;

            if (absUnderRound == 0)
            {
                return new FixedPoint8(((long)(absOverRound * overDiv)) * -1);
            }
            return new FixedPoint8(((long)((absOverRound + 1) * overDiv)) * -1);

        }
        else
        {
            absValue = (ulong)InnerValue;

            var overDiv = powerArray[-decimals + 8];
            var absOverRound = absValue / overDiv;

            return new FixedPoint8((long)(absOverRound * overDiv));
        }
    }

    public FixedPoint8 Truncate()
    {
        long absValue;

        if (InnerValue < 0)
        {
            if (InnerValue == long.MinValue)
            {
                return new FixedPoint8(-92233720368__0000_0000);
            }

            absValue = -InnerValue;

            var overPoint = absValue / InnerPower;
            return new FixedPoint8(overPoint * InnerPower * -1);
        }
        else
        {
            absValue = InnerValue;

            var overPoint = absValue / InnerPower;
            return new FixedPoint8(overPoint * InnerPower);
        }
    }

    public FixedPoint8 Truncate(int decimals)
    {
        if (decimals < -10 || decimals > 7)
        {
            throw new ArgumentOutOfRangeException(_decimalsOutOfRange);
        }

        ulong absValue;

        if (InnerValue < 0)
        {
            if (InnerValue == long.MinValue)
            {
                return ((FixedPoint8)(-92233720368.54775807m)).Truncate(decimals);
            }

            absValue = (ulong)(-InnerValue);

            var overDiv = powerArray[-decimals + 8];
            var absOverRound = absValue / overDiv;

            return new FixedPoint8(((long)(absOverRound * overDiv)) * -1);
        }
        else
        {
            absValue = (ulong)InnerValue;

            var overDiv = powerArray[-decimals + 8];
            var absOverRound = absValue / overDiv;

            return new FixedPoint8((long)(absOverRound * overDiv));
        }
    }

    public FixedPoint8 Ceiling()
    {
        long absValue;

        if (InnerValue < 0)
        {
            if (InnerValue == long.MinValue)
            {
                return new FixedPoint8(-92233720368__0000_0000);
            }

            absValue = -InnerValue;

            var overPoint = absValue / InnerPower;

            return new FixedPoint8(-overPoint * InnerPower);
        }
        else
        {
            var overPoint = (InnerValue + 9999_9999) / InnerPower;
            return new FixedPoint8(overPoint * InnerPower);
        }
    }


    public FixedPoint8 Ceiling(int decimals)
    {
        ulong absValue;

        if (InnerValue < 0)
        {
            if (InnerValue == long.MinValue)
            {
                return ((FixedPoint8)(-92233720368.54775807m)).Ceiling(decimals);
            }

            absValue = (ulong)(-InnerValue);

            var overDiv = powerArray[-decimals + 8];
            var absOverRound = absValue / overDiv;

            return new FixedPoint8(((long)(absOverRound * overDiv)) * -1);
        }
        else
        {
            absValue = (ulong)InnerValue;

            var overDiv = powerArray[-decimals + 8];
            var absOverRound = absValue / overDiv;

            var underPower = powerArray[10 + decimals];
            var absUnderRound = (absValue - absOverRound * overDiv) * underPower;

            if (absUnderRound == 0)
            {
                return new FixedPoint8((long)(absOverRound * overDiv));
            }
            return new FixedPoint8((long)((absOverRound + 1) * overDiv));
        }
    }

    static readonly ulong[] powerArray = new ulong[] {
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

    static uint Div1000(uint value) => value * 8389 >> 23;
    static uint Div100(uint value) => value * 5243 >> 19;
    static uint Div10(uint value) => value * 6554 >> 16;
}
