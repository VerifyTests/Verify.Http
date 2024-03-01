namespace VerifyTests.Http;

public class LoggedSend
{
    public Uri? RequestUri { get; }
    public HttpRequestOptions RequestOptions { get; }
    public string RequestMethod { get; }
    public Dictionary<string, string> RequestHeaders { get; }
    public string? RequestContent { get; }
    public HttpStatusCode ResponseStatus { get; }
    public Dictionary<string, string> ResponseHeaders { get; }
    public string? ResponseContent { get; }

    public LoggedSend(
        Uri? requestUri,
        HttpRequestOptions requestOptions,
        string requestMethod,
        Dictionary<string, string> requestHeaders,
        string? requestContent,
        HttpStatusCode responseStatus,
        Dictionary<string, string> responseHeaders,
        string? responseContent)
    {
        RequestUri = requestUri;
        RequestOptions = requestOptions;
        RequestMethod = requestMethod;
        RequestHeaders = requestHeaders;
        RequestContent = requestContent;
        ResponseStatus = responseStatus;
        ResponseHeaders = responseHeaders;
        ResponseContent = responseContent;
    }
}