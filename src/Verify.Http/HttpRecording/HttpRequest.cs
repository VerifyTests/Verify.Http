namespace VerifyTests.Http;

public class HttpRequest
{
    public HttpRequest(HttpRequestMessage request)
    {
        if (request.RequestUri == null)
        {
            throw new ArgumentNullException("HttpRequestMessage.RequestUri");
        }

        Uri = request.RequestUri;
        Method = request.Method;
        Method = request.Method;
        Version = request.Version;
        VersionPolicy = request.VersionPolicy;

        if (request.Headers.Any())
        {
            Headers = request.Headers;
        }

        if (request.Content is not null)
        {
            if (request.Content.Headers.Any())
            {
                ContentHeaders = request.Content.Headers;
            }

            var stringContent = request.Content.TryReadStringContent();
            ContentStringParsed = stringContent.prettyContent;
            ContentString = stringContent.content;
        }
    }

    public HttpVersionPolicy VersionPolicy { get; }
    public HttpMethod Method { get; }
    public Version Version { get; }
    public Uri Uri { get; }
    public HttpRequestHeaders? Headers { get; }
    public HttpContentHeaders? ContentHeaders { get; }
    public object? ContentStringParsed { get; }
    public string? ContentString { get; }
}