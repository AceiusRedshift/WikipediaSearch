using System.Text.RegularExpressions;
namespace Common;

public static partial class StringExtensions
{
    [GeneratedRegex("""\[\[[^\[]+?\]\]|[^\W_]+’[^\W_]+|[^\W_]+""")]
    private static partial Regex TermRegex();
    private static readonly Regex RegexInstance = TermRegex();

    public static string[] ToTerms(this string s) => PorterStemmer.StemWords(RegexInstance.Matches(s).Select(match => match.Value).Where(StopWords.IsNotStopWord).ToArray());
}