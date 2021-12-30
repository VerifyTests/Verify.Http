using System.Net;

namespace VerifyTests.Http;

public class LoggedSend
{
    public Uri? RequestUri { get; }
#if(NET5_0_OR_GREATER)
    public HttpRequestOptions RequestOptions { get; }
#endif
    public string RequestMethod { get; }
    public Dictionary<string, string> RequestHeaders { get; }
    public string? RequestContent { get; }
    public HttpStatusCode ResponseStatus { get; }
    public Dictionary<string, string> ResponseHeaders { get; }
    public string? ResponseContent { get; }

    public LoggedSend(
        Uri? requestUri,
        #if(NET5_0_OR_GREATER)
        HttpRequestOptions requestOptions,
        #endif
        string requestMethod,
        Dictionary<string, string> requestHeaders,
        string? requestContent,
        HttpStatusCode responseStatus,
        Dictionary<string, string> responseHeaders,
        string? responseContent)
    {
        RequestUri = requestUri;
#if(NET5_0_OR_GREATER)
        RequestOptions = requestOptions;
#endif
        RequestMethod = requestMethod;
        RequestHeaders = requestHeaders;
        RequestContent = requestContent;
        ResponseStatus = responseStatus;
        ResponseHeaders = responseHeaders;
        ResponseContent = responseContent;
    }
}