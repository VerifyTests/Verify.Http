namespace VerifyTests.Http;

public class HttpResponse
{
    public HttpResponse(HttpResponseMessage response)
    {
        Status = response.StatusCode;
        var headers = response.Headers.Simplify();
        if (headers.Count != 0)
        {
            Headers = headers;
        }

        var content = (HttpContent?)response.Content;
        if (content != null)
        {
            var httpContentHeaders = content.Headers.Simplify();
            if (httpContentHeaders.Count != 0)
            {
                ContentHeaders = httpContentHeaders;
            }
        }

        var trailingHeaders = response.TrailingHeaders.Simplify();
        if (trailingHeaders.Count != 0)
        {
            TrailingHeaders = trailingHeaders;
        }

        var stringContent = content.TryReadStringContent();
        ContentStringParsed = stringContent.prettyContent;
        ContentString = stringContent.content;
    }

    public HttpStatusCode Status { get; }
    public Dictionary<string, object>? Headers { get; }
    public Dictionary<string, object>? TrailingHeaders { get; }
    public Dictionary<string, object>? ContentHeaders { get; }
    public object? ContentStringParsed { get; }

    [JsonIgnore]
    public string? ContentString { get; }
}