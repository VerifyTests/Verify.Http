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

        if (response.Content.Headers.Any())
        {
            ContentHeaders = response.Content.Headers;
        }

        var stringContent = response.Content.TryReadStringContent();
        ContentString = stringContent.prettyContent;
        ContentStringRaw = stringContent.content;
    }

    public HttpStatusCode Status { get; }
    public HttpResponseHeaders? Headers { get; }
    public HttpContentHeaders? ContentHeaders { get; }
    public string? ContentString { get; }
    [JsonIgnore] public string? ContentStringRaw { get; }
}