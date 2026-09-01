using System.Diagnostics.CodeAnalysis;

namespace Sentinela.Domain.Extensions;

public static class StringExtensions
{
    public static bool IsNotEmpty([NotNullWhen(true)]this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    public static bool IsEmpty([NotNullWhen(false)] this string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }
}
