using System.Text.Json;
using System.Text.RegularExpressions;
using Common;
using Indexer.Document;

// The total number of times tf the term appears in the corpus.
Dictionary<string, int> termFrequency = [];
// The total number of documents n the term appears in.
Dictionary<string, int> termOccurrence = [];
Dictionary<string, double> inverseDocumentFrequency = [];

Article[] wiki = Parser.ParseFile("SmallWiki.xml");

foreach (Article article in wiki)
{
    int id = article.Id;
    string title = article.Title.Trim();
    string[] terms = article.Text.ToTerms();

    // if link

    foreach (string term in terms)
    {
        if (!termFrequency.TryAdd(term, 1))
        {
            termFrequency[term]++;
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

IEnumerable<string> allTerms = termFrequency.Keys; // If the term appears at least once it will be in the term frequency dictionary, so instead of parsing the corpus again we re-use this.
int articleCount = wiki.Length;

foreach (string term in allTerms)
{
    inverseDocumentFrequency[term] = Math.Log(articleCount, termOccurrence[term]);
}

Console.Write(
    JsonSerializer.Serialize(termFrequency));