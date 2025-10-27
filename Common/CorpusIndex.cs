using System.Text.Json;
namespace Common;

public class CorpusIndex
{
    public string[] Terms { get; set; } = [];
    public List<int[]> Documents { get; set; } = [];

    public void ToFile(string path) => File.WriteAllText(path, JsonSerializer.Serialize(this));
}