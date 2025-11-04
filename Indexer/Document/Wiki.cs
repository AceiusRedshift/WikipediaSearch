using System.Xml.Serialization;
namespace Indexer.Document;

[XmlRoot("xml")]
public class Wiki
{
    [XmlElement("page")] public Article[] Articles { get; set; } = [];
}