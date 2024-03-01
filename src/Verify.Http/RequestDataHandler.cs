namespace VerifyTests.Http;

public class LoggedSend(
    Uri? requestUri,
    HttpRequestOptions requestOptions,
    string requestMethod,
    Dictionary<string, string> requestHeaders,
    string? requestContent,
    HttpStatusCode responseStatus,
    Dictionary<string, string> responseHeaders,
    string? responseContent)
{
    public Uri? RequestUri { get; } = requestUri;
    public HttpRequestOptions RequestOptions { get; } = requestOptions;
    public string RequestMethod { get; } = requestMethod;
    public Dictionary<string, string> RequestHeaders { get; } = requestHeaders;
    public string? RequestContent { get; } = requestContent;
    public HttpStatusCode ResponseStatus { get; } = responseStatus;
    public Dictionary<string, string> ResponseHeaders { get; } = responseHeaders;
    public string? ResponseContent { get; } = responseContent;
}