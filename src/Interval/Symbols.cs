using System;
using System.Text;

namespace Math.Interval
{
    public static class Symbols {
        internal const byte leftSquare = (byte)'[';
        internal const byte leftParen = (byte)'(';
        internal const byte rightSquare = (byte)']';
        internal const byte rightParen = (byte)')';
        internal const byte tildeSymbol = (byte)'~';
        internal const byte greaterThan = (byte)'>';
        internal const byte lessThan = (byte)'<';
        internal const byte comma = (byte)',';
        internal static ReadOnlySpan<byte> approximateSymbol => Encoding.UTF8.GetBytes("≈");
        internal static ReadOnlySpan<byte> greaterEqualSymbol => Encoding.UTF8.GetBytes("≥");
        internal static ReadOnlySpan<byte> greaterEqualSymbolAlt => Encoding.UTF8.GetBytes(">=");
        internal static ReadOnlySpan<byte> lessEqualSymbol => Encoding.UTF8.GetBytes("≤");
        internal static ReadOnlySpan<byte> lessEqualSymbolAlt => Encoding.UTF8.GetBytes("<=");
        internal static ReadOnlySpan<byte> toleranceSymbol => Encoding.UTF8.GetBytes("±");
        internal static ReadOnlySpan<byte> toleranceSymbolAlt => Encoding.UTF8.GetBytes("±");
        internal static ReadOnlySpan<byte> epsilonSymbol => Encoding.UTF8.GetBytes("ε");
        internal static ReadOnlySpan<byte> emptySymbol => Encoding.UTF8.GetBytes("∅");
    }
}