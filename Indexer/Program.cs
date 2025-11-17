using System.Diagnostics;
using System.Text.Json;
using Common;
using Indexer.Document;

// The total number of times tf the term appears in the corpus.
Dictionary<int, Dictionary<string, int>> termFrequency = [];
// The total number of documents n the term appears in.
Dictionary<string, int> termOccurrence = [];
Dictionary<string, double> inverseDocumentFrequency = [];
Dictionary<int, string> titles = [];
Dictionary<int, List<int>> links = [];

string path = args.ElementAtOrDefault(0) ?? "SmallWiki.xml";
Stopwatch timer = Stopwatch.StartNew();

Console.WriteLine($"Parsing {path}");
Article[] wiki = Parser.ParseFile(path);

int i = 0;
float j = wiki.Length;
foreach (Article article in wiki)
{
    int id = article.Id;
    string title = article.Title.Trim();
    string[] terms = article.Text.ToTerms();

    termFrequency[id] = [];
    titles[id] = title;
    links[id] = [];

    int skippedLinks = 0;
    foreach (string term in terms)
    {
        bool isLink = term.StartsWith("[[") && term.EndsWith("]]");
        if (isLink)
        {
            string linkText = term.TrimStart('[').TrimEnd(']');
            Article? linkedArticle = wiki.FirstOrDefault(a => a.Title.Trim().Contains(linkText, StringComparison.CurrentCultureIgnoreCase));

            if (linkedArticle is null)
            {
                skippedLinks++;
            }
            else
            {
                links[id].Add(linkedArticle.Id);
            }
        }

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

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"Article {article.Title.Trim()} links to {skippedLinks} articles outside of the corpus.");
    Console.ResetColor();

    DrawProgress(i / j);

    i++;
}

IEnumerable<string> allTerms = termOccurrence.Keys;// If the term appears at least once it will be in the occurrence dictionary, so instead of parsing the corpus again we re-use this.
int articleCount = wiki.Length;

foreach (string term in allTerms)
{
    inverseDocumentFrequency[term] = Math.Log((float)articleCount / termOccurrence[term]);
}

IndexFile index = new(titles, termFrequency, termOccurrence, inverseDocumentFrequency);
index.Save();

DrawProgress(1);

Console.WriteLine(JsonSerializer.Serialize(links));

Console.WriteLine($"Done in {timer.Elapsed:g}");

return;

void DrawProgress(float progress)
{
    string dial = $"] [{(progress * 100):000.000}%]";

    Console.Write("[");

    int width = Console.BufferWidth - (dial.Length + 1);
    int barWidth = (int)(width * progress);
    for (int k = 0; k < barWidth; k++)
    {
        Console.Write('.');
    }
    for (int k = 0; k < (width - barWidth); k++)
    {
        Console.Write(' ');
    }

    Console.WriteLine(dial);
}