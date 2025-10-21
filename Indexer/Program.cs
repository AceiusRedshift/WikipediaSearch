using System.Text.RegularExpressions;
using Indexer;

var articles = Parser.ParseFile("SmallWiki.xml");

foreach (Article article in articles)
{
    Regex tokenizer = new("\"\\[\\[[^\\[]+?\\]\\]|[^\\W_]+’[^\\W_]+|[^\\W_]+\"");

    var tokens = tokenizer.Split(article.Text);
    
    Console.WriteLine(string.Join(',', tokens));
}