using System.Net;

namespace VerifyTests.Http;

public class MockHttpClient :
    HttpClient
{
    MockHttpHandler handler;

    public MockHttpClient(Func<HttpRequestMessage, HttpResponseMessage> responseBuilder) :
        this(new MockHttpHandler(responseBuilder))
    {
    }

    public MockHttpClient(IEnumerable<HttpResponseMessage> responses) :
        this(new MockHttpHandler(responses))
    {
    }

    public MockHttpClient(params HttpResponseMessage[] responses) :
        this(new MockHttpHandler(responses))
    {
    }

    public MockHttpClient(HttpResponseMessage response) :
        this(new MockHttpHandler(response))
    {
    }

    public MockHttpClient(HttpStatusCode status = HttpStatusCode.OK) :
        this(new MockHttpHandler(status))
    {
    }

    public MockHttpClient(string content, string mediaType) :
        this(new MockHttpHandler(content, mediaType))
    {
    }

    MockHttpClient(MockHttpHandler handler) :
        base(handler) =>
        this.handler = handler;

    public IReadOnlyCollection<HttpCall> Calls => handler.Calls;
}