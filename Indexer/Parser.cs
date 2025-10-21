using System.Xml.Serialization;
namespace Indexer;

public class Parser
{
    public static Article[] ParseFile(string path)
    {
        XmlSerializer serializer = new(typeof(Corpus));
        StreamReader reader = new(path);
        
        Corpus corpus = serializer.Deserialize(reader) as Corpus;
        reader.Close();

        return corpus?.Articles ?? throw new Exception();
    }
}