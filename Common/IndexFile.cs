using System.Text.Json;
using static System.Environment;
using static System.IO.Path;
namespace Common;

public record IndexFile(Dictionary<int, string> Titles, Dictionary<int, Dictionary<string, int>> TermFrequency, Dictionary<string, int> TermOccurrence, Dictionary<string, double> InverseDocumentFrequency)
{
    public static readonly string Path = Join(GetFolderPath(SpecialFolder.MyDocuments), "WikipediaSearch.Index");

    public static IndexFile Load() => JsonSerializer.Deserialize<IndexFile>(File.ReadAllText(Path)) ?? throw new InvalidOperationException();

    public void Save() => File.WriteAllText(Path, JsonSerializer.Serialize(this));

    public string[] Query(string[] terms) => Titles.Keys
        .OrderBy(j => terms.Sum(term => -(TermFrequency[j].GetValueOrDefault(term, 0) * InverseDocumentFrequency.GetValueOrDefault(term, 999.0))))
        .Select(id => Titles[id])
        .ToArray();
}