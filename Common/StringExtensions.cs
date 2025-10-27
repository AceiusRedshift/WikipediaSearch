using System.Text.RegularExpressions;
using Indexer;
namespace Common;

public static partial class StringExtensions
{
    [GeneratedRegex("\"\\[\\[[^\\[]+?\\]\\]|[^\\W_]+’[^\\W_]+|[^\\W_]+\"")]
    private static partial Regex GeneratedTokenMatchingRegexPattern();

    static string SanitizedMatch(Match match) => match.Value.ToLower().Trim('"');
    
    public static List<string> ExtractTerms(this string input) => GeneratedTokenMatchingRegexPattern()
        .Matches(input)
        .Select(SanitizedMatch)
        .Where(StopWords.IsNotStopWord)
        .Select(word => PorterStemmer.StemOneWord(word, new PorterStemmerImpl()))
        .ToList();
}