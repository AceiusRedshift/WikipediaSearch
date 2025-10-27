using Common;
namespace Querier;

public static class Lookup
{
    private static readonly CorpusIndex Index = CorpusIndex.FromFile(
        Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "index.json")
    );
    
    public static int[] Terms(string input)
    {
        input.ExtractTerms();
        
        
        
        return [0];
    }
}