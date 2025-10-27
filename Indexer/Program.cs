using System.Diagnostics;
using Common;

namespace Indexer;

internal class Program
{
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

        List<string> allTerms = articles.Select(article => article.Text.ExtractTerms()).SelectMany(i => i).ToList();
        timer.Report("Terms Tokenized & Stemmed");

        // All unique terms
        string[] corpus = allTerms.Distinct().ToArray();
        List<int[]> documentVectors = [];

        // The following could be a linq statement but whatever
        foreach (List<string> articleTerms in articles.Select(article => article.Text.ExtractTerms()))
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

        CorpusIndex index = new()
        {
            Terms = corpus,
            Documents = articles.Select(article => new CorpusIndexEntry(article.Id, article.Title, documentVectors[article.Id])).ToArray()
        };

        index.ToFile(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "index.json"));
        timer.Report("Index File Written");

        Console.WriteLine($"Done In {fullTimer.Elapsed:g}");
    }
}