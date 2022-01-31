using System.Net.Http.Headers;

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
#if NET5_0_OR_GREATER
        VersionPolicy = request.VersionPolicy;
#endif

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

#if NET5_0_OR_GREATER
    public HttpVersionPolicy VersionPolicy { get; }
#endif
    public HttpMethod Method { get; }
    public Version Version { get; }
    public Uri Uri { get; }
    public HttpRequestHeaders? Headers { get; }
    public HttpContentHeaders? ContentHeaders { get; }
    public object? ContentStringParsed { get; }
    public string? ContentString { get; }
}