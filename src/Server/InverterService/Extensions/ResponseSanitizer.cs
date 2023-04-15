using System.Text.RegularExpressions;

namespace InverterMon.Server.InverterService;

public static partial class Extensions
{
    [GeneratedRegex("[^\\u0009\\u000A\\u000D\\u0020-\\u007E]")]
    private static partial Regex StringSanitizer();
    private static readonly Regex sanRx = StringSanitizer();
    public static string Sanitize(this string input) => sanRx.Replace(input, "");

    [GeneratedRegex("'\\((.*?)\\\\")]
    private static partial Regex CLIParser();
    private static readonly Regex cliRx = CLIParser();
    public static string ParseCLI(this string input)
    {
        var match = cliRx.Match(input);
        if (match.Success)
        {
            return match.Groups[0].Value;
        }
        return "`(NAK\\";
    }
}