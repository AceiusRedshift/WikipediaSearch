using System.Text.Json;
using System.Text.RegularExpressions;
using Common;
using Indexer.Document;

Dictionary<string, int> termFrequency = [];
Dictionary<int, int> inverseDocumentFrequency = [];

Regex regex = new("""\[\[[^\[]+?\]\]|[^\W_]+’[^\W_]+|[^\W_]+""");

var wiki = Parser.ParseFile("SmallWiki.xml");

foreach (Article article in wiki)
{
    int id = article.Id;
    string title = article.Title.Trim();
    string[] terms = PorterStemmer.StemWords(regex.Matches(article.Text).Select(match => match.Value).Where(StopWords.IsNotStopWord).ToArray());

    // if link
    
    foreach (string term in terms)
    {
        if (!termFrequency.TryAdd(term, 1))
        {
            termFrequency[term]++;
        }
    }
    
    Console.Write(
        JsonSerializer.Serialize(termFrequency));
}