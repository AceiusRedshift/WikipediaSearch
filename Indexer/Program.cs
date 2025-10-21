using Indexer;

var fileStream = File.OpenRead("SmallWiki.xml");
StreamReader reader = new(fileStream);

var articles = Parser.Parse(reader);

foreach (Article article in articles)
{
    Console.WriteLine(article.Title);
}