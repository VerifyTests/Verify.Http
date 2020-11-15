using System.Net.Http.Headers;

namespace VerifyTests.Web
{
    public class LoggedSend
    {
        public string? RequestContent { get; }
        public HttpRequestHeaders RequestHeaders { get; }
        public string? ResponseContent { get; }
        public HttpResponseHeaders ResponseHeaders { get; }

        public LoggedSend(string? requestContent, HttpRequestHeaders requestHeaders, string? responseContent, HttpResponseHeaders responseHeaders)
        {
            RequestContent = requestContent;
            RequestHeaders = requestHeaders;
            ResponseContent = responseContent;
            ResponseHeaders = responseHeaders;
        }
    }
}