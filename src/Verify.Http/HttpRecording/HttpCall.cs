namespace VerifyTests.Http;

public class HttpCall
{
    public HttpCall(HttpRequestMessage request, HttpResponseMessage response, TimeSpan? duration = null, TaskStatus? status = null)
    {
        Request = new(request);
        Response = new(response);

        if (status != TaskStatus.RanToCompletion)
        {
            Status = status;
        }

        Duration = duration;
    }

    [JsonIgnore] public TimeSpan? Duration { get; }

    public TaskStatus? Status { get; }

    public HttpRequest Request { get; }

    public HttpResponse Response { get; }
}