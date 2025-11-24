namespace VerifyTests.Http;

public class MockHttpClient :
    HttpClient
{
    MockHttpHandler handler;

    public MockHttpClient(Func<HttpRequestMessage, HttpResponseMessage> responseBuilder, bool recording = false) :
        this(new MockHttpHandler(responseBuilder, recording))
    {
    }

    public MockHttpClient(params IEnumerable<HttpResponseMessage> responses) :
        this(responses, false)
    {
    }

    public MockHttpClient(IEnumerable<HttpResponseMessage> responses, bool recording = false) :
        this(new MockHttpHandler(responses, recording))
    {
    }

    public MockHttpClient(params IEnumerable<HttpStatusCode> statuses) :
        this(statuses, false)
    {
    }

    public MockHttpClient(IEnumerable<HttpStatusCode> statuses, bool recording = false) :
        this(new MockHttpHandler(statuses, recording))
    {
    }

    public MockHttpClient(params IEnumerable<string> files) :
        this(files, false)
    {
    }

    public MockHttpClient(IEnumerable<string> files, bool recording = false) :
        this(new MockHttpHandler(files, recording))
    {
    }

    public MockHttpClient(HttpResponseMessage response, bool recording = false) :
        this(new MockHttpHandler(response, recording))
    {
    }

    public MockHttpClient(HttpStatusCode status = HttpStatusCode.OK, bool recording = false) :
        this(new MockHttpHandler(status, recording))
    {
    }

    public MockHttpClient(string content, string mediaType, bool recording = false) :
        this(new MockHttpHandler(content, mediaType, recording))
    {
    }

    MockHttpClient(MockHttpHandler handler) :
        base(handler) =>
        this.handler = handler;

    public IReadOnlyCollection<HttpCall> Calls => handler.Calls;

    public bool SimulateNetworkStream
    {
        get => handler.SimulateNetworkStream;
        set => handler.SimulateNetworkStream = value;
    }
}