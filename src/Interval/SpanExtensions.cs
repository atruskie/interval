using System;


namespace Util
{
    public static class SpanExtensions
    {
        public static bool TryConsume<T>(this in ReadOnlySpan<T> input, ReadOnlySpan<T> token, out ReadOnlySpan<T> next)
            where T : IEquatable<T>
        {
            if (input.StartsWith(token))
            {
                next = input[token.Length..];
                return true;
            }
            
            next = input;
            return false;
        }
        public static bool TryConsume<T>(this in ReadOnlySpan<T> input, T token, out ReadOnlySpan<T> next)
            where T : IEquatable<T>
        {
            if (input.Length > 0 && input[0].Equals(token))
            {
                next = input[1..];
                return true;
            }

            next = input;
            return false;
        }
    }
}