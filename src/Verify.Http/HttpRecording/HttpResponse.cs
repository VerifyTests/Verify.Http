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

        if (response.TrailingHeaders.Any())
        {
            TrailingHeaders = response.TrailingHeaders;
        }

        var stringContent = content.TryReadStringContent();
        ContentStringParsed = stringContent.prettyContent;
        ContentString = stringContent.content;
    }

    public HttpStatusCode Status { get; }
    public HttpResponseHeaders? Headers { get; }
    public HttpResponseHeaders? TrailingHeaders { get; }
    public HttpContentHeaders? ContentHeaders { get; }
    public object? ContentStringParsed { get; }
    [JsonIgnore] public string? ContentString { get; }
}