using System.Xml.Serialization;
namespace Indexer;

[XmlRoot("xml")]
public class Corpus
{
    [XmlElement("page")] public Article[] Articles { get; set; } = [];
}