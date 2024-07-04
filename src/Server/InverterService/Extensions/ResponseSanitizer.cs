using System.Text.RegularExpressions;

namespace InverterMon.Server.InverterService;

public static partial class Extensions
{
    [GeneratedRegex(@"[^\u0009\u000A\u000D\u0020-\u007E]")]
    private static partial Regex StringSanitizer();

    static readonly Regex _sanRx = StringSanitizer();

    public static string Sanitize(this string input)
        => _sanRx.Replace(input, "");

    [GeneratedRegex(@"'\((.*?)\\")]
    private static partial Regex CLIParser();

    static readonly Regex _cliRx = CLIParser();

    public static string ParseCli(this string input)
    {
        var match = _cliRx.Match(input);

        return match.Success
                   ? match.Groups[0].Value
                   : "`(NAK\\";
    }
}