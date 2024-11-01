using System.Text.RegularExpressions;

namespace Api.Modules;

public static partial class StringExtension
{
    [GeneratedRegex("([a-z0-9])([A-Z])", RegexOptions.Compiled)]
    private static partial Regex KebabCaseRule();

    public static string ToKebabCase(this string input)
        => KebabCaseRule().Replace(input, "$1-$2").ToLower().Trim('-');
}
