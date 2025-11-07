using Querier.WebServer;
using Server server = new(Renderer.HandleRequest);
server.Run();