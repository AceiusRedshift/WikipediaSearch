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
        
        foreach (CorpusIndexEntry entry in Index.Documents)
        {
            // term, tf, idf
            List<(string, float, float)> termData = [];

            foreach (string term in inputTerms)
            {
                float termFrequency = 0;
                float inverseDocumentFrequency = 0;
            
                // entry.Vector

                termData.Add((term, termFrequency, inverseDocumentFrequency));
            }
        }

        return [0];
    }
}