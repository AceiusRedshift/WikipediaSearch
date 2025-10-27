using System.Text.Json;
namespace Common;

public class CorpusIndex
{
    public string[] Terms { get; set; } = [];
    public CorpusIndexEntry[] Documents { get; set; } = [];

    public void ToFile(string path) => File.WriteAllText(path, JsonSerializer.Serialize(this));
    
    public static CorpusIndex FromFile(string path) => JsonSerializer.Deserialize<CorpusIndex>(File.ReadAllText(path)) ?? throw new InvalidOperationException();
}