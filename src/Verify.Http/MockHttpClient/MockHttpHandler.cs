namespace VerifyTests.Http;

public class MockHttpHandler :
    DelegatingHandler
{
    bool recording;
    ConcurrentQueue<HttpCall> calls = [];
    Func<HttpRequestMessage, HttpResponseMessage> builder;

    public MockHttpHandler(Func<HttpRequestMessage, HttpResponseMessage> responseBuilder, bool recording = false)
    {
        builder = responseBuilder;
        this.recording = recording;
    }

    ConcurrentBag<IDisposable> disposables = [];

    protected override void Dispose(bool disposing)
    {
        foreach (var disposable in disposables)
        {
            disposable.Dispose();
        }

        base.Dispose(disposing);
    }

    public MockHttpHandler(params IEnumerable<string> files) :
        this(files, false)
    {
    }

    public MockHttpHandler(IEnumerable<string> files, bool recording = false)
    {
        this.recording = recording;
        // ReSharper disable once GenericEnumeratorNotDisposed
        var enumerator = files.GetEnumerator();
        disposables.Add(enumerator);
        builder = _ =>
        {
            var hasNext = enumerator.MoveNext();
            if (!hasNext)
            {
                throw new("Not enough responses provided");
            }

            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new FileContent(enumerator.Current)
            };
            return message;
        };
    }
    public MockHttpHandler(params IEnumerable<HttpResponseMessage> responses) :
        this(responses, false)
    {
    }

    public MockHttpHandler(IEnumerable<HttpResponseMessage> responses, bool recording = false)
    {
        this.recording = recording;
        // ReSharper disable once GenericEnumeratorNotDisposed
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

    public MockHttpHandler(params IEnumerable<HttpStatusCode> statuses)
        :
        this(statuses, false)
    {
    }

    public MockHttpHandler(IEnumerable<HttpStatusCode> statuses, bool recording = false)
    {
        this.recording = recording;
        // ReSharper disable once GenericEnumeratorNotDisposed
        var enumerator = statuses.GetEnumerator();
        disposables.Add(enumerator);
        builder = _ =>
        {
            var hasNext = enumerator.MoveNext();
            if (!hasNext)
            {
                throw new("Not enough responses provided");
            }

            return new(enumerator.Current);
        };
    }

    public MockHttpHandler(HttpStatusCode status = HttpStatusCode.OK, bool recording = false)
    {
        this.recording = recording;
        builder = _ => new(status);
    }

    public MockHttpHandler(HttpResponseMessage response, bool recording = false)
    {
        this.recording = recording;
        builder = _ => response;
    }

    public MockHttpHandler(string content, string mediaType, bool recording = false)
    {
        this.recording = recording;
        builder = _ =>
            new(HttpStatusCode.OK)
            {
                Content = new StringContent(content, Encoding.UTF8, mediaType)
            };
    }

    public IReadOnlyCollection<HttpCall> Calls => calls;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, Cancel cancel)
    {
        var response = Add(request);
        if (recording &&
            Recording.IsRecording())
        {
            Recording.Add(
                "httpCall",
                new HttpCall(request, response, TimeSpan.Zero));
        }

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