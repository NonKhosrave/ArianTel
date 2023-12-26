using System;
using System.Linq;

namespace BuildingBlocks.Service;
public static class UnicodeExtensions
{
    public static string CleanUnderLines(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        const char chr1600 = (char)1600; //ـ=1600
        const char chr8204 = (char)8204; //‌=8204

        return text.Replace(chr1600.ToString(), "", StringComparison.Ordinal)
            .Replace(chr8204.ToString(), "", StringComparison.Ordinal);
    }

    public static string RemovePunctuation(this string text)
    {
        return string.IsNullOrWhiteSpace(text)
            ? string.Empty
            : new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
    }
}
