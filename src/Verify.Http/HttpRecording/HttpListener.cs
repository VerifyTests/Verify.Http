// ReSharper disable UnusedParameter.Global
#pragma warning disable CA1822
class HttpListener :
    IObserver<DiagnosticListener>,
    IDisposable
{
    ConcurrentQueue<IDisposable> subscriptions = [];

    public void OnNext(DiagnosticListener value)
    {
        if (value.Name != "HttpHandlerDiagnosticListener")
        {
            return;
        }

        subscriptions.Enqueue(
            value.SubscribeWithAdapter(
                this,
                _ => Recording.IsRecording()));
    }

    [DiagnosticName("System.Net.Http.HttpRequestOut")]
    public void IsEnabled()
    {
    }

    [DiagnosticName("System.Net.Http.HttpRequestOut.Start")]
    public virtual void OnHttpRequestOutStart(HttpRequestMessage request)
    {
    }

    [DiagnosticName("System.Net.Http.HttpRequestOut.Stop")]
    public virtual void OnHttpRequestOutStop(HttpRequestMessage request, HttpResponseMessage response, TaskStatus status)
    {
        if (!Recording.IsRecording())
        {
            return;
        }

        Recording.Add(
            "httpCall",
            new HttpCall(request, response, Activity.Current?.Duration, status));
    }

    void Clear()
    {
        foreach (var subscription in subscriptions)
        {
            subscription.Dispose();
        }
    }

    public void Dispose() =>
        Clear();

    public void OnCompleted() =>
        Clear();

    public void OnError(Exception error)
    {
    }
}