using System.Diagnostics;
using System.Text.RegularExpressions;
using Indexer;

internal partial class Program
{
    [GeneratedRegex("\"\\[\\[[^\\[]+?\\]\\]|[^\\W_]+’[^\\W_]+|[^\\W_]+\"")]
    private static partial Regex GeneratedTokenMatchingRegexPattern();
    
    static string SanitizedMatch(Match match) => match.Value.ToLower().Trim('"');

    static List<string[]> AllTerms(Article[] articles)
    {
        List<string[]> allTerms = [];  
    
        foreach (Article article in articles)
        {
            Regex tokenMatchingPattern = GeneratedTokenMatchingRegexPattern();

            string[] terms = tokenMatchingPattern
                .Matches(article.Text)
                .Select(SanitizedMatch)
                .Where(StopWords.IsNotStopWord)
                .Select(word => PorterStemmer.StemOneWord(word, new PorterStemmerImpl()))
                .ToArray();
    
            allTerms.Add(terms);
    
            // Console.WriteLine($"{article.Title.Trim()} (ID {article.Id}): {string.Join(',', terms)}");
        }

        return allTerms;
    }
    
    public static void Main(string[] args)
    {
        Stopwatch timer = Stopwatch.StartNew();

        var articles = Parser.ParseFile("SmallWiki.xml");
        timer.Report("Corpus Parsed");

        List<string[]> allTerms = AllTerms(articles);  
        timer.Report("Terms Tokenized & Stemmed");
    }
}
