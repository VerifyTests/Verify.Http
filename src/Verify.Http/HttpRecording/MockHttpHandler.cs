namespace VerifyTests.Http;

public class MockHttpHandler :
    DelegatingHandler
{
    ConcurrentQueue<HttpCall> calls = new();
    Func<HttpRequestMessage, HttpResponseMessage> builder;

    public MockHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> responseBuilder) =>
        builder = responseBuilder;

    ConcurrentBag<IDisposable> disposables = [];

    protected override void Dispose(bool disposing)
    {
        foreach (var disposable in disposables)
        {
            disposable.Dispose();
        }

        base.Dispose(disposing);
    }

    public MockHttpHandler(IEnumerable<HttpResponseMessage> responses)
    {
        var enumerator = responses.GetEnumerator();
        disposables.Add(enumerator);
        builder = _ =>
        {
            var hasNext = enumerator.MoveNext();
            if (!hasNext)
            {
                throw new("Not enough responses provided");
            }

            return enumerator.Current;
        };
    }

    public MockHttpHandler(HttpStatusCode status = HttpStatusCode.OK) =>
        builder = _ => new(status);

    public MockHttpHandler(HttpResponseMessage response) =>
        builder = _ => response;

    public MockHttpHandler(string content, string mediaType) =>
        builder = _ =>
            new(HttpStatusCode.OK)
            {
                Content = new StringContent(content, Encoding.UTF8, mediaType)
            };

    public IReadOnlyCollection<HttpCall> Calls => calls;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, Cancel cancel)
    {
        var response = Add(request);
        return Task.FromResult(response);
    }

    HttpResponseMessage Add(HttpRequestMessage request)
    {
        var response = builder(request);
        disposables.Add(response);
        disposables.Add(request);
        calls.Enqueue(new(request, response));
        return response;
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, Cancel cancel) =>
        Add(request);
}