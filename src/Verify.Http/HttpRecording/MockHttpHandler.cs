namespace VerifyTests.Http;

public class MockHttpHandler :
    DelegatingHandler
{
    ConcurrentQueue<HttpCall> calls = new();
    Func<HttpRequestMessage, HttpResponseMessage> builder;

    public MockHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> responseBuilder) =>
        builder = responseBuilder;

    ConcurrentBag<IDisposable> disposables = new();

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
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(content, Encoding.UTF8, mediaType);
            return response;
        };

    public IReadOnlyCollection<HttpCall> Calls => calls;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellation)
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

#if !NETCOREAPP3_1 && !NET48 && !NET461 && !NETSTANDARD2_0

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellation) =>
        Add(request);

#endif
}