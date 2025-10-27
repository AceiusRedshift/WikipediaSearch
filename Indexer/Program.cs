using System.Diagnostics;
using System.Text.RegularExpressions;
using Common;

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

    private static Dictionary<string, int> TermOccurrence(List<string> terms)
    {
        Dictionary<string, int> termOccurrences = [];

        foreach (string term in terms)
        {
            termOccurrences[term] = termOccurrences.TryGetValue(term, out int value) ? value + 1 : 1;
        }

        return termOccurrences;
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Starting Indexer");
        Stopwatch timer = Stopwatch.StartNew();
        Stopwatch fullTimer = Stopwatch.StartNew();

        var articles = Parser.ParseFile("SmallWiki.xml");
        timer.Report("Articles Parsed");

        List<string> allTerms = articles.Select(GetAllTerms).SelectMany(i => i).ToList();
        timer.Report("Terms Tokenized & Stemmed");

        // All unique terms
        string[] corpus = allTerms.Distinct().ToArray();
        List<int[]> documentVectors = [];

        // The following could be a linq statement but whatever
        foreach (List<string> articleTerms in articles.Select(GetAllTerms))
        {
            List<int> corpusTermCount = [];
            Dictionary<string, int> articleTermCount = TermOccurrence(articleTerms);

            foreach (string term in corpus)
            {
                corpusTermCount.Add(articleTermCount.GetValueOrDefault(term, 0));
            }

            documentVectors.Add(corpusTermCount.ToArray());
        }
        
        timer.Report("Document Vectors Created");

        CorpusIndex index = new CorpusIndex()
        {
            Terms = corpus,
            Documents = documentVectors.ToList()
        };

        index.ToFile(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "index.json"));
        timer.Report("Index File Written");
        
        Console.WriteLine($"Done in {fullTimer.Elapsed}");
    }
}