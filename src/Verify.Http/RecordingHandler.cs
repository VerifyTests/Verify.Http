namespace VerifyTests.Http;

public class RecordingHandler :
    DelegatingHandler
{
    public ConcurrentQueue<LoggedSend> Sends = new();

    public void Resume() =>
        Recording = true;

    public void Pause() =>
        Recording = false;

    public RecordingHandler(bool recording = true) =>
        Recording = recording;

    public bool Recording { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, Cancel cancel)
    {
        if (!Recording)
        {
            return await base.SendAsync(request, cancel);
        }
        string? requestText = null;
        var requestContent = request.Content;
        if (requestContent != null)
        {
            if (requestContent.IsText())
            {
                requestText = await requestContent.ReadAsStringAsync(cancel);
            }
        }

        var response = await base.SendAsync(request, cancel);

        var responseContent = response.Content;
        string? responseText = null;
        if (responseContent != null)
        {
            if (responseContent.IsText())
            {
                responseText = await responseContent.ReadAsStringAsync(cancel);
            }
        }

        var item = new LoggedSend(
            request.RequestUri,
#if NET6_0_OR_GREATER
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
}