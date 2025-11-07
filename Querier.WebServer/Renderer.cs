using System.Net;
using System.Text;
using Common;
namespace Querier.WebServer;

public static class Renderer
{
    private static readonly IndexFile Index = IndexFile.Load();

    public static string HandleRequest(HttpListenerRequest request) => new StringBuilder().AppendLine("<html>")
        .AppendLine("<head>")
        .AppendLine("<title>Querier</title>")
        .AppendLine("""
                    <style>
                        body {
                            font-family: Veranda, sans-serif;
                        }
                        
                        form {
                            margin: 0;
                            padding: 0;
                        }
                        
                        .card {
                            margin: 8px;
                            padding: 8px;
                            border: 1px solid black;
                            max-width: calc(100% - 16px);
                        }
                    </style> 
                    """)
        .AppendLine("</head>")
        .AppendLine("<body>")
        .AppendLine("<form><input name=\"q\" class=\"card\" placeholder=\"Search Wikipedia...\"><input class=\"card\" type=\"submit\"></input></form>")
        .AppendLine(request.QueryString["q"] is { } query
            ? Index.Query(query.ToTerms())
                .Prepend(string.Empty)
                .Aggregate((stuff, page) =>
                    $"{stuff}<div class=\"card\"><a href=\"https://en.wikipedia.org/wiki/{page}\">{page}</a></div>"
                )
            : "<p>Type a query to continue</p>")
        .AppendLine("</body>")
        .AppendLine("</html>")
        .ToString();
}