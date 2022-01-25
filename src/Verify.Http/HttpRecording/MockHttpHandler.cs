using System.Net;

namespace VerifyTests.Http;

public class MockHttpHandler :
    DelegatingHandler
{
    ConcurrentQueue<HttpCall> calls = new();
    Func<HttpRequestMessage, HttpResponseMessage> builder;

    public MockHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> responseBuilder)
    {
        builder = responseBuilder;
    }

    public MockHttpHandler(HttpStatusCode status = HttpStatusCode.OK)
    {
        builder = _ => new HttpResponseMessage(status);
    }

    public MockHttpHandler(string content, string mediaType)
    {
        builder = _ =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(content, Encoding.UTF8, mediaType);
            return response;
        };
    }

    public IReadOnlyCollection<HttpCall> Calls
    {
        get => calls;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellation)
    {
        var response = Add(request);
        return Task.FromResult(response);
    }

    HttpResponseMessage Add(HttpRequestMessage request)
    {
        var response = builder(request);
        calls.Enqueue(new HttpCall(request, response));
        return response;
    }

#if !NETCOREAPP3_1 && !NET48 && !NET461 && !NETSTANDARD2_0

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellation)
    {
        return Add(request);
    }

#endif
}