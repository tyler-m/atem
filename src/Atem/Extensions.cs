using System;

namespace Atem
{
    public static class Extensions
    {
        public static string Truncate(this string input, int maxLength, string appendage = "...")
        {
            if (string.IsNullOrEmpty(input) || maxLength <= 3)
            {
                return input;
            }

            if (input.Length > maxLength)
            {
                return string.Concat(input.AsSpan(0, maxLength - 3), appendage);
            }
            else
            {
                return input;
            }
        }
    }
}
