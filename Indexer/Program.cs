using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Indexer;

internal partial class Program
{
    [GeneratedRegex("\"\\[\\[[^\\[]+?\\]\\]|[^\\W_]+’[^\\W_]+|[^\\W_]+\"")]
    private static partial Regex GeneratedTokenMatchingRegexPattern();

    static string SanitizedMatch(Match match) => match.Value.ToLower().Trim('"');

    static List<string> GetAllTerms(Article article) => GeneratedTokenMatchingRegexPattern()
        .Matches(article.Text)
        .Select(SanitizedMatch)
        .Where(StopWords.IsNotStopWord)
        .Select(word => PorterStemmer.StemOneWord(word, new PorterStemmerImpl()))
        .ToList();

    public static void Main(string[] args)
    {
        Console.WriteLine("Starting Indexer");
        Stopwatch timer = Stopwatch.StartNew();

        var articles = Parser.ParseFile("SmallWiki.xml");
        timer.Report("Corpus Parsed");

        List<string> allTerms = articles.Select(GetAllTerms).SelectMany(i => i).Distinct().ToList();
        timer.Report("Terms Tokenized & Stemmed");

        foreach (Article article in articles)
        {
            var l = TermOccurrence(GetAllTerms(article));
        }
        
        Dictionary<string, int> termOccurrence = TermOccurrence(allTerms);
        timer.Report("Term Occurrence Counted");
        
        
    }

    private static Dictionary<string, int> TermOccurrence(List<string> terms)
    {
        Dictionary<string, int> termOccurrences = [];

        foreach (string term in terms)
        {
            termOccurrences[term] = termOccurrences.TryGetValue(term, out int value) ? value + 1 : 1;
        }

        return termOccurrences;
    }
}