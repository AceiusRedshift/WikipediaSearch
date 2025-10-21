using Indexer;

var articles = Parser.ParseFile("SmallWiki.xml");

foreach (Article article in articles)
{
    Console.WriteLine("#" + article.Title.Trim());
}