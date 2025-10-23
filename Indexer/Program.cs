using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Indexer;

internal partial class Program
{
    [GeneratedRegex("\"\\[\\[[^\\[]+?\\]\\]|[^\\W_]+’[^\\W_]+|[^\\W_]+\"")]
    private static partial Regex GeneratedTokenMatchingRegexPattern();

    static string SanitizedMatch(Match match) => match.Value.ToLower().Trim('"');

    static List<string[]> AllTerms(Article[] articles)
    {
        List<string[]> allTerms = [];

        foreach (Article article in articles)
        {
            Regex tokenMatchingPattern = GeneratedTokenMatchingRegexPattern();

            string[] terms = tokenMatchingPattern
                .Matches(article.Text)
                .Select(SanitizedMatch)
                .Where(StopWords.IsNotStopWord)
                .Select(word => PorterStemmer.StemOneWord(word, new PorterStemmerImpl()))
                .ToArray();

            allTerms.Add(terms);
        }

        return allTerms;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Starting Indexer");
        Stopwatch timer = Stopwatch.StartNew();

        var articles = Parser.ParseFile("SmallWiki.xml");
        timer.Report("Corpus Parsed");

        List<IndexInfo> index = articles.Select(article => new IndexInfo(article, [],[])).ToList();

        List<string[]> allTerms = AllTerms(articles);
        index.ForEach(article => article = article with
        {
            Terms = allTerms[article.Article.Id]
        });
        timer.Report("Terms Tokenized & Stemmed");

        Dictionary<string, int> termOccurrence = TermOccurrence(allTerms);
        timer.Report("Term Occurrence Counted");
        
        
    }

    private static Dictionary<string, int> TermOccurrence(List<string[]> allTerms)
    {
        Dictionary<string, int> termOccurrences = [];

        foreach (string[] termSet in allTerms)
        {
            foreach (string term in termSet)
            {
                termOccurrences[term] = termOccurrences.TryGetValue(term, out int value) ? value + 1 : 1;
            }
        }

        return termOccurrences;
    }
}

internal record IndexInfo(Article Article, string[] Terms, int[] Vec);