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
            ContentString = stringContent.prettyContent;
            ContentStringRaw = stringContent.content;
        }
    }

    public Uri Uri { get; }

    public HttpRequestHeaders? Headers { get; }
    public HttpContentHeaders? ContentHeaders { get; }
    public string? ContentString { get; }
    public string? ContentStringRaw { get; }
}