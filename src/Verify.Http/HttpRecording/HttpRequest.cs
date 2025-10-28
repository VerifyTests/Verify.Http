namespace VerifyTests.Http;

public class HttpRequest
{
    public HttpRequest(HttpRequestMessage request)
    {
        Uri = request.RequestUri;
        Method = request.Method;
        Method = request.Method;
        Version = request.Version;
        VersionPolicy = request.VersionPolicy;

        var headers = request.Headers.Simplify();
        if (headers.Count != 0)
        {
            Headers = headers;
        }

        var content = request.Content;
        if (content is not null)
        {
            var contentHeaders = content.Headers.Simplify();
            if (contentHeaders.Count != 0)
            {
                ContentHeaders = contentHeaders;
            }

            var stringContent = content.TryReadStringContent();
            ContentStringParsed = stringContent.prettyContent;
            ContentString = stringContent.content;
        }
    }

    public HttpVersionPolicy VersionPolicy { get; }
    public HttpMethod Method { get; }
    public Version Version { get; }
    public Uri? Uri { get; }
    public Dictionary<string, object>? Headers { get; }
    public Dictionary<string, object>? ContentHeaders { get; }
    public object? ContentStringParsed { get; }
    public string? ContentString { get; }
}