using System.Xml.Serialization;
namespace Indexer;

public class Article
{
    [XmlElement("title")]
    public string Title { get; set; }

    [XmlElement("id")]
    public int Id { get; set; }

    [XmlElement("text")]
    public string Text { get; set; }
    
    
}