using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Gitan.FixedPoint8;

public readonly struct FixedPoint8 : INumber<FixedPoint8>
{

    public static FixedPoint8 MaxValue { get; } = new FixedPoint8(long.MaxValue);
    public static FixedPoint8 MinValue { get; } = new FixedPoint8(long.MinValue);

    public const int InnerPower = 100_000_000;

    readonly long _innerValue = 0;
    public long InnerValue => _innerValue;

    public static FixedPoint8 Zero { get; } = new FixedPoint8(0);

    public static FixedPoint8 One { get; } = new FixedPoint8(InnerPower);

    public static int Radix => 10;

    public static FixedPoint8 AdditiveIdentity => Zero;

    public static FixedPoint8 MultiplicativeIdentity => One;

    public FixedPoint8(long innerValue)
    {
        _innerValue = innerValue;
    }

    // ****************************************
    // FixedPoint8への変換
    // ****************************************

    public static FixedPoint8 FromInnerValue(long value)
    {
        return new FixedPoint8(value);
    }

    public static FixedPoint8 FromDouble(double value)
    {
        return new FixedPoint8((long)(value * InnerPower + (value > 0 ? 0.5 : -0.5)));
    }

    public static FixedPoint8 FromDecimal(decimal value)
    {
        return new FixedPoint8((long)(value * InnerPower + (value > 0m ? 0.5m : -0.5m)));
    }

    public static void ThrowException(string value)
    {
        throw new FormatException(value);
    }

    public static FixedPoint8 Parse(string s)
    {       
        if(TryParse(s.AsSpan(), out var result))
        {
            return result;
        }
        ThrowException("正しい形式ではありません");
        return Zero;
    }

    public static FixedPoint8 Parse(ReadOnlySpan<char> s)
    {
        if (TryParse(s, out var result))
        {
            return result;
        }
        ThrowException("正しい形式ではありません");
        return Zero;
    }

    public static FixedPoint8 Parse(ReadOnlySpan<byte> utf8)
    {
        if (TryParse(utf8, out var result))
        {
            return result;
        }
        throw new FormatException("正しい形式ではありません");
    } 

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

    // ****************************************
    // FixedPoint8からの変換
    // ****************************************

    public override string ToString()
    {
        Span<char> buffer = stackalloc char[21];

        int charsWritten = WriteChars(ref buffer);
        return new string(buffer[..charsWritten]);
    }

    public byte[] ToUtf8()
    {
        Span<byte> buffer = stackalloc byte[21];

        int charsWritten = WriteUtf8(ref buffer);
        return buffer[..charsWritten].ToArray();
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
        return (sbyte)(value._innerValue / InnerPower);
    }

    public static explicit operator byte(FixedPoint8 value)
    {
        return (byte)(value._innerValue / InnerPower);
    }

    public static explicit operator short(FixedPoint8 value)
    {
        return (short)(value._innerValue / InnerPower);
    }

    public static explicit operator ushort(FixedPoint8 value)
    {
        return (ushort)(value._innerValue / InnerPower);
    }

    public static explicit operator int(FixedPoint8 value)
    {
        return (int)(value._innerValue / InnerPower);
    }

    public static explicit operator uint(FixedPoint8 value)
    {
        return (uint)(value._innerValue / InnerPower);
    }

    public static explicit operator long(FixedPoint8 value)
    {
        return value._innerValue / InnerPower;
    }

    public static explicit operator ulong(FixedPoint8 value)
    {
        return (ulong)(value._innerValue / InnerPower);
    }

    public static explicit operator float(FixedPoint8 value)
    {
        return ((float)(value._innerValue)) / InnerPower;
    }

    public static explicit operator double(FixedPoint8 value)
    {
        return ((double)(value._innerValue)) / InnerPower;
    }

    public static explicit operator decimal(FixedPoint8 value)
    {
        return ((decimal)(value._innerValue)) / InnerPower;
    }

    // ****************************************
    // operator
    // ****************************************

    public static FixedPoint8 operator +(FixedPoint8 left, FixedPoint8 right)
    {
        return new FixedPoint8(left._innerValue + right._innerValue);
    }

    public static FixedPoint8 operator -(FixedPoint8 left, FixedPoint8 right)
    {
        return new FixedPoint8(left._innerValue - right._innerValue);
    }


    public static FixedPoint8 operator *(FixedPoint8 left, long right)
    {
        return new FixedPoint8(left._innerValue * right);
    }

    public static FixedPoint8 operator *(FixedPoint8 left, ulong right)
    {
        return new FixedPoint8(left._innerValue * (long)right);
    }

    //速度が出ないので使用は推奨しない
    public static FixedPoint8 operator *(FixedPoint8 left, FixedPoint8 right)
    {
        var decimalLeft = (decimal)(left._innerValue) / InnerPower;
        var decimalRight = (decimal)(right._innerValue) / InnerPower;
        return FixedPoint8.FromDecimal(decimalLeft * decimalRight);
    }

    public static FixedPoint8 operator /(FixedPoint8 left, long right)
    {
        return new FixedPoint8(left._innerValue / right);
    }

    public static FixedPoint8 operator /(FixedPoint8 left, ulong right)
    {
        return new FixedPoint8(left._innerValue / (long)right);
    }

    //速度が出ないので使用は推奨しない
    public static FixedPoint8 operator /(FixedPoint8 left, FixedPoint8 right)
    {
        var decimalLeft = (decimal)(left._innerValue) / InnerPower;
        var decimalRight = (decimal)(right._innerValue) / InnerPower;
        return FixedPoint8.FromDecimal(decimalLeft / decimalRight);
    }

    public static bool operator ==(FixedPoint8 left, FixedPoint8 right)
    {
        return left._innerValue == right._innerValue;
    }

    public static bool operator !=(FixedPoint8 left, FixedPoint8 right)
    {
        return left._innerValue != right._innerValue;
    }

    public static bool operator <(FixedPoint8 left, FixedPoint8 right)
    {
        return left._innerValue < right._innerValue;
    }
    public static bool operator <=(FixedPoint8 left, FixedPoint8 right)
    {
        return left._innerValue <= right._innerValue;
    }

    public static bool operator >(FixedPoint8 left, FixedPoint8 right)
    {
        return left._innerValue > right._innerValue;
    }

    public static bool operator >=(FixedPoint8 left, FixedPoint8 right)
    {
        return left._innerValue >= right._innerValue;
    }

    public static FixedPoint8 operator %(FixedPoint8 left, FixedPoint8 right)
    {
        return new FixedPoint8(left._innerValue % right._innerValue);
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
               _innerValue == fixedPoint8._innerValue;
    }

    public bool Equals(FixedPoint8 value)
    {
        return value._innerValue == _innerValue;
    }

    public override int GetHashCode()
    {
        return _innerValue.GetHashCode();
    }

    public int CompareTo(object? obj)
    {
        return obj is FixedPoint8 fixedPoint8 ? _innerValue.CompareTo(fixedPoint8._innerValue) : throw new ArgumentException("obj is not FixedPoint8.");
    }

    public int CompareTo(FixedPoint8 other)
    {
        return _innerValue.CompareTo(other._innerValue);
    }

    public static FixedPoint8 Abs(FixedPoint8 value)
    {
        if (value._innerValue < 0)
        {
            return new FixedPoint8(-value._innerValue);
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
        long overPoint = value._innerValue / 200_000_000;
        long underPoint = value._innerValue - overPoint * 200_000_000;
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
        long overPoint = value._innerValue / InnerPower;
        long underPoint = value._innerValue - overPoint * InnerPower;
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
        return value._innerValue < 0;
    }

    public static bool IsNegativeInfinity(FixedPoint8 value)
    {
        return false;
    }

    public static bool IsNormal(FixedPoint8 value)
    {
        if(value == FixedPoint8.Zero)
        {
            return false;
        }
        return true;
    }

    public static bool IsOddInteger(FixedPoint8 value)
    {
        long overPoint = value._innerValue / 200_000_000;
        long underPoint = value._innerValue - overPoint * 200_000_000;
        if (underPoint == InnerPower || underPoint == -InnerPower)
        {
            return true;
        }
        return false;
    }

    public static bool IsPositive(FixedPoint8 value)
    {
        return (value._innerValue >= 0);
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
        return (value._innerValue == 0);
    }

    public static FixedPoint8 MaxMagnitude(FixedPoint8 x, FixedPoint8 y)
    {
        long absX = x._innerValue;

        if (absX < 0)
        {
            absX = -absX;

            if (absX < 0)
            {
                return x;
            }
        }

        long absY = y._innerValue;

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
        long absX = x._innerValue;

        if (absX < 0)
        {
            absX = -absX;

            if (absX < 0)
            {
                return y;
            }
        }

        long absY = y._innerValue;

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

    public static FixedPoint8 Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static FixedPoint8 Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out FixedPoint8 result)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out FixedPoint8 result)
    {       
        throw new NotImplementedException();
    }


    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (format.Length != 0)
        {
            decimal dec = (decimal)this;
            return dec.TryFormat(destination, out charsWritten, format, provider);
        }
        if(destination.Length >= 21)
        {
            charsWritten = WriteChars(ref destination);
            return true;
        }
        else
        {
            return TryWriteChars(destination, out charsWritten);
        }
    }

    public bool TryWriteChars(Span<char> destination, out int charsWritten)
    {
        int offset = 0;
        charsWritten = 0;

        var value = InnerValue;

        uint num1, num2, num3, div;
        ulong valueA, valueB;

        if (value < 0)
        {
            if (InnerValue == long.MinValue)
            {
                string minValue = "-92233720368.54775808";

                charsWritten = minValue.Length;
                return minValue.TryCopyTo(destination);
            }

            if (destination.Length <= offset) { return false; }
            destination[offset++] = (char)'-';
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
                goto L1;
            }
            if (num1 < 100)
            {
                if (destination.Length < offset + 2) { return false; }
                goto L2;
            }
            if (num1 < 1000)
            {
                if (destination.Length < offset + 3) { return false; }
                goto L3;
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
                        goto L5;
                    }
                    if (destination.Length < offset + 6) { return false; }
                    goto L6;
                }
                if (num2 < 1000)
                {
                    if (destination.Length < offset + 7) { return false; }
                    goto L7;
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
                        goto L9;
                    }
                    if (destination.Length < offset + 10) { return false; }
                    goto L10;
                }
                if (num3 < 1000)
                {
                    if (destination.Length < offset + 11) { return false; }
                    goto L11;
                }
                if (destination.Length < offset + 12) { return false; }
                goto L12;

            L12:
                destination[offset++] = (char)('0' + (div = Div1000(num3)));
                num3 -= div * 1000;
            L11:
                destination[offset++] = (char)('0' + (div = Div100(num3)));
                num3 -= div * 100;
            L10:
                destination[offset++] = (char)('0' + (div = Div10(num3)));
                num3 -= div * 10;
            L9:
                destination[offset++] = (char)('0' + num3);

            }
        L8:
            destination[offset++] = (char)('0' + (div = Div1000(num2)));
            num2 -= div * 1000;
        L7:
            destination[offset++] = (char)('0' + (div = Div100(num2)));
            num2 -= div * 100;
        L6:
            destination[offset++] = (char)('0' + (div = Div10(num2)));
            num2 -= div * 10;
        L5:
            destination[offset++] = (char)('0' + num2);

        }

    L4:
        destination[offset++] = (char)('0' + (div = Div1000(num1)));
        num1 -= div * 1000;
    L3:
        destination[offset++] = (char)('0' + (div = Div100(num1)));
        num1 -= div * 100;
    L2:
        destination[offset++] = (char)('0' + (div = Div10(num1)));
        num1 -= div * 10;
    L1:
        destination[offset++] = (char)('0' + num1);


        uint num4, num5, num6, num7, num8;
        int offset2;

        if (underPoint > 0)
        {
            //buffer[offset++] = ((char)'.');

            num8 = underPoint;

            num8 -= (num4 = num8 / 10000) * 10000;
            num4 -= (num2 = Div100(num4)) * 100;
            num2 -= (num1 = Div10(num2)) * 10;

            if (num8 > 0) // 5～8桁を評価
            {
                // 小数点以下出力は、5桁以上
                num4 -= (num3 = Div10(num4)) * 10;
                num8 -= (num6 = Div100(num8)) * 100;
                num6 -= (num5 = Div10(num6)) * 10;
                if (num8 > 0) // 7～8桁を評価
                {
                    // 小数点以下出力は、7or8桁
                    num8 -= (num7 = Div10(num8)) * 10;
                    if (num8 > 0) // 8桁を評価
                    {
                        // 小数点以下出力は、8桁
                        offset2 = offset += 9;
                        if (destination.Length < offset) { return false; }
                        goto U8;
                    }
                    else
                    {
                        // 小数点以下出力は、7桁
                        offset2 = offset += 8;
                        if (destination.Length < offset) { return false; }
                        goto U7;
                    }
                }
                else
                {
                    // 小数点以下出力は、5or6桁
                    if (num6 > 0) // 6桁を評価
                    {
                        // 小数点以下出力は、6桁
                        offset2 = offset += 7;
                        if (destination.Length < offset) { return false; }
                        goto U6;
                    }
                    else
                    {
                        // 小数点以下出力は、5桁
                        offset2 = offset += 6;
                        if (destination.Length < offset) { return false; }
                        goto U5;
                    }
                }
            }
            else
            {
                // 小数点以下出力は、4桁以下
                if (num4 > 0) // 3～4桁を評価
                {
                    // 小数点以下出力は、3or4桁
                    num4 -= (num3 = Div10(num4)) * 10;
                    if (num4 > 0) // 4桁を評価
                    {
                        // 小数点以下出力は、4桁
                        offset2 = offset += 5;
                        if (destination.Length < offset) { return false; }
                        goto U4;
                    }
                    else
                    {
                        // 小数点以下出力は、3桁
                        offset2 = offset += 4;
                        if (destination.Length < offset) { return false; }
                        goto U3;
                    }
                }
                else
                {
                    // 小数点以下出力は、1or2桁
                    if (num2 > 0) // 2桁を評価
                    {
                        // 小数点以下出力は、2桁
                        offset2 = offset += 3;
                        if (destination.Length < offset) { return false; }
                        goto U2;
                    }
                    else
                    {
                        // 小数点以下出力は、1桁
                        offset2 = offset += 2;
                        if (destination.Length < offset) { return false; }
                        goto U1;
                    }
                }
            }

        U8:
            destination[--offset2] = (char)('0' + num8);
        U7:
            destination[--offset2] = (char)('0' + num7);
        U6:
            destination[--offset2] = (char)('0' + num6);
        U5:
            destination[--offset2] = (char)('0' + num5);
        U4:
            destination[--offset2] = (char)('0' + num4);
        U3:
            destination[--offset2] = (char)('0' + num3);
        U2:
            destination[--offset2] = (char)('0' + num2);
        U1:
            destination[--offset2] = (char)('0' + num1);
            destination[--offset2] = '.';
        }

        charsWritten = offset;
        return true;
    }

    public int WriteChars(ref Span<char> destination)
    {
        int offset = 0;
        int charsWritten;

        var value = InnerValue;

        uint num1, num2, num3, div;
        ulong valueA, valueB;

        if (value < 0)
        {
            if (InnerValue == long.MinValue)
            {
                string minValue = "-92233720368.54775808";

                charsWritten = minValue.Length;
                minValue.CopyTo(destination);
                return charsWritten;
            }

            destination[offset++] = (char)'-';
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

                num3 = (uint)valueA;
                if (num3 < 100)
                {
                    if (num3 < 10) { goto L9; }
                    goto L10;
                }
                if (num3 < 1000) { goto L11; }
                goto L12;

            L12:
                destination[offset++] = (char)('0' + (div = Div1000(num3)));
                num3 -= div * 1000;
            L11:
                destination[offset++] = (char)('0' + (div = Div100(num3)));
                num3 -= div * 100;
            L10:
                destination[offset++] = (char)('0' + (div = Div10(num3)));
                num3 -= div * 10;
            L9:
                destination[offset++] = (char)('0' + num3);

            }
        L8:
            destination[offset++] = (char)('0' + (div = Div1000(num2)));
            num2 -= div * 1000;
        L7:
            destination[offset++] = (char)('0' + (div = Div100(num2)));
            num2 -= div * 100;
        L6:
            destination[offset++] = (char)('0' + (div = Div10(num2)));
            num2 -= div * 10;
        L5:
            destination[offset++] = (char)('0' + num2);

        }

    L4:
        destination[offset++] = (char)('0' + (div = Div1000(num1)));
        num1 -= div * 1000;
    L3:
        destination[offset++] = (char)('0' + (div = Div100(num1)));
        num1 -= div * 100;
    L2:
        destination[offset++] = (char)('0' + (div = Div10(num1)));
        num1 -= div * 10;
    L1:
        destination[offset++] = (char)('0' + num1);


        uint num4, num5, num6, num7, num8;
        int offset2;

        if (underPoint > 0)
        {
            //buffer[offset++] = ((char)'.');

            num8 = underPoint;

            num8 -= (num4 = num8 / 10000) * 10000;
            num4 -= (num2 = Div100(num4)) * 100;
            num2 -= (num1 = Div10(num2)) * 10;

            if (num8 > 0) // 5～8桁を評価
            {
                // 小数点以下出力は、5桁以上
                num4 -= (num3 = Div10(num4)) * 10;
                num8 -= (num6 = Div100(num8)) * 100;
                num6 -= (num5 = Div10(num6)) * 10;
                if (num8 > 0) // 7～8桁を評価
                {
                    // 小数点以下出力は、7or8桁
                    num8 -= (num7 = Div10(num8)) * 10;
                    if (num8 > 0) // 8桁を評価
                    {
                        // 小数点以下出力は、8桁
                        offset2 = offset += 9;
                        goto U8;
                    }
                    else
                    {
                        // 小数点以下出力は、7桁
                        offset2 = offset += 8;
                        goto U7;
                    }
                }
                else
                {
                    // 小数点以下出力は、5or6桁
                    if (num6 > 0) // 6桁を評価
                    {
                        // 小数点以下出力は、6桁
                        offset2 = offset += 7;
                        goto U6;
                    }
                    else
                    {
                        // 小数点以下出力は、5桁
                        offset2 = offset += 6;
                        goto U5;
                    }
                }
            }
            else
            {
                // 小数点以下出力は、4桁以下
                if (num4 > 0) // 3～4桁を評価
                {
                    // 小数点以下出力は、3or4桁
                    num4 -= (num3 = Div10(num4)) * 10;
                    if (num4 > 0) // 4桁を評価
                    {
                        // 小数点以下出力は、4桁
                        offset2 = offset += 5;
                        goto U4;
                    }
                    else
                    {
                        // 小数点以下出力は、3桁
                        offset2 = offset += 4;
                        goto U3;
                    }
                }
                else
                {
                    // 小数点以下出力は、1or2桁
                    if (num2 > 0) // 2桁を評価
                    {
                        // 小数点以下出力は、2桁
                        offset2 = offset += 3;
                        goto U2;
                    }
                    else
                    {
                        // 小数点以下出力は、1桁
                        offset2 = offset += 2;
                        goto U1;
                    }
                }
            }

        U8:
            destination[--offset2] = (char)('0' + num8);
        U7:
            destination[--offset2] = (char)('0' + num7);
        U6:
            destination[--offset2] = (char)('0' + num6);
        U5:
            destination[--offset2] = (char)('0' + num5);
        U4:
            destination[--offset2] = (char)('0' + num4);
        U3:
            destination[--offset2] = (char)('0' + num3);
        U2:
            destination[--offset2] = (char)('0' + num2);
        U1:
            destination[--offset2] = (char)('0' + num1);
            destination[--offset2] = '.';
        }

        charsWritten = offset;
        return charsWritten;
    }


    public bool TryWriteUtf8(Span<byte> destination, out int charsWritten)
    {
        int offset = 0;
        charsWritten = 0;

        var value = InnerValue;

        uint num1, num2, num3, div;
        ulong valueA, valueB;

        if (value < 0)
        {
            if (InnerValue == long.MinValue)
            {
                ReadOnlySpan<byte> minValue = "-92233720368.54775808"u8;

                charsWritten = minValue.Length;
                return minValue.TryCopyTo(destination);
            }

            if (destination.Length <= offset ) { return false; }
            destination[offset++] = (byte)'-';
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
                goto L1;
            }
            if (num1 < 100)
            {
                if (destination.Length < offset + 2) { return false; }
                goto L2;
            }
            if (num1 < 1000)
            {
                if (destination.Length < offset + 3) { return false; }
                goto L3;
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
                        goto L5;
                    }
                    if (destination.Length < offset + 6) { return false; }
                    goto L6;
                }
                if (num2 < 1000)
                {
                    if (destination.Length < offset + 7) { return false; }
                    goto L7;
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
                        goto L9;
                    }
                    if (destination.Length < offset + 10) { return false; }
                    goto L10;
                }
                if (num3 < 1000)
                {
                    if (destination.Length < offset + 11) { return false; }
                    goto L11;
                }
                if (destination.Length < offset + 12) { return false; }
                goto L12;

            L12:
                destination[offset++] = (byte)('0' + (div = Div1000(num3)));
                num3 -= div * 1000;
            L11:
                destination[offset++] = (byte)('0' + (div = Div100(num3)));
                num3 -= div * 100;
            L10:
                destination[offset++] = (byte)('0' + (div = Div10(num3)));
                num3 -= div * 10;
            L9:
                destination[offset++] = (byte)('0' + num3);

            }
        L8:
            destination[offset++] = (byte)('0' + (div = Div1000(num2)));
            num2 -= div * 1000;
        L7:
            destination[offset++] = (byte)('0' + (div = Div100(num2)));
            num2 -= div * 100;
        L6:
            destination[offset++] = (byte)('0' + (div = Div10(num2)));
            num2 -= div * 10;
        L5:
            destination[offset++] = (byte)('0' + num2);

        }

    L4:
        destination[offset++] = (byte)('0' + (div = Div1000(num1)));
        num1 -= div * 1000;
    L3:
        destination[offset++] = (byte)('0' + (div = Div100(num1)));
        num1 -= div * 100;
    L2:
        destination[offset++] = (byte)('0' + (div = Div10(num1)));
        num1 -= div * 10;
    L1:
        destination[offset++] = (byte)('0' + num1);


        uint num4, num5, num6, num7, num8;
        int offset2;

        if (underPoint > 0)
        {
            //buffer[offset++] = ((char)'.');

            num8 = underPoint;

            num8 -= (num4 = num8 / 10000) * 10000;
            num4 -= (num2 = Div100(num4)) * 100;
            num2 -= (num1 = Div10(num2)) * 10;

            if (num8 > 0) // 5～8桁を評価
            {
                // 小数点以下出力は、5桁以上
                num4 -= (num3 = Div10(num4)) * 10;
                num8 -= (num6 = Div100(num8)) * 100;
                num6 -= (num5 = Div10(num6)) * 10;
                if (num8 > 0) // 7～8桁を評価
                {
                    // 小数点以下出力は、7or8桁
                    num8 -= (num7 = Div10(num8)) * 10;
                    if (num8 > 0) // 8桁を評価
                    {
                        // 小数点以下出力は、8桁
                        offset2 = offset += 9;
                        if (destination.Length < offset) { return false; }
                        goto U8;
                    }
                    else
                    {
                        // 小数点以下出力は、7桁
                        offset2 = offset += 8;
                        if (destination.Length < offset) { return false; }
                        goto U7;
                    }
                }
                else
                {
                    // 小数点以下出力は、5or6桁
                    if (num6 > 0) // 6桁を評価
                    {
                        // 小数点以下出力は、6桁
                        offset2 = offset += 7;
                        if (destination.Length < offset) { return false; }
                        goto U6;
                    }
                    else
                    {
                        // 小数点以下出力は、5桁
                        offset2 = offset += 6;
                        if (destination.Length < offset) { return false; }
                        goto U5;
                    }
                }
            }
            else
            {
                // 小数点以下出力は、4桁以下
                if (num4 > 0) // 3～4桁を評価
                {
                    // 小数点以下出力は、3or4桁
                    num4 -= (num3 = Div10(num4)) * 10;
                    if (num4 > 0) // 4桁を評価
                    {
                        // 小数点以下出力は、4桁
                        offset2 = offset += 5;
                        if (destination.Length < offset) { return false; }
                        goto U4;
                    }
                    else
                    {
                        // 小数点以下出力は、3桁
                        offset2 = offset += 4;
                        if (destination.Length < offset) { return false; }
                        goto U3;
                    }
                }
                else
                {
                    // 小数点以下出力は、1or2桁
                    if (num2 > 0) // 2桁を評価
                    {
                        // 小数点以下出力は、2桁
                        offset2 = offset += 3;
                        if (destination.Length < offset) { return false; }
                        goto U2;
                    }
                    else
                    {
                        // 小数点以下出力は、1桁
                        offset2 = offset += 2;
                        if (destination.Length < offset) { return false; }
                        goto U1;
                    }
                }
            }

        U8:
            destination[--offset2] = (byte)('0' + num8);
        U7:
            destination[--offset2] = (byte)('0' + num7);
        U6:
            destination[--offset2] = (byte)('0' + num6);
        U5:
            destination[--offset2] = (byte)('0' + num5);
        U4:
            destination[--offset2] = (byte)('0' + num4);
        U3:
            destination[--offset2] = (byte)('0' + num3);
        U2:
            destination[--offset2] = (byte)('0' + num2);
        U1:
            destination[--offset2] = (byte)('0' + num1);
            destination[--offset2] = (byte)'.';
        }

        charsWritten = offset;
        return true;
    }

    public int WriteUtf8(ref Span<byte> destination)
    {
        int offset = 0;
        int charsWritten;

        var value = InnerValue;

        uint num1, num2, num3, div;
        ulong valueA, valueB;

        if (value < 0)
        {
            if (InnerValue == long.MinValue)
            {
                ReadOnlySpan<byte> minValue = "-92233720368.54775808"u8;

                charsWritten = minValue.Length;
                minValue.CopyTo(destination);
                return charsWritten;
            }

            destination[offset++] = (byte)'-';
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

                num3 = (uint)valueA;
                if (num3 < 100)
                {
                    if (num3 < 10) { goto L9; }
                    goto L10;
                }
                if (num3 < 1000) { goto L11; }
                goto L12;

            L12:
                destination[offset++] = (byte)('0' + (div = Div1000(num3)));
                num3 -= div * 1000;
            L11:
                destination[offset++] = (byte)('0' + (div = Div100(num3)));
                num3 -= div * 100;
            L10:
                destination[offset++] = (byte)('0' + (div = Div10(num3)));
                num3 -= div * 10;
            L9:
                destination[offset++] = (byte)('0' + num3);

            }
        L8:
            destination[offset++] = (byte)('0' + (div = Div1000(num2)));
            num2 -= div * 1000;
        L7:
            destination[offset++] = (byte)('0' + (div = Div100(num2)));
            num2 -= div * 100;
        L6:
            destination[offset++] = (byte)('0' + (div = Div10(num2)));
            num2 -= div * 10;
        L5:
            destination[offset++] = (byte)('0' + num2);

        }

    L4:
        destination[offset++] = (byte)('0' + (div = Div1000(num1)));
        num1 -= div * 1000;
    L3:
        destination[offset++] = (byte)('0' + (div = Div100(num1)));
        num1 -= div * 100;
    L2:
        destination[offset++] = (byte)('0' + (div = Div10(num1)));
        num1 -= div * 10;
    L1:
        destination[offset++] = (byte)('0' + num1);


        uint num4, num5, num6, num7, num8;
        int offset2;

        if (underPoint > 0)
        {
            //buffer[offset++] = ((char)'.');

            num8 = underPoint;

            num8 -= (num4 = num8 / 10000) * 10000;
            num4 -= (num2 = Div100(num4)) * 100;
            num2 -= (num1 = Div10(num2)) * 10;

            if (num8 > 0) // 5～8桁を評価
            {
                // 小数点以下出力は、5桁以上
                num4 -= (num3 = Div10(num4)) * 10;
                num8 -= (num6 = Div100(num8)) * 100;
                num6 -= (num5 = Div10(num6)) * 10;
                if (num8 > 0) // 7～8桁を評価
                {
                    // 小数点以下出力は、7or8桁
                    num8 -= (num7 = Div10(num8)) * 10;
                    if (num8 > 0) // 8桁を評価
                    {
                        // 小数点以下出力は、8桁
                        offset2 = offset += 9;
                        goto U8;
                    }
                    else
                    {
                        // 小数点以下出力は、7桁
                        offset2 = offset += 8;
                        goto U7;
                    }
                }
                else
                {
                    // 小数点以下出力は、5or6桁
                    if (num6 > 0) // 6桁を評価
                    {
                        // 小数点以下出力は、6桁
                        offset2 = offset += 7;
                        goto U6;
                    }
                    else
                    {
                        // 小数点以下出力は、5桁
                        offset2 = offset += 6;
                        goto U5;
                    }
                }
            }
            else
            {
                // 小数点以下出力は、4桁以下
                if (num4 > 0) // 3～4桁を評価
                {
                    // 小数点以下出力は、3or4桁
                    num4 -= (num3 = Div10(num4)) * 10;
                    if (num4 > 0) // 4桁を評価
                    {
                        // 小数点以下出力は、4桁
                        offset2 = offset += 5;
                        goto U4;
                    }
                    else
                    {
                        // 小数点以下出力は、3桁
                        offset2 = offset += 4;
                        goto U3;
                    }
                }
                else
                {
                    // 小数点以下出力は、1or2桁
                    if (num2 > 0) // 2桁を評価
                    {
                        // 小数点以下出力は、2桁
                        offset2 = offset += 3;
                        goto U2;
                    }
                    else
                    {
                        // 小数点以下出力は、1桁
                        offset2 = offset += 2;
                        goto U1;
                    }
                }
            }

        U8:
            destination[--offset2] = (byte)('0' + num8);
        U7:
            destination[--offset2] = (byte)('0' + num7);
        U6:
            destination[--offset2] = (byte)('0' + num6);
        U5:
            destination[--offset2] = (byte)('0' + num5);
        U4:
            destination[--offset2] = (byte)('0' + num4);
        U3:
            destination[--offset2] = (byte)('0' + num3);
        U2:
            destination[--offset2] = (byte)('0' + num2);
        U1:
            destination[--offset2] = (byte)('0' + num1);
            destination[--offset2] = (byte)'.';
        }

        charsWritten = offset;
        return charsWritten;
    }


    public string ToString(string? format, IFormatProvider? formatProvider)　
    {
        decimal dec = (decimal)this;
        return dec.ToString(format, formatProvider);
    }

    public static FixedPoint8 Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        var dec = decimal.Parse(s, provider);
        return (FixedPoint8)dec;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out FixedPoint8 result)
    {
        var success = decimal.TryParse(s, provider,out var decResult);
        if(success)
        {
            result = (FixedPoint8)decResult;
            return true;
        }
        else
        {
            result = FixedPoint8.Zero;
            return false;
        }
    }

    public static FixedPoint8 Parse(string s, IFormatProvider? provider)
    {
        var dec = decimal.Parse(s, provider);
        return (FixedPoint8)dec;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out FixedPoint8 result)
    {
        var success = decimal.TryParse(s, provider, out var decResult);
        if (success)
        {
            result = (FixedPoint8)decResult;
            return true;
        }
        else
        {
            result = FixedPoint8.Zero;
            return false;
        }
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

        if (_innerValue < 0)
        {
            absValue = -_innerValue;
            sign = -1;
        }
        else
        {
            absValue = _innerValue;
            sign = 1;
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
        var value = InnerValue + 50000000 * sign;
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

        long absValue;
        int sign; // 1 = + ,-1 = -

        if (_innerValue < 0)
        {
            absValue = -_innerValue;
            sign = -1;
        }
        else
        {
            absValue = _innerValue;
            sign = 1;
        }

        var overDiv = powerArray[-decimals + 8];
        var absOverRound = absValue / (long)overDiv;

        var underPower = powerArray[10 + decimals];
        var absUnderRound = (absValue - absOverRound * (long)overDiv) * (long)underPower;

        if (absUnderRound == 500000000000000000)
        {
            if ((absOverRound & 1) == 0)
            {
                return new FixedPoint8(absOverRound * (long)overDiv * sign) ;
            }
            return new FixedPoint8((absOverRound + 1) * (long)overDiv * sign);
        }

        var mRoundOff = 5 * (long)overDiv / 10;
        return new FixedPoint8(((absValue + mRoundOff) / (long)overDiv) * (long)overDiv * sign);
    }

    public FixedPoint8 Floor()
    {
        long absValue;

        if (_innerValue < 0)
        {
            absValue = -_innerValue;

            var overPoint = absValue / InnerPower;
            var underPoint = absValue - overPoint * InnerPower;

            if (underPoint == 0)
            {
                return new FixedPoint8(-overPoint * InnerPower);
            }
            return new FixedPoint8(-(1 + overPoint) * InnerPower);
        }
        else
        {
            absValue = _innerValue;

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

        long absValue;

        if (_innerValue < 0)
        {
            absValue = -_innerValue;

            var overDiv = powerArray[-decimals + 8];
            var absOverRound = absValue / (long)overDiv;

            var underPower = powerArray[10 + decimals];
            var absUnderRound = (absValue - absOverRound * (long)overDiv) * (long)underPower;

            if (absUnderRound == 0)
            {
                return new FixedPoint8(-absOverRound * (long)overDiv);
            }
            return new FixedPoint8(-(absOverRound + 1) * (long)overDiv);

        }
        else
        {
            absValue = _innerValue;

            var overDiv = powerArray[-decimals + 8];
            var absOverRound = absValue / (long)overDiv;

            return new FixedPoint8(absOverRound * (long)overDiv);
        }
    }


    public FixedPoint8 Truncate()
    {
        long absValue;
        int sign; // 1 = + ,-1 = -

        if (_innerValue < 0)
        {
            absValue = -_innerValue;
            sign = -1;
        }
        else
        {
            absValue = _innerValue;
            sign = 1;
        }

        var overPoint = absValue / InnerPower;

        return new FixedPoint8(overPoint * InnerPower * sign);
    }

    public FixedPoint8 Truncate(int decimals)
    {
        if (decimals < -10 || decimals > 7)
        {
            throw new ArgumentOutOfRangeException(_decimalsOutOfRange);
        }

        long absValue;
        int sign; // 1 = + ,-1 = -

        if (_innerValue < 0)
        {
            absValue = -_innerValue;
            sign = -1;
        }
        else
        {
            absValue = _innerValue;
            sign = 1;
        }

        var overDiv = powerArray[-decimals + 8];
        var absOverRound = absValue / (long)overDiv;

        return new FixedPoint8(absOverRound * (long)overDiv * sign);
    }


    public FixedPoint8 Ceiling()
    {
        long absValue;

        if (_innerValue < 0)
        {
            absValue = -_innerValue;

            var overPoint = absValue / InnerPower;

            return new FixedPoint8(-overPoint * InnerPower);
        }
        else
        {
            absValue = _innerValue;

            var overPoint = absValue / InnerPower;
            var underPoint = absValue - overPoint * InnerPower;

            if (underPoint == 0)
            {
                return new FixedPoint8(overPoint * InnerPower);
            }
            return new FixedPoint8((1 + overPoint) * InnerPower);
        }
    }


    public FixedPoint8 Ceiling(int decimals)
    {
        long absValue;

        if (_innerValue < 0)
        {
            absValue = -_innerValue;

            var overDiv = powerArray[-decimals + 8];
            var absOverRound = absValue / (long)overDiv;

            return new FixedPoint8(-absOverRound * (long)overDiv);
        }
        else
        {
            absValue = _innerValue;

            var overDiv = powerArray[-decimals + 8];
            var absOverRound = absValue / (long)overDiv;

            var underPower = powerArray[10 + decimals];
            var absUnderRound = (absValue - absOverRound * (long)overDiv) * (long)underPower;

            if (absUnderRound == 0)
            {
                return new FixedPoint8(absOverRound * (long)overDiv);
            }
            return new FixedPoint8((absOverRound + 1) * (long)overDiv);
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








