using System.Net;
namespace Querier.WebServer;

public class Server : IDisposable
{
    private readonly HttpListener listener;
    private bool keepRunning = true;

    private readonly Func<HttpListenerRequest, string> requestHandler;

    public Server(Func<HttpListenerRequest, string> requestHandler)
    {
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:2160/");

        this.requestHandler = requestHandler;
    }

    public void Run()
    {
        listener.Start();
        Console.WriteLine("Server started at " + listener.Prefixes.First());

        while (keepRunning)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(requestHandler(request));
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();

            Console.WriteLine($"[{DateTime.Now}] {request.HttpMethod} {request.Url} ");
        }

        Console.WriteLine("Stopping server.");
        listener.Stop();
    }

    public void Stop() => keepRunning = false;

    public void Dispose() => ((IDisposable)listener).Dispose();
}