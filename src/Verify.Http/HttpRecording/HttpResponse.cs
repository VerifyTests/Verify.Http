using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace VerifyTests.Http;

public class HttpResponse
{
    public HttpResponse(HttpResponseMessage response)
    {
        Status = response.StatusCode;
        if (response.Headers.Any())
        {
            Headers = response.Headers;
        }

        var content = (HttpContent?) response.Content;
        if (content != null && content.Headers.Any())
        {
            ContentHeaders = content.Headers;
        }

#if NET5_0_OR_GREATER || NETSTANDARD2_1
        if (response.TrailingHeaders.Any())
        {
            TrailingHeaders = response.TrailingHeaders;
        }
#endif

        var stringContent = content.TryReadStringContent();
        ContentString = stringContent.prettyContent;
        ContentStringRaw = stringContent.content;
    }

    public HttpStatusCode Status { get; }
    public HttpResponseHeaders? Headers { get; }
#if NET5_0_OR_GREATER || NETSTANDARD2_1
    public HttpResponseHeaders? TrailingHeaders { get; }
#endif
    public HttpContentHeaders? ContentHeaders { get; }
    public string? ContentString { get; }
    [JsonIgnore] public string? ContentStringRaw { get; }
}