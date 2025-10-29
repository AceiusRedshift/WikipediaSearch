using Common;
namespace Querier;

public static class Lookup
{
    private static readonly CorpusIndex Index = CorpusIndex.FromFile(
        Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "index.json")
    );

    public static int[] Terms(string input)
    {
        List<string> inputTerms = input.ExtractTerms().Distinct().ToList();
        Dictionary<int, List<(string, float, float)>> indexScores = [];

        foreach (CorpusIndexEntry entry in Index.Documents)
        {
            // term, tf, idf
            List<(string, float, float)> termData = [];

            for (int i = 0; i < inputTerms.Count; i++)
            {
                int termIndex = Array.IndexOf(Index.Terms, inputTerms[i]);

                float termFrequency = 0;
                float inverseDocumentFrequency = MathF.Log(Index.Documents.Length, Index.Documents.Count(doc => doc.Vector[termIndex] != 0));

                termData.Add((Index.Terms[termIndex], termFrequency, inverseDocumentFrequency));
            }

            indexScores.Add(entry.Id, termData);
        }

        return indexScores.OrderBy(score => score.Value.First().Item3).ToArray().Select(pair => (int)pair.Value.First().Item3).ToArray();
    }
}