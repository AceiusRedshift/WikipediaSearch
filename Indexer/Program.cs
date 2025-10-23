using System.Text.RegularExpressions;
using Indexer;

var articles = Parser.ParseFile("SmallWiki.xml");

foreach (Article article in articles)
{
    Regex tokenMatchingPattern = new("\"\\[\\[[^\\[]+?\\]\\]|[^\\W_]+’[^\\W_]+|[^\\W_]+\"");

    var terms = tokenMatchingPattern
        .Matches(article.Text)
        .Select(SanitizedMatch)
        .Where(StopWords.IsNotStopWord)
        .Select(word => PorterStemmer.StemOneWord(word, new PorterStemmerImpl()));
    
    Console.WriteLine(string.Join(',', terms));
}

return;

static string SanitizedMatch(Match match) => match.Value.ToLower().Trim('"');