using System.Diagnostics;
using Common;
using Indexer.Document;

// The total number of times tf the term appears in the corpus.
Dictionary<int, Dictionary<string, int>> termFrequency = [];
// The total number of documents n the term appears in.
Dictionary<string, int> termOccurrence = [];
Dictionary<string, double> inverseDocumentFrequency = [];
Dictionary<int, string> titles = [];

string path = args.ElementAtOrDefault(0) ?? "SmallWiki.xml";
Stopwatch timer = Stopwatch.StartNew();

Console.WriteLine($"Parsing {path}");
Article[] wiki = Parser.ParseFile(path);

foreach (Article article in wiki)
{
    int id = article.Id;
    string title = article.Title.Trim();
    string[] terms = article.Text.ToTerms();

    termFrequency[id] = [];
    titles[id] = title;

    // if link

    foreach (string term in terms)
    {
        if (!termFrequency[id].TryAdd(term, 1))
        {
            termFrequency[id][term]++;
        }
    }

    foreach (string distinctTerm in terms.Distinct())
    {
        if (!termOccurrence.TryAdd(distinctTerm, 1))
        {
            termOccurrence[distinctTerm]++;
        }
    }
}

IEnumerable<string> allTerms = termOccurrence.Keys;// If the term appears at least once it will be in the occurrence dictionary, so instead of parsing the corpus again we re-use this.
int articleCount = wiki.Length;

foreach (string term in allTerms)
{
    inverseDocumentFrequency[term] = Math.Log((float)articleCount / termOccurrence[term]);
}

IndexFile index = new(titles, termFrequency, termOccurrence, inverseDocumentFrequency);
index.Save();

Console.WriteLine($"Done in {timer.Elapsed:g}");