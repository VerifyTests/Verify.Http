namespace VerifyTests.Http;

public class RecordingHandler :
    DelegatingHandler
{
    private Func<HttpContent?, Task<string?>> _customRequestContentRetriever = DefaultIsTextContentRetriever;
    private Func<HttpContent?, Task<string?>> _customResponseContentRetriever = DefaultIsTextContentRetriever;

    public ConcurrentQueue<LoggedSend> Sends = new();

    public void Resume()
    {
        Recording = true;
    }

    public void Pause()
    {
        Recording = false;
    }

    public RecordingHandler(bool recording = true)
    {
        Recording = recording;
    }

    public bool Recording { get; private set; }

    public void SetCustomRequestContentRetriever(Func<HttpContent?, Task<string?>> func)
    {
        _customRequestContentRetriever = func ?? throw new ArgumentNullException(nameof(func));
    }

    public void SetCustomResponseContentRetriever(Func<HttpContent?, Task<string?>> func)
    {
        _customResponseContentRetriever = func ?? throw new ArgumentNullException(nameof(func));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellation)
    {
        if (!Recording)
        {
            return await base.SendAsync(request, cancellation);
        }
 
        var requestContent = request.Content;
        var requestText = await _customRequestContentRetriever.Invoke(requestContent);
        var response = await base.SendAsync(request, cancellation);

        var responseContent = response.Content;
        var responseText = await _customResponseContentRetriever.Invoke(responseContent);

        var item = new LoggedSend(
            request.RequestUri,
#if(NET5_0_OR_GREATER)
                request.Options,
#endif
            request.Method.ToString(),
            request.Headers.ToDictionary(),
            requestText,
            response.StatusCode,
            response.Headers.ToDictionary(),
            responseText);
        Sends.Enqueue(item);

        return response;
    }

    private static async Task<string?> DefaultIsTextContentRetriever(HttpContent? content)
    {
        if (content == null)
            return null;

        if (!content.IsText())
            return null;

        return await content.ReadAsStringAsync();
    }
}