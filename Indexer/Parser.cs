using System.Xml.Serialization;
namespace Indexer;

public class Parser
{
    public static Article[] Parse(StreamReader reader)
    {
        XmlSerializer serializer = new(typeof(Corpus));
        reader.ReadToEnd();
        Article[] articles = serializer.Deserialize(reader) as Article[];
        reader.Close();
        
        return articles ?? throw new Exception();
    }
}