using System.Net.Sockets;

// Minimal loopback http server so tests can use a real HttpClient (and hence
// DiagnosticsHandler and HttpListener) without depending on an external site.
static class TestServer
{
    static TcpListener listener = Start();

    public static string Root { get; } = $"http://127.0.0.1:{((IPEndPoint) listener.LocalEndpoint).Port}";

    static TcpListener Start()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        _ = Task.Run(() => AcceptLoop(listener));
        return listener;
    }

    static async Task AcceptLoop(TcpListener listener)
    {
        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = Task.Run(() => Handle(client));
        }
    }

    static async Task Handle(TcpClient client)
    {
        using (client)
        {
            var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, leaveOpen: true);
            var requestLine = await reader.ReadLineAsync();
            string? line;
            do
            {
                line = await reader.ReadLineAsync();
            } while (!string.IsNullOrEmpty(line));

            var path = requestLine!.Split(' ')[1];
            var (status, contentType, body) = Content(path);
            var head =
                $"HTTP/1.1 {status}\r\n" +
                $"Content-Type: {contentType}\r\n" +
                $"Content-Length: {body.Length}\r\n" +
                "Connection: close\r\n" +
                "\r\n";
            await stream.WriteAsync(Encoding.ASCII.GetBytes(head));
            await stream.WriteAsync(body);
        }
    }

    static (string status, string contentType, byte[] body) Content(string path) =>
        path switch
        {
            "/get" or "/json" => ("200 OK", "application/json", Read("sample.json")),
            "/xml" => ("200 OK", "application/xml", Read("sample.xml")),
            "/html" => ("200 OK", "text/html", Read("sample.html")),
            _ => ("404 Not Found", "text/plain", "Not Found"u8.ToArray())
        };

    static byte[] Read(string file) =>
        File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, file));
}
