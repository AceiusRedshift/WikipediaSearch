using System.Xml.Serialization;
namespace Indexer.Document;

public static class Parser
{
    public static Article[] ParseFile(string path)
    {
        XmlSerializer serializer = new(typeof(Wiki));
        StreamReader reader = new(path);
        
        Wiki corpus = serializer.Deserialize(reader) as Wiki;
        reader.Close();

        return corpus?.Articles ?? throw new Exception();
    }
}