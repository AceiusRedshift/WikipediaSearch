using System.Net;
using System.Text;
using Common;
namespace Querier.WebServer;

public static class Renderer
{
    private static readonly IndexFile Index = IndexFile.Load();

    public static string HandleRequest(HttpListenerRequest request)
    {
        StringBuilder responseBuilder = new();

        return responseBuilder.AppendLine("<html>")
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
            .AppendLine("<form><input name=\"q\"  class=\"card\" placeholder=\"Search Wikipedia...\"></form>")
            .AppendLine(request.QueryString["q"] is { } query
                ? Index.Query(query.ToTerms())
                    .Prepend(string.Empty)
                    .Aggregate((stuff, term) =>
                        $"{stuff}<div class=\"card\">{term}</div>"
                    )
                : "<p>Type a query to continue</p>")
            .AppendLine("</body>")
            .AppendLine("</html>")
            .ToString();
    }
}